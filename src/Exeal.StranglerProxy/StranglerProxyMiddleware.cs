using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace Exeal.StranglerProxy
{
    internal class StranglerProxyMiddleware
    {
        private readonly RequestDelegate next;

        private readonly IActionDescriptorCollectionProvider actionDescriptorCollectionProvider;

        private readonly IActionSelector actionSelector;

        private readonly String destinationURL;

        public StranglerProxyMiddleware(RequestDelegate next,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
            IActionSelector actionSelector,
            IConfiguration configuration)
        {
            this.next = next;

            this.actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;

            this.actionSelector = actionSelector;

            destinationURL = configuration["StranglerProxy:DestinationURL"];

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

        private Task Forward(HttpContext context)
        {
            var url = context.Request.Path.ToUriComponent();

            var queryString = context.Request.QueryString.ToUriComponent();

            var uri = new Uri($"{destinationURL}{url}{queryString}");

            return context.ProxyRequest(uri);
        }

        private void ValidateDestinationURL(string destinationURL)
        {
            if (!Uri.IsWellFormedUriString(destinationURL, UriKind.Absolute))
                throw new Exception("Please check the destination URL in your appsettings. " +
                                    "If you don't added yet, please add this following lines:" +
                                    "'StranglerProxy': { 'DestinationURL': 'https://localhost:44322/' }");
        }
    }
}