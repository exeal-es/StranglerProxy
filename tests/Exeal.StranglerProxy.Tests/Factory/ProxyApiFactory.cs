using Exeal.StranglerProxy.Tests.ProxyApi;
using ProxyApi;

namespace Exeal.StranglerProxy.Tests.Factory
{
    public class ProxyApiFactory : WebApplicationFactoryBase<Startup>
    {
        public ProxyApiFactory()
            : base("http://localhost:5000")
        {
        }
    }
}