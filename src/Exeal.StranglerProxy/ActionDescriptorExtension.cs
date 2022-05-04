using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;

namespace Exeal.StranglerProxy
{
    internal static class ActionDescriptorExtension
    {
        public static MatchingDescriptor GetPossibleActionMatchersFor(
            this IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, HttpContext currentContext)
        {
            var path = currentContext.Request.Path.Value;

            var actionDescriptors = actionDescriptorCollectionProvider.GetMatchersDescriptorsFor(path);

            var routeContext = new RouteContext(currentContext);

            return new MatchingDescriptor(routeContext, actionDescriptors);
        }

        public static bool HasMatcherController(this IActionSelector actionSelector,
            MatchingDescriptor matchingDescriptor)
        {
            var actionMatch = actionSelector.SelectBestCandidate(matchingDescriptor.RouteContext,
                matchingDescriptor.ActionDescriptors);

            return actionMatch != null;
        }

        private static IEnumerable<ActionDescriptor> GetMatchersDescriptorsFor(
            this IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, string requestPath)
        {
            return actionDescriptorCollectionProvider.ActionDescriptors
                .Items
                .Where(actionDescriptor =>
                    HasMatcherController(actionDescriptor.AttributeRouteInfo.Template, requestPath));
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

        internal class MatchingDescriptor
        {
            internal MatchingDescriptor(RouteContext routeContext, IEnumerable<ActionDescriptor> matchingDescriptors)
            {
                RouteContext = routeContext;

                ActionDescriptors = matchingDescriptors.ToList().AsReadOnly();
            }

            public RouteContext RouteContext { get; }

            public ReadOnlyCollection<ActionDescriptor> ActionDescriptors { get; }
        }
    }
}