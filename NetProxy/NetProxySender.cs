using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using Newtonsoft.Json;

namespace NetFwkProxy
{
    public class NetProxySender : INetProxySender
    {
        private readonly HttpActionContext context;

        private readonly HttpClient httpClient;

        public NetProxySender(HttpActionContext context, Uri destination)
        {
            this.context = context;

            this.httpClient = new HttpClient(new HttpClientHandler()
            {
                AllowAutoRedirect = false,
            });

            this.httpClient.BaseAddress = destination;
        }

        public async Task<T> Send<T>()
        {
            var request = new HttpRequestMessage(this.context.Request.Method, this.context.Request.RequestUri.AbsolutePath);

            var response = await this.httpClient.SendAsync(request);

            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(result);
        }
    }
}
