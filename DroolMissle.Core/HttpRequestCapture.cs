using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace DroolMissle
{
    public class HttpRequestCapture
    {
        public string Url { get; set; }
        public HttpMethod Method { get; set; }
        public string RequestBody { get; set; }

        public DateTime RequestStartTime { get; set; }
        public DateTime RequestEndTime { get; set; }
        public HttpStatusCode ResponseStatusCode { get; set; }
        public string ResponseContent { get; set; }
        public TimeSpan RequestDuration { get; set; }

        public T As<T>()
        {
            return JsonConvert.DeserializeObject<T>(this.ResponseContent);
        }
    }
}
