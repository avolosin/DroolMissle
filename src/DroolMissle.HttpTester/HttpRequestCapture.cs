using Newtonsoft.Json;
using System.Net;

namespace DroolMissle.HttpTester
{
    public class HttpRequestCapture
    {
        static HttpRequestCapture()
        {
            //HD: mitigration of https://github.com/advisories/GHSA-5crp-9r3c-p9vr without forcing clients to upgrade to 13.0.1
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings { MaxDepth = 128 };
        }

        public string Url { get; set; }
        public HttpMethod Method { get; set; }
        public System.Text.Encoding RequestEncoding { get; set; }
        public string RequestContentTypeHeader { get; set; }
        public string RequestBody { get; set; }

        public DateTime RequestStartTime { get; set; }
        public DateTime RequestEndTime { get; set; }
        public HttpStatusCode ResponseStatusCode { get; set; }
        public string ResponseContent { get; set; }
        public TimeSpan RequestDuration { get; set; }
        
        /// <summary>
        /// If the request resulted in an exception - this is the exception
        /// </summary>
        public Exception? ExecutionException { get; set; }

        

        public T As<T>()
        {
            
            return JsonConvert.DeserializeObject<T>(this.ResponseContent);
        }

        public System.Net.Http.HttpRequestMessage AsHttpRequestMessage()
        {
            var request = new HttpRequestMessage(this.Method, this.Url);
            if (this.ResponseContent != null)
            {
                request.Content = new StringContent(this.RequestBody,this.RequestEncoding,this.RequestContentTypeHeader);
            }

            return request;
        }
    }
}
