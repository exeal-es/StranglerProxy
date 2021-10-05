using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Exeal.StranglerProxy
{
    internal static class HttpContextExtension
    {
        private static string AuthorizationKey = "Authorization";

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

                var actualContent = bodyReader.ReadToEnd();

                if (!String.IsNullOrEmpty(actualContent))
                {
                    var contentType = new ContentType(actualRequest.ContentType);

                    remoteRequest.Content = new StringContent(actualContent, bodyReader.CurrentEncoding, contentType.MediaType);
                }

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
}