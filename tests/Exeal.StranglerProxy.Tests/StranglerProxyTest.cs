using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Exeal.StranglerProxy.Tests.Factory;
using Newtonsoft.Json;
using Xunit;

namespace Exeal.StranglerProxy.Tests
{
    public class StranglerProxyTest : IClassFixture<ProxyApiFactory>, IClassFixture<DestinationApiFactory>
    {
        private readonly ProxyApiFactory proxyApiFactory;

        public StranglerProxyTest(ProxyApiFactory proxyApiFactory)
        {
            this.proxyApiFactory = proxyApiFactory;
        }

        [Fact]
        public async Task GetOverwrittenControllerWithoutArgument()
        {
            // Arrange
            var client = proxyApiFactory.CreateClient();

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
            var client = proxyApiFactory.CreateClient();

            // Act
            var response = await client.GetAsync("test1/Hi");

            // Assert
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"overrideController\":true,\"argument\":\"Hi\"}", body);
        }

        [Fact]
        public async Task GetNotOverwrittenController()
        {
            // Arrange
            var client = proxyApiFactory.CreateClient();

            // Act
            var response = await client.GetAsync("test2/Hi");

            // Assert
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"destinationController\":true,\"argument\":\"Hi\"}", body);
        }

        [Fact]
        public async Task PostNotOverwrittenController()
        {
            // Arrange
            var client = proxyApiFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(new { Body = "Hi" }), Encoding.UTF8,
                "application/json");

            // Act

            var response = await client.PostAsync("test3", content);

            // Assert
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"destinationController\":true,\"argument\":{\"Body\":\"Hi\"}}", body);
        }

        [Fact]
        public async Task PostOverwrittenController()
        {
            // Arrange
            var client = proxyApiFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(new { Body = "Hi" }), Encoding.UTF8,
                "application/json");

            // Act

            var response = await client.PostAsync("test4", content);

            // Assert
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"overrideController\":true,\"argument\":{\"Body\":\"Hi\"}}", body);
        }

        [Fact]
        public async Task PutNotOverwrittenController()
        {
            // Arrange
            var client = proxyApiFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(new { Body = "Hi" }), Encoding.UTF8,
                "application/json");

            // Act
            var response = await client.PutAsync("test5", content);

            // Assert
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"destinationController\":true,\"argument\":{\"Body\":\"Hi\"}}", body);
        }

        [Fact]
        public async Task PutOverwrittenController()
        {
            // Arrange
            var client = proxyApiFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(new { Body = "Hi" }), Encoding.UTF8,
                "application/json");

            // Act
            var response = await client.PutAsync("test6", content);

            // Assert
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"overrideController\":true,\"argument\":{\"Body\":\"Hi\"}}", body);
        }

        [Fact]
        public async Task DeleteNotOverwrittenController()
        {
            // Arrange
            var client = proxyApiFactory.CreateClient();

            // Act
            var response = await client.DeleteAsync("test7/90");

            // Assert
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"destinationController\":true,\"argument\":\"90\"}", body);
        }

        [Fact]
        public async Task DeleteOverwrittenController()
        {
            // Arrange
            var client = proxyApiFactory.CreateClient();

            // Act
            var response = await client.DeleteAsync("test8/90");

            // Assert
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"overrideController\":true,\"argument\":\"90\"}", body);
        }

        [Fact]
        public async Task GetAuthorizationHeader()
        {
            // Arrange
            var client = proxyApiFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Your Oauth token");

            // Act
            var response = await client.GetAsync("test9");

            // Assert
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"destinationController\":true,\"authorization\":[\"Bearer Your Oauth token\"]}", body);
        }

        [Fact]
        public async Task GetDestinationWithQueryString()
        {
            // Arrange
            var client = proxyApiFactory.CreateClient();

            // Act
            var response = await client.GetAsync("test10?aQueryString=12");

            // Assert
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"destinationController\":true,\"queryString\":\"?aQueryString=12\"}", body);
        }

        [Fact]
        public async Task GetDestinationCustomHeader()
        {
            // Arrange
            var client = proxyApiFactory.CreateClient();
            client.DefaultRequestHeaders.Add("CustomHeader", "20");

            // Act
            var response = await client.GetAsync("test11");

            // Assert
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"destinationController\":true,\"customHeader\":[\"20\"]}", body);
        }

        [Fact]
        public async Task CloneContentWhenEncodingIsNotSpecified()
        {
            // Arrange
            var client = proxyApiFactory.CreateClient();
            var contentWithoutEncoding = new ByteArrayContent(new byte[10]);
            contentWithoutEncoding.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            // Act
            var response = await client.PostAsync("test12", contentWithoutEncoding);

            // Assert
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"destinationController\":true,\"contentType\":\"application/json\"}", body);
        }

        [Theory]
        [InlineData("test13")]
        [InlineData("TeSt13/")]
        public async Task ExcludeOverwrittenResource(string path)
        {
            // Arrange
            var client = proxyApiFactory.CreateClient();

            // Act
            var response = await client.GetAsync(path);

            // Assert
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"destinationController\":true}", body);
        }

        [Fact]
        public async Task ExcludeOverwrittenResourceWithArgument()
        {
            // Arrange
            var client = proxyApiFactory.CreateClient();

            // Act
            var response = await client.GetAsync("test14/Hi");

            // Assert
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"destinationController\":true,\"argument\":\"Hi\"}", body);
        }
    }
}