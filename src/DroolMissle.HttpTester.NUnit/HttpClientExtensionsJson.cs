using System;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Text.RegularExpressions;
using DroolMissle.HttpTester;


namespace DroolMissle.HttpTester
{
    public static class HttpClientExtensionsJson
    {
        public static List<string> ClassNamePrefixRemoval = new List<string>();
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

                Assert.Inconclusive($"Snapshot for method={TestContext.CurrentContext.Test.MethodName} was created at={snapshotFilePath} and should be reviewed before committing results");
            }

            var tokenMatchResults = JsonComparer.Compare(File.ReadAllText(snapshotFilePath), capture.ResponseContent, matchCriteria);

            //find the stuff that should have matched based on TokenMatchCriteria being applied
            var failedTokenMatches = tokenMatchResults.Where(r => r.IsMatch == false && r.IsTokenMatchApplied).ToList();
            var ftmActions = failedTokenMatches.Select(r => new Action(() =>
            {
                var customMessage = $"token={r.Token};";
                if (!String.IsNullOrWhiteSpace(stepName))
                {
                    customMessage += $" step={stepName};";
                }
                if (!string.IsNullOrWhiteSpace(r.MatchDescription))
                {
                    customMessage += $" description={r.MatchDescription};";
                }

                customMessage += $" file={fileName}";

                r.ActualJsonValue.ShouldBe(r.ExpectedJsonValue, customMessage: customMessage);
            })).ToArray();

            failedTokenMatches.ShouldSatisfyAllConditions(ftmActions);

            //find the stuff that should have matched based on _exact_ values
            var failedExactMatches = tokenMatchResults.Where(r => r.IsMatch == false && !r.IsTokenMatchApplied).ToList();
            var femActions = failedExactMatches.Select(r => new Action(() =>
            {
                var customMessage = $"token={r.Token};";
                if (!String.IsNullOrWhiteSpace(stepName))
                {
                    customMessage += $" step={stepName};";
                }

                if (!string.IsNullOrWhiteSpace(r.MatchDescription))
                {
                    customMessage += $" description={r.MatchDescription}";
                }

                r.ActualJsonValue.ShouldBe(r.ExpectedJsonValue, customMessage);
            })).ToArray();

            failedExactMatches.ShouldSatisfyAllConditions(femActions);

            return capture;
        }
        private static string GetSnapshotDirectory()
        {
            var projectDir = Directory.GetParent(TestContext.CurrentContext.TestDirectory).Parent.Parent.FullName;
            var className = TestContext.CurrentContext.Test.ClassName;
            foreach (var prefix in ClassNamePrefixRemoval)
            {
                className = className.Replace(prefix, string.Empty);
            }
            var classPath = className.Split('.');
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


        private static Regex _safeTestNameRegex = new Regex(@"[^a-zA-Z0-9 -]", System.Text.RegularExpressions.RegexOptions.Compiled);
        /// <summary>
        /// Generates test name that is safe for the purposes of a file name. Can't really use Path.GetInvalidFileNameChars because you might generate
        /// the files on Windows but run the tests on Linux which has a different set of allowable characters
        /// </summary>
        /// <returns></returns>
        private static string GetSafeTestName()
        {
            var methodName = TestContext.CurrentContext.Test.MethodName;
            var testName = TestContext.CurrentContext.Test.Name;

            if (methodName != testName)
            {
                testName = testName.Replace(methodName, string.Empty);
            }

            testName = _safeTestNameRegex.Replace(testName, "_");

            //trim any leading or trailing underscores
            return testName.TrimStart('_').TrimEnd('_');
        }

        private static string PrettifyJson(string json)
        {
            var parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Newtonsoft.Json.Formatting.Indented);
        }
    }
}
