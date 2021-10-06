using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Exeal.StranglerProxy
{
    internal static class HttpContextExtension
    {
        public static HttpRequestMessage CloneRequestFor(this HttpContext context, Uri targetUri)
        {
            context.Features.Get<IHttpBodyControlFeature>().AllowSynchronousIO = true;

            var actualRequest = context.Request;

            using (var bodyReader = new StreamReader(actualRequest.Body))
            {
                var remoteRequest = new HttpRequestMessage
                {
                    Method = new HttpMethod(actualRequest.Method),
                    RequestUri = targetUri,
                };

                CloneRequestContent(bodyReader, actualRequest, remoteRequest);

                CloneRequestHeaders(actualRequest, remoteRequest);

                remoteRequest.Headers.Host = targetUri.Host;

                return remoteRequest;
            }
        }

        private static void CloneRequestContent(StreamReader bodyReader, HttpRequest actualRequest,
            HttpRequestMessage remoteRequest) {
            var actualContent = bodyReader.ReadToEnd();

            if (string.IsNullOrEmpty(actualContent)) return;
            var contentType = new ContentType(actualRequest.ContentType);
            remoteRequest.Content = new StringContent(actualContent, bodyReader.CurrentEncoding, contentType.MediaType);
            remoteRequest.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType.MediaType);
        }

        private static void CloneRequestHeaders(HttpRequest actualRequest, HttpRequestMessage remoteRequest) {
            foreach (var (headerName, value) in actualRequest.Headers) {
                TryAddHeader(remoteRequest, headerName, value.ToArray());
            }
        }

        private static void TryAddHeader(HttpRequestMessage remoteRequest, string headerName, string[] headerValue) {
            var isContentTypeHeader = headerName.Equals("content-type", StringComparison.OrdinalIgnoreCase);
            if(isContentTypeHeader) return;

            if (!remoteRequest.Headers.TryAddWithoutValidation(headerName, headerValue))
                remoteRequest.Content?.Headers.TryAddWithoutValidation(headerName, headerValue);
        }
    }
}