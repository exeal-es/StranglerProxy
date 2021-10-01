using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

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
                var headerName = header.Key;
                var headerValue = header.Value.ToArray();

                if (!remoteRequest.Headers.TryAddWithoutValidation(headerName, headerValue))
                    remoteRequest.Content?.Headers.TryAddWithoutValidation(headerName, headerValue);
            }

            remoteRequest.Headers.Host = targetUri.Host;

            if (actualRequest.Headers.Keys.Any(key => key == AuthorizationKey))
                remoteRequest.Headers.Authorization = AuthenticationHeaderValue.Parse(actualRequest.Headers[AuthorizationKey]);

            return remoteRequest;
        }
    }
}