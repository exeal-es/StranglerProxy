using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace Exeal.StranglerProxy.Tests.Factory
{
    public class WebApplicationFactoryBase<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        private readonly string DestinationURLAddress;

        private IWebHost host;

        public WebApplicationFactoryBase(string url)
        {
            DestinationURLAddress = url;

            ClientOptions.BaseAddress = new Uri(DestinationURLAddress);

            CreateServer(CreateWebHostBuilder());
        }

        protected override TestServer CreateServer(IWebHostBuilder builder)
        {
            host = builder.Build();

            host.Start();


            return new TestServer(CreateWebHostBuilder());
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

            builder.UseStartup<TStartup>().UseUrls(DestinationURLAddress);

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