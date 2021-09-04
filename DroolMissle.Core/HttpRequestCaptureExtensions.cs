using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace DroolMissle
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
