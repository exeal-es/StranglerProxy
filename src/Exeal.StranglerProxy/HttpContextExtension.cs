using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;

namespace Exeal.StranglerProxy
{
    internal static class HttpContextExtension
    {
        public static HttpRequestMessage CloneRequestFor(this HttpContext context, Uri targetUri)
        {
            var actualRequest = context.Request;

            var remoteRequest = new HttpRequestMessage
            {
                Method = new HttpMethod(actualRequest.Method),
                Content = new StreamContent(actualRequest.Body),
                RequestUri = targetUri,
            };

            CloneHeaders(actualRequest, remoteRequest);

            remoteRequest.Headers.Host = targetUri.Host;

            return remoteRequest;
        }

        private static void CloneHeaders(HttpRequest actualRequest, HttpRequestMessage remoteRequest)
        {
            foreach (var header in actualRequest.Headers)
            {
                var headerName = header.Key;
                var headerValue = header.Value.ToArray();

                if (!remoteRequest.Headers.TryAddWithoutValidation(headerName, headerValue))
                    remoteRequest.Content?.Headers.TryAddWithoutValidation(headerName, headerValue);
            }
        }
    }
}