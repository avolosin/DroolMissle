using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DroolMissle.HttpTester
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpRequestCapture> TestGetJsonAsync(this HttpClient client, string uri)
        {
            var capture = new HttpRequestCapture() { Url = uri, Method = HttpMethod.Get, RequestStartTime = DateTime.UtcNow };
            var sw = Stopwatch.StartNew();
            var response = await client.GetAsync(uri);
            sw.Stop();
            capture.RequestDuration = sw.Elapsed;
            capture.RequestEndTime = DateTime.UtcNow;
            capture.ResponseStatusCode = response.StatusCode;


            capture.ResponseContent = await response.Content.ReadAsStringAsync();


            return capture;
        }

        public static async Task<HttpRequestCapture> TestPostAsync<T>(this HttpClient client, string uri, string payload, Encoding encoding, string contentTypeHeader)
        {
            var capture = new HttpRequestCapture() { Url = uri, Method = HttpMethod.Post, RequestStartTime = DateTime.UtcNow };
            var sw = Stopwatch.StartNew();
            var response = await client.PostAsync(uri, new StringContent(payload, encoding, contentTypeHeader));
            sw.Stop();
            capture.RequestDuration = sw.Elapsed;
            capture.RequestBody = payload;
            capture.RequestEndTime = DateTime.UtcNow;
            capture.ResponseStatusCode = response.StatusCode;

            capture.ResponseContent = await response.Content.ReadAsStringAsync();
            return capture;
        }

        public static async Task<HttpRequestCapture> TestDeleteAsync<T>(this HttpClient client, string uri, string payload, Encoding encoding, string contentTypeHeader)
        {
            var capture = new HttpRequestCapture() { Url = uri, Method = HttpMethod.Delete, RequestStartTime = DateTime.UtcNow };
            var sw = Stopwatch.StartNew();
            var response = await client.PostAsync(uri, new StringContent(payload, encoding, contentTypeHeader));
            sw.Stop();
            capture.RequestDuration = sw.Elapsed;
            capture.RequestBody = payload;
            capture.RequestEndTime = DateTime.UtcNow;
            capture.ResponseStatusCode = response.StatusCode;

            capture.ResponseContent = await response.Content.ReadAsStringAsync();
            return capture;
        }

        public static async Task<HttpRequestCapture> TestPostCsvAsync(this HttpClient client, string uri, string csvContents, Encoding encoding, string contentTypeHeader)
        {
            var capture = new HttpRequestCapture() { Url = uri, Method = HttpMethod.Post, RequestStartTime = DateTime.UtcNow };

            var sw = Stopwatch.StartNew();
            var response = await client.PostAsync(uri, new StringContent(csvContents, encoding, contentTypeHeader));
            sw.Stop();
            capture.RequestDuration = sw.Elapsed;
            capture.RequestBody = csvContents;
            capture.RequestEndTime = DateTime.UtcNow;
            capture.ResponseStatusCode = response.StatusCode;

            capture.ResponseContent = await response.Content.ReadAsStringAsync();
            return capture;
        }

        public static async Task<HttpRequestCapture> TestPostJsonAsync<T>(this HttpClient client, string uri, T payload)
        {
            return await TestPostAsync<T>(client, uri, JsonConvert.SerializeObject(payload, Formatting.Indented), Encoding.UTF8, "application/json");
        }

        public static async Task<HttpRequestCapture> TestPutJsonAsync<T>(this HttpClient client, string uri, T payload)
        {
            var capture = new HttpRequestCapture() { Url = uri, Method = HttpMethod.Post, RequestStartTime = DateTime.UtcNow };

            var sw = Stopwatch.StartNew();
            var postJson = JsonConvert.SerializeObject(payload, Formatting.None);
            var response = await client.PutAsync(uri, new StringContent(postJson, Encoding.UTF8, "application/json"));
            sw.Stop();
            capture.RequestDuration = sw.Elapsed;
            capture.RequestBody = postJson;
            capture.RequestEndTime = DateTime.UtcNow;
            capture.ResponseStatusCode = response.StatusCode;


            capture.ResponseContent = await response.Content.ReadAsStringAsync();


            return capture;
        }
    }
}
