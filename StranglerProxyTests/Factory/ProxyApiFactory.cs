using ProxyApi;

namespace StranglerProxyTests
{
    public class ProxyApiFactory : WebApplicationFactoryBase<Startup>
    {
        public ProxyApiFactory()
            : base("http://localhost:5000")
        {
        }
    }
}