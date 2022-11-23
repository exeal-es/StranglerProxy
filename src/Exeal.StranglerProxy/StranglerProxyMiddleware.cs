using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        private readonly ICollection<string> excludedResources;

        public StranglerProxyMiddleware(RequestDelegate next,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
            IActionSelector actionSelector,
            IConfiguration configuration)
        {
            this.next = next;

            this.actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;

            this.actionSelector = actionSelector;

            destinationURL = configuration["StranglerProxy:DestinationURL"];

            excludedResources = configuration.GetSection("StranglerProxy:ExcludedResources").Get<List<string>>() ?? new List<string>();

            ValidateDestinationURL(destinationURL);
        }

        public Task Invoke(HttpContext context)
        {
            if (PathIsExcluded(context) || !HasMatcherController(context))
            {
                return Forward(context);
            }
            return next.Invoke(context);
        }

        private bool HasMatcherController(HttpContext currentContext)
        {
            var matcher = actionDescriptorCollectionProvider.GetPossibleActionMatchersFor(currentContext);

            return actionSelector.HasMatcherController(matcher);
        }

        private bool PathIsExcluded(HttpContext currentContext)
        {
            var requestedPath = currentContext.Request.Path.ToUriComponent();

            return excludedResources.Any(MatchesWith(requestedPath));
        }

        private static Func<string, bool> MatchesWith(string requestedPath)
        {
            return excludedResource =>
            {
                requestedPath = requestedPath.ToLower();

                var pattern = $@"^([/]{excludedResource.ToLower()}([/]|$))";

                return Regex.IsMatch(requestedPath, pattern);
            }; 
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