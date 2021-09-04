using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace DroolMissle
{
    public static class HttpClientExtensionsJson
    {
        public static string ClassNamePrefixRemoval = String.Empty;
        public static HttpRequestCapture ShouldJsonSnapshotCompare(this HttpRequestCapture capture, params TokenMatchCriteria[] matchCriteria)
        {
            return ShouldJsonSnapshotCompare(capture, null, matchCriteria);
        }

        public static HttpRequestCapture ShouldJsonSnapshotCompare(this HttpRequestCapture capture, string stepName, params TokenMatchCriteria[] matchCriteria)
        {
            var fileName = GetSafeTestName();
            if (!String.IsNullOrWhiteSpace(stepName))
            {
                fileName += $"__step_{stepName}";
            }

            fileName += ".json";

            var snapshotDirectory = GetSnapshotDirectory();

            if (!Directory.Exists(snapshotDirectory))
            {
                Directory.CreateDirectory(snapshotDirectory);
            }

            var snapshotFilePath = Path.Combine(snapshotDirectory, fileName);

            if (!File.Exists(snapshotFilePath))
            {
                File.WriteAllText(snapshotFilePath, PrettifyJson(capture.ResponseContent));

                Assert.Inconclusive($"Snapshot for {TestContext.CurrentContext.Test.MethodName} was created and should be reviewed before committing results");
            }

            var tokenMatchResults = JsonComparer.Compare(File.ReadAllText(snapshotFilePath), capture.ResponseContent, matchCriteria);
            foreach (var r in tokenMatchResults)
            {
                if (!r.IsMatch)
                {
                    r.ActualJsonValue.ShouldBe(r.ExpectedJsonValue, customMessage: r.Token);
                }
            }

            return capture;
        }


        static readonly IList<char> invalidFileNameChars = Path.GetInvalidFileNameChars();

        private static string GetSnapshotDirectory()
        {
            var projectDir = Directory.GetParent(TestContext.CurrentContext.TestDirectory).Parent.Parent.FullName;
            var classPath = TestContext.CurrentContext.Test.ClassName.Replace(ClassNamePrefixRemoval, string.Empty).Split('.');
            var methodName = TestContext.CurrentContext.Test.MethodName;

            var path = Path.Combine(projectDir, "__snapshots__");
            //append on the namespace part
            foreach (var cp in classPath)
            {
                path = Path.Combine(path, cp);
            }
            //append on the method name
            return Path.Combine(path, methodName);

        }
        private static string GetSafeTestName()
        {
            var methodName = TestContext.CurrentContext.Test.MethodName;
            var testName = TestContext.CurrentContext.Test.Name;

            if (methodName != testName)
            {
                testName = testName.Replace(methodName, string.Empty);
            }
            return new string(testName.Select(ch => invalidFileNameChars.Contains(ch) ? '_' : ch).ToArray());
        }

        private static string PrettifyJson(string json)
        {
            var parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }
    }
}
