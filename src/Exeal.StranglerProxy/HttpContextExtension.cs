using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace Exeal.StranglerProxy
{
    internal static class HttpContextExtension
    {
        private static string AuthorizationKey = "Authorization";

        public static HttpRequestMessage CloneRequestFor(this HttpContext context, Uri targetUri)
        {
            var actualRequest = context.Request;

            var remoteRequest = new HttpRequestMessage
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

            if (actualRequest.Headers.Keys.Any(key => key == AuthorizationKey))
                remoteRequest.Headers.Authorization = AuthenticationHeaderValue.Parse(actualRequest.Headers[AuthorizationKey]);

            return remoteRequest;
        }
    }
}