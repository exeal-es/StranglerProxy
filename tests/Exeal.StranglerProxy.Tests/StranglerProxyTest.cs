using Exeal.StranglerProxy.Tests.Factory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
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
        public async Task GetNotOverrittenController()
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
        public async Task PostNotOverrittenController()
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
        public async Task PostOverrittenController()
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
        public async Task PutNotOverrittenController()
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
        public async Task PutOverrittenController()
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
        public async Task DeleteNotOverrittenController()
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
        public async Task DeleteOverrittenController()
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
        public async Task GetAcceptLanguageHeader()
        {
            // Arrange
            var client = proxyApiFactory.CreateClient();
            client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("andalu"));
            // Act
            var response = await client.GetAsync("headers/Accept-Language");

            // Assert
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"destinationController\":true,\"header\":[\"andalu\"]}", body);
        }

        [Fact]
        public async Task GetAcceptHeader()
        {
            // Arrange
            var client = proxyApiFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("audio/basic"));
            // Act
            var response = await client.GetAsync("headers/Accept");

            // Assert
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"destinationController\":true,\"header\":[\"audio/basic\"]}", body);
        }

        [Theory]
        [MemberData(nameof(SpecialHeadersTestCases))]
        public async Task GetHeader(Action<HttpClient> configHeader, string endPoint, string expectedResponse)
        {
            // Arrange
            var client = proxyApiFactory.CreateClient();
            configHeader(client);

            // Act
            var response = await client.GetAsync(endPoint);

            // Assert
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal(expectedResponse, body);
        }

        public static IEnumerable<object[]> SpecialHeadersTestCases()
        {
            yield return new object[]
            {
                (Action<HttpClient>)((client) => client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("audio/basic"))),
                "headers/Accept",
                "{\"destinationController\":true,\"header\":[\"audio/basic\"]}"
            };

            yield return new object[]
            {
                (Action<HttpClient>)((client) => client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("andalu"))),
                "headers/Accept-Language",
                "{\"destinationController\":true,\"header\":[\"andalu\"]}"
            };

        }
    }
}