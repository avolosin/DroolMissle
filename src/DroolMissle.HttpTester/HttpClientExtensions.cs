using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace DroolMissle.HttpTester
{
    public static class HttpClientExtensions
    {


        public static async Task<HttpRequestCapture> TestPostAsync(this HttpClient client, string uri, string payload, Encoding encoding, string contentTypeHeader)
        {
            var capture = new HttpRequestCapture() { Url = uri, Method = HttpMethod.Post, RequestStartTime = DateTime.UtcNow, RequestBody = payload, RequestContentTypeHeader = contentTypeHeader, RequestEncoding = encoding };
            var request = capture.AsHttpRequestMessage();
            return await SendHttpRequestAsync(client, request, capture);
        }

        public static async Task<HttpRequestCapture> TestPutAsync(this HttpClient client, string uri, string payload, Encoding encoding, string contentTypeHeader)
        {
            var capture = new HttpRequestCapture() { Url = uri, Method = HttpMethod.Put, RequestStartTime = DateTime.UtcNow, RequestBody = payload, RequestContentTypeHeader = contentTypeHeader, RequestEncoding = encoding };
            var request = capture.AsHttpRequestMessage();
            return await SendHttpRequestAsync(client, request, capture);
        }

        public static async Task<HttpRequestCapture> TestGetAsync(this HttpClient client, string uri)
        {
            var capture = new HttpRequestCapture() { Url = uri, Method = HttpMethod.Get, RequestStartTime = DateTime.UtcNow, RequestBody = null, RequestContentTypeHeader = null, RequestEncoding = null };
            var request = capture.AsHttpRequestMessage();
            return await SendHttpRequestAsync(client, request, capture);
        }
        public static async Task<HttpRequestCapture> TestDeleteAsync(this HttpClient client, string uri)
        {
            var capture = new HttpRequestCapture() { Url = uri, Method = HttpMethod.Delete, RequestStartTime = DateTime.UtcNow, RequestBody = null, RequestContentTypeHeader = null, RequestEncoding = null };
            var request = capture.AsHttpRequestMessage();
            return await SendHttpRequestAsync(client, request, capture);
        }

        public static async Task<HttpRequestCapture> SendHttpRequestAsync(this HttpClient client, HttpRequestMessage request, HttpRequestCapture capture, CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var response = await client.SendAsync(request, cancellationToken);
                sw.Stop(); //we'll do this here AND in the finally, in the event reading the response content in a successful situation takes a while (eg: a long stream)
                capture.ResponseStatusCode = response.StatusCode;
                capture.ResponseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            }
            catch (System.Exception ex)
            {
                capture.ExecutionException = ex;
            }
            finally
            {
                sw.Stop();
                capture.RequestDuration = sw.Elapsed;
                capture.RequestEndTime = DateTime.UtcNow;
            }

            return capture;
        }

        public static async Task<HttpRequestCapture> TestPostJsonAsync<T>(this HttpClient client, string uri, T payload)
        {
            return await TestPostAsync(client, uri, JsonConvert.SerializeObject(payload, Formatting.Indented), Encoding.UTF8, "application/json");
        }

        public static async Task<HttpRequestCapture> TestPutJsonAsync<T>(this HttpClient client, string uri, T payload)
        {
            return await TestPutAsync(client, uri, JsonConvert.SerializeObject(payload, Formatting.Indented), Encoding.UTF8, "application/json");
        }

        public static async Task<HttpRequestCapture> TestPostCsvAsync(this HttpClient client, string uri, string csvContents, Encoding encoding, string contentTypeHeader = "text/csv")
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
    }
}
