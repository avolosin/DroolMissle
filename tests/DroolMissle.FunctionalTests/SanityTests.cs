using System.Net;
using DroolMissle.HttpTester;
using Shouldly;

namespace DroolMissle.FunctionalTests
{
    /// <summary>
    /// Basic sanity tests to make sure core functionality is working. Over time these will be less required, but while we still have few tests, these will give us some sanity.
    /// </summary>
    public class SanityTests
    {
        
        /// <summary>
        /// Asserts that HTTP Get returns JSON and the JSON
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Test_Json_Get()
        {
            var http = new HttpClient();
            var result = await http.TestGetAsync("https://jsonplaceholder.typicode.com/todos/1");


            result.ResponseStatusCode.ShouldBe(HttpStatusCode.OK);
            result.RequestDuration.ShouldBeLessThan(TimeSpan.FromSeconds(30));

            var expectedJson = "{\r\n  \"userId\": 1,\r\n  \"id\": 1,\r\n  \"title\": \"delectus aut autem\",\r\n  \"completed\": false\r\n}";
            var compareResult = JsonCompare.JsonComparer.Compare(result.ResponseContent, expectedJson);

            compareResult.Count(c=>c.IsMatch==false).ShouldBe(0);
        }

        internal class JsonPlaceHolderToDo
        {
            public int UserId { get; set; }
            public int Id { get; set; }
            public string Title { get; set; }
            public bool Completed { get; set; }
            
        }
    }
}