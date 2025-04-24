
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;


namespace rest_api_tests.Tests
{
    public class ToDoApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ToDoApiTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        /*
         * 
         * todo
         * 
         */

    }
}
