using System;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Client;

namespace DParserverTests.Util
{
    public class WorkAndProgressTester
    {
        private readonly StringBuilder _recordedResultProgress = new StringBuilder("[");
        private readonly StringBuilder _recordedWorkProgress = new StringBuilder("[");
        private static readonly string SrcFolderUri = new Uri(Lsp4DUtil.DefaultSrcFolder).AbsoluteUri;

        public void Setup(LanguageClient client)
        {
            
            client.HandleNotification("$/progress", (JObject progressParams) =>
            {
                var jsonAgain = progressParams.ToString(Formatting.None)
                    .Replace(SrcFolderUri, "SRC")
                    .Replace(Lsp4DUtil.DefaultSrcFolder, "ABSSRC");

                switch (progressParams["token"].ToString())
                {
                    case "partialResult":
                        _recordedResultProgress.Append(jsonAgain).Append(',').AppendLine();
                        break;
                    case "workDone":
                        _recordedWorkProgress.Append(jsonAgain).Append(',').AppendLine();
                        break;
                    default:
                        Assert.Fail("unexpected progress notification token " + progressParams["token"]);
                        return;
                }
            });
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
            var jsonContent = JsonConvert.SerializeObject(content, Formatting.None)
                .Replace(SrcFolderUri, "SRC")
                .Replace(Lsp4DUtil.DefaultSrcFolder, "ABSSRC");
            AssertJsonEquality(jsonContent, "DParserverTests.DReferencesHandlerTests.ExpectedProgress3-result.json");
        }

        static void AssertJsonEquality(string content, string jsonFileInResources)
        {
            Assert.AreEqual(GetEmbeddedResource(jsonFileInResources), content);
        }

        static string GetEmbeddedResource(string res)
        {
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(res)))
            {
                return reader.ReadToEnd();
            }
        }
    }
}