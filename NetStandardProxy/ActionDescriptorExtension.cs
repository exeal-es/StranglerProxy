using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;

namespace NetStandardProxy
{
    public static class ActionDescriptorExtension
    {
        public static MatchigDescriptor GetPosibleActionMatchersFor(this IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, string path, string httpMethod)
        {
            var matchingDescriptors = actionDescriptorCollectionProvider
                                          .ActionDescriptors
                                          .Items
                                          .Where(actionDescriptor => HasMatcherController(actionDescriptor.AttributeRouteInfo.Template, path))
                                          .ToList();

            var httpContext = new DefaultHttpContext();

            httpContext.Request.Path = path;

            httpContext.Request.Method = httpMethod;

            var routeContext = new RouteContext(httpContext);

            return new MatchigDescriptor(matchingDescriptors, routeContext);
        }

        public static bool HasMatcherController(this IActionSelector actionSelector, MatchigDescriptor matchigDescriptor)
        {
            var actionMatch = actionSelector.SelectBestCandidate(matchigDescriptor.RouteContext, matchigDescriptor.MatchingDescriptors.AsReadOnly());

            return actionMatch != null;
        }

        private static bool HasMatcherController(string routeTemplate, string requestPath)
        {
            var template = TemplateParser.Parse(routeTemplate);

            var matcher = new TemplateMatcher(template, GetDefaults(template));

            var values = new RouteValueDictionary();

            return matcher.TryMatch(requestPath, values);
        }

        private static RouteValueDictionary GetDefaults(RouteTemplate parsedTemplate)
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

        public class MatchigDescriptor
        {
            internal MatchigDescriptor(List<ActionDescriptor> matchingDescriptors, RouteContext routeContext)
            {
                this.MatchingDescriptors = matchingDescriptors;

                this.RouteContext = routeContext;
            }

            public RouteContext RouteContext { get; }

            public List<ActionDescriptor> MatchingDescriptors { get; }
        }
    }
}
