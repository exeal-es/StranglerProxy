using System;
using System.Web.Http.Controllers;

namespace NetFwkProxy
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
