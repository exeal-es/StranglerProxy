using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace NetProxyTests
{
    public class WebApplicationFactoryBase<TStartup> : WebApplicationFactory<TStartup>
           where TStartup : class
    {
        private readonly string DestinationURLAddress;

        private IWebHost host;

        public WebApplicationFactoryBase(string url)
        {
            this.DestinationURLAddress = url;

            this.ClientOptions.BaseAddress = new Uri(this.DestinationURLAddress);

            this.CreateServer(this.CreateWebHostBuilder());
        }

        protected override TestServer CreateServer(IWebHostBuilder builder)
        {
            host = builder.Build();

            host.Start();


            return new TestServer(this.CreateWebHostBuilder());
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            var builder = WebHost.CreateDefaultBuilder(Array.Empty<string>());

            builder
            .ConfigureAppConfiguration((builderContext, config) =>
            {
                var env = builderContext.HostingEnvironment;
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            });

            builder.UseStartup<TStartup>().UseUrls(this.DestinationURLAddress);

            return builder;
        }

        [ExcludeFromCodeCoverage]
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                host?.Dispose();
            }
        }
    }
}
