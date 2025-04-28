
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
         * /getAll
         * 
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

        /*
         * /addTask
         * /deleteTask
         */
        [Fact]
        public async Task isAddTaskWorking()
        {
            // creating task
            var client = _factory.CreateClient();

            var task = new rest_api.Task
            {
                expiry = DateTime.UtcNow,
                title = "test task",
                description = "Task created by test",
                completePercentage = 0
            };


            var response = await client.PostAsJsonAsync("/addTask", task);
            // check if positive code
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            // delete created task
            var createdTask = await response.Content.ReadFromJsonAsync<rest_api.Task>();
            Assert.NotNull(createdTask);

            var cleanTask = await client.DeleteAsync($"/deleteTask/{createdTask.Id}");
            
            cleanTask.EnsureSuccessStatusCode();
        }

        /*
         *  /getIncoming
         */
        [Fact]
        public async Task isGetIncomingWorking()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/getIncoming/{0}");
            response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
