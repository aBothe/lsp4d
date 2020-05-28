using System;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace DParserverTests.Util
{
    public class WorkAndProgressTester
    {
        private readonly StringBuilder _recordedResultProgress = new StringBuilder("[");
        private readonly StringBuilder _recordedWorkProgress = new StringBuilder("[");
        private static readonly string SrcFolderUri = new Uri(Lsp4DUtil.DefaultSrcFolder).AbsoluteUri;
        public static readonly ProgressToken PartialResultToken = new ProgressToken("partialResult");
        public static readonly ProgressToken WorkDoneToken = new ProgressToken("workDone");

        private WorkAndProgressTester() {}
        
        public static WorkAndProgressTester Setup(LanguageClient client)
        {
            var tester = new WorkAndProgressTester();
            
            client.HandleNotification("$/progress", (JObject progressParams) =>
            {
                var jsonAgain = progressParams.ToString()
                    .Replace(SrcFolderUri, "SRC")
                    .Replace(Lsp4DUtil.DefaultSrcFolder, "ABSSRC");

                switch (progressParams["token"].ToString())
                {
                    case "partialResult":
                        tester._recordedResultProgress.Append(jsonAgain).Append(',').AppendLine();
                        break;
                    case "workDone":
                        tester._recordedWorkProgress.Append(jsonAgain).Append(',').AppendLine();
                        break;
                    default:
                        Assert.Fail("unexpected progress notification token " + progressParams["token"]);
                        return;
                }
            });
            return tester;
        }

        public void AssertProgressLogExpectations(string resourceBaseName)
        {
            _recordedResultProgress.Append(']');
            _recordedWorkProgress.Append(']');

            AssertJsonEquality(_recordedWorkProgress.ToString(), resourceBaseName + "-work.json");
            AssertJsonEquality(_recordedResultProgress.ToString(), resourceBaseName + "-result.json");
        }

        public static void AssertResultEquality(object content, string jsonFileInResources)
        {
            var jsonContent = JsonConvert.SerializeObject(content, Formatting.Indented)
                .Replace(SrcFolderUri, "SRC")
                .Replace(Lsp4DUtil.DefaultSrcFolder, "ABSSRC");
            AssertJsonEquality(jsonContent, jsonFileInResources);
        }

        static void AssertJsonEquality(string content, string jsonFileInResources)
        {
            Assert.AreEqual(GetEmbeddedResource(jsonFileInResources), content, "File " + jsonFileInResources);
        }

        static string GetEmbeddedResource(string res)
        {
            using var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(res));
            return reader.ReadToEnd();
        }
    }
}