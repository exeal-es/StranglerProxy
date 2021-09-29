using DestinationApi;

namespace StranglerProxyTests
{
    public class DestinationApiFactory : WebApplicationFactoryBase<Startup>
    {
        public DestinationApiFactory()
            : base("http://localhost:5001") // This url is configured in app setting file at ProxyApi config.
        {
        }
    }
}