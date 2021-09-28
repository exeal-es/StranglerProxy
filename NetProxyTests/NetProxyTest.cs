using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace NetProxyTests
{
    public class NetProxyTest : IClassFixture<ProxyApiFactory>, IClassFixture<DestinationApiFactory>
    {
        private readonly ProxyApiFactory proxyApiFactory;

        private readonly DestinationApiFactory destinationApiFactory;

        public NetProxyTest(ProxyApiFactory proxyApiFactory, DestinationApiFactory destinationApiFactory)
        {
            this.proxyApiFactory = proxyApiFactory;

            this.destinationApiFactory = destinationApiFactory;
        }

        [Fact]
        public async Task GetOverwrittenControllerWithoutArgument()
        {
            // Arrange
            var client = this.proxyApiFactory.CreateClient();

            // Act
            var response = await client.GetAsync("test");

            // Assert
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"overrideController\":true}", body);
        }

        [Fact]
        public async Task GetOverwrittenControllerWithArgument()
        {
            // Arrange
            var client = this.proxyApiFactory.CreateClient();

            // Act
            var response = await client.GetAsync("test1/Hi");

            // Assert
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"overrideController\":true,\"argument\":\"Hi\"}", body);
        }

        [Fact]
        public async Task GetNotOverrittenController()
        {
            // Arrange
            var client = this.proxyApiFactory.CreateClient();

            // Act
            var response = await client.GetAsync("test2/Hi");

            // Assert
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"destinationController\":true,\"argument\":\"Hi\"}", body);
        }

        [Fact]
        public async Task PostNotOverrittenController()
        {
            // Arrange
            var client = this.proxyApiFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(new { Body = "Hi" }), System.Text.Encoding.UTF8, "application/json");

            // Act

            var response = await client.PostAsync("test3", content);

            // Assert
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"destinationController\":true,\"argument\":{\"Body\":\"Hi\"}}", body);
        }

        [Fact]
        public async Task PostOverrittenController()
        {
            // Arrange
            var client = this.proxyApiFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(new { Body = "Hi" }), System.Text.Encoding.UTF8, "application/json");

            // Act

            var response = await client.PostAsync("test4", content);

            // Assert
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"overrideController\":true,\"argument\":{\"Body\":\"Hi\"}}", body);
        }

        [Fact]
        public async Task PutNotOverrittenController()
        {
            // Arrange
            var client = this.proxyApiFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(new { Body = "Hi" }), System.Text.Encoding.UTF8, "application/json");

            // Act

            var response = await client.PutAsync("test5", content);

            // Assert
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"destinationController\":true,\"argument\":{\"Body\":\"Hi\"}}", body);
        }

        [Fact]
        public async Task PutOverrittenController()
        {
            // Arrange
            var client = this.proxyApiFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(new { Body = "Hi" }), System.Text.Encoding.UTF8, "application/json");

            // Act

            var response = await client.PutAsync("test6", content);

            // Assert
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"overrideController\":true,\"argument\":{\"Body\":\"Hi\"}}", body);
        }

        [Fact]
        public async Task DeleteNotOverrittenController()
        {
            // Arrange
            var client = this.proxyApiFactory.CreateClient();

            // Act

            var response = await client.DeleteAsync("test7/90");

            // Assert
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"destinationController\":true,\"argument\":\"90\"}", body);
        }

        [Fact]
        public async Task DeleteOverrittenController()
        {
            // Arrange
            var client = this.proxyApiFactory.CreateClient();

            // Act

            var response = await client.DeleteAsync("test8/90");

            // Assert
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"overrideController\":true,\"argument\":\"90\"}", body);
        }
    }
}