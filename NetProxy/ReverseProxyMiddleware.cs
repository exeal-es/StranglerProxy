using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace NetProxy
{
    public class ReverseProxyMiddleware
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

            this.httpclient = new HttpClient();

            this.destinationURL = configuration["ReverseProxy:DestinationURL"];

            this.ValidateDestinationURL(this.destinationURL);
        }

        public Task Invoke(HttpContext context)
        {
            if (!this.HasMatcherController(context))
            {
                return this.Fordward(context);
            }

            return this.next.Invoke(context);
        }

        private bool HasMatcherController(HttpContext context)
        {
            var path = context.Request.Path.Value;

            var method = context.Request.Method;

            var matcher = this.actionDescriptorCollectionProvider.GetPosibleActionMatchersFor(path, method);

            return this.actionSelector.HasMatcherController(matcher);
        }

        private async Task Fordward(HttpContext context)
        {
            var url = context.Request.Path.ToUriComponent();

            var uri = new Uri($"{this.destinationURL}{url}");

            var request = context.CloneRequestFor(uri);

            var remoteResponse = await this.httpclient.SendAsync(request);

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
