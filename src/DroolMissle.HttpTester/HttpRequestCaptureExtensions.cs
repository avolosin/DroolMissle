using System;
using Shouldly;
using System.Net;

namespace DroolMissle.HttpTester
{
    public static class HttpRequestCaptureExtensions
    {

        public static HttpRequestCapture ExpectHttpStatus(this HttpRequestCapture capture, HttpStatusCode expected)
        {
            capture.ResponseStatusCode.ShouldBe(expected,customMessage:$"{capture.Method:G} {capture.Url} Body: '{capture.RequestBody}'; Response Body: '{capture.ResponseContent}'");
            return capture;
        }

    }
}
