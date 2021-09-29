using DestinationApi;
using Exeal.StranglerProxy.Tests.DestinationApi;

namespace Exeal.StranglerProxy.Tests.Factory
{
    public class DestinationApiFactory : WebApplicationFactoryBase<Startup>
    {
        public DestinationApiFactory()
            : base("http://localhost:5001") // This url is configured in app setting file at ProxyApi config.
        {
        }
    }
}