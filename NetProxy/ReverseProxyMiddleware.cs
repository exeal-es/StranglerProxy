using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace NetProxy
{
    internal class ReverseProxyMiddleware
    {
        private readonly RequestDelegate next;

        private readonly IActionDescriptorCollectionProvider actionDescriptorCollectionProvider;

        private readonly IActionSelector actionSelector;

        private readonly HttpClient httpclient;

        private readonly String destinationURL;

        public ReverseProxyMiddleware(RequestDelegate next,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
            IActionSelector actionSelector,
            IConfiguration configuration)
        {
            this.next = next;

            this.actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;

            this.actionSelector = actionSelector;

            httpclient = new HttpClient();

            destinationURL = configuration["ReverseProxy:DestinationURL"];

            ValidateDestinationURL(destinationURL);
        }

        public Task Invoke(HttpContext context)
        {
            if (!HasMatcherController(context))
            {
                return Forward(context);
            }

            return next.Invoke(context);
        }

        private bool HasMatcherController(HttpContext context)
        {
            var path = context.Request.Path.Value;

            var method = context.Request.Method;

            var matcher = actionDescriptorCollectionProvider.GetPossibleActionMatchersFor(path, method);

            return actionSelector.HasMatcherController(matcher);
        }

        private async Task Forward(HttpContext context)
        {
            var url = context.Request.Path.ToUriComponent();

            var uri = new Uri($"{destinationURL}{url}");

            var request = context.CloneRequestFor(uri);

            var remoteResponse = await httpclient.SendAsync(request);

            var actualResponse = context.Response;

            foreach (var header in remoteResponse.Headers)
            {
                actualResponse.Headers.Add(header.Key, header.Value.ToArray());
            }

            actualResponse.ContentType = remoteResponse.Content.Headers.ContentType?.ToString();

            actualResponse.ContentLength = remoteResponse.Content.Headers.ContentLength;

            await remoteResponse.Content.CopyToAsync(actualResponse.Body);
        }

        private void ValidateDestinationURL(string destinationURL)
        {
            if (!Uri.IsWellFormedUriString(destinationURL, UriKind.Absolute))
                throw new Exception("Please check the destination URL in your appsettings. " +
                                    "If you don't added yet, please add this following lines:" +
                                    "'ReverseProxy': { 'DestinationURL': 'https://localhost:44322/' }");
        }
    }
}