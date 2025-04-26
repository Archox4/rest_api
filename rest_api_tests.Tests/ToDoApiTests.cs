
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;


namespace rest_api_tests.Tests
{
    public class ToDoApiTests : IClassFixture<WebApplicationFactory<Program>>
    {

        private readonly WebApplicationFactory<Program> _factory;

        public ToDoApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        /*
         * Check if returns
         */
        [Fact]
        public async Task getAllReturns()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/getAll");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

    }
}
