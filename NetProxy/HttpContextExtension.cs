using System;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace NetProxy
{
    internal static class HttpContextExtension
    {
        public static HttpRequestMessage CloneRequestFor(this HttpContext context, Uri targetUri)
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
    }
}