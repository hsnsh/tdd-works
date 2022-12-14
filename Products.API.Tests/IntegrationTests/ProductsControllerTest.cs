using System.Net;
using System.Text;
using Newtonsoft.Json;
using Products.API.Models;
using Products.API.Tests.Base;
using Xunit;

namespace Products.API.Tests.IntegrationTests
{
    public class ProductsControllerTest : IClassFixture<InMemoryWebApplicationFactory<Startup>>
    {
        private readonly InMemoryWebApplicationFactory<Startup> _factory;
        private readonly string _baseUrl = "/api/v1/products";

        public ProductsControllerTest(InMemoryWebApplicationFactory<Startup> factory)
        {
            this._factory = factory;
        }

        [Fact]
        public async Task web_api_basari_testi()
        {
            //Arrange
            var client = _factory.CreateClient();

            //Act
            var response = await client.GetAsync($"{_baseUrl}");

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task post_request_test()
        {
            var product = new Product { Name = "TestName", Price = 5, Stock = 1500 };

            var client = _factory.CreateClient();
            var httpContent = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_baseUrl}", httpContent);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);
        }

        [Fact]
        public async Task put_request_test()
        {
            var client = _factory.CreateClient();
            var request = new Product { Name = "X", Price = 100, Stock = 10 };
            var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_baseUrl}/3", httpContent);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task get_by_id_request_test()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync($"{_baseUrl}/3");
            var strinResult = await response.Content.ReadAsStringAsync();
            var jsonObject = JsonConvert.DeserializeObject<Product>(strinResult);
            Assert.Equal("750", jsonObject.Stock.ToString());
        }
    }
}