using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Configuration;

namespace NetStandardProxy
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
            if (!this.TheRequestHasControllerListening(context))
            {
                return this.Fordward(context);
            }

            return this.next.Invoke(context);
        }

        private bool TheRequestHasControllerListening(HttpContext context)
        {
            var path = context.Request.Path.Value;

            var action = this.GetMatchingAction(path, context.Request.Method);

            return action != null;
        }

        private async Task Fordward(HttpContext context)
        {
            var url = context.Request.Path.ToUriComponent();

            var uri = new Uri($"{this.destinationURL}{url}");

            var request = CopyRequest(context, uri);

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


        private HttpRequestMessage CopyRequest(HttpContext context, Uri targetUri)
        {
            var actualRequest = context.Request;

            var remoteRequest = new HttpRequestMessage()
            {
                Method = new HttpMethod(actualRequest.Method),
                Content = new StreamContent(actualRequest.Body),
                RequestUri = targetUri,
            };

            foreach (var header in actualRequest.Headers)
            {
                remoteRequest.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }

            remoteRequest.Headers.Host = targetUri.Host;

            return remoteRequest;
        }

        private void ValidateDestinationURL(string destinationURL)
        {
            if (!Uri.IsWellFormedUriString(destinationURL, UriKind.Absolute))
                throw new Exception("Please check the destination URL in your appsettings. " +
                    "If you don't added yet, please add this following lines:" +
                    "'ReverseProxy': { 'DestinationURL': 'https://localhost:44322/' }");
        }

        private ActionDescriptor GetMatchingAction(string path, string httpMethod)
        {
            var actionDescriptors = this.actionDescriptorCollectionProvider.ActionDescriptors.Items;

            var matchingDescriptors = new List<ActionDescriptor>();

            foreach (var actionDescriptor in actionDescriptors)
            {
                var matchesRouteTemplate = this.MatchesTemplate(actionDescriptor.AttributeRouteInfo.Template, path);

                if (matchesRouteTemplate)
                {
                    matchingDescriptors.Add(actionDescriptor);
                }
            }

            var httpContext = new DefaultHttpContext();

            httpContext.Request.Path = path;

            httpContext.Request.Method = httpMethod;

            var routeContext = new RouteContext(httpContext);

            return actionSelector.SelectBestCandidate(routeContext, matchingDescriptors.AsReadOnly());
        }


        public bool MatchesTemplate(string routeTemplate, string requestPath)
        {
            var template = TemplateParser.Parse(routeTemplate);

            var matcher = new TemplateMatcher(template, this.GetDefaults(template));

            var values = new RouteValueDictionary();

            return matcher.TryMatch(requestPath, values);
        }

        private RouteValueDictionary GetDefaults(RouteTemplate parsedTemplate)
        {
            var result = new RouteValueDictionary();

            foreach (var parameter in parsedTemplate.Parameters)
            {
                if (parameter.DefaultValue != null)
                {
                    result.Add(parameter.Name, parameter.DefaultValue);
                }
            }

            return result;
        }
    }
}
