using System;
using System.Web.Http.Controllers;

namespace NetProxy
{
    public class NetProxy : INetProxy
    {
        private readonly Uri destination;

        public NetProxy(Uri destination)
        {
            this.destination = destination;
        }

        public INetProxySender Fordward(HttpActionContext context)
        {
            return new NetProxySender(context, destination);
        }
    }
}
