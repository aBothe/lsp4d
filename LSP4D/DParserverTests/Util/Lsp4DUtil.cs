using System.IO;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace DParserverTests.Util
{
    public static class Lsp4DUtil
    {
        public static readonly string DLANG = "d";
        public static readonly string DefaultWorkspaceRoot = Path.Combine(Directory.GetCurrentDirectory(), "defaultworkspace");
        public static readonly string DefaultSrcFolder = Path.Combine(DefaultWorkspaceRoot, "src");
        public static readonly string DefaultMainFile = Path.Combine(DefaultSrcFolder, "main.d");
        
        public static LanguageClient MakeClient()
        {
            Directory.CreateDirectory(DefaultSrcFolder);

            var loggerFactory = new LoggerFactory(new[] {new InMemoryLoggerProvider()});

            var client = new LanguageClient(loggerFactory, new DummyServerProcess(loggerFactory));
            client.ClientCapabilities.Workspace.WorkspaceEdit = Supports.OfBoolean<WorkspaceEditCapability>(true);
            client.ClientCapabilities.Workspace.WorkspaceFolders = Supports.OfBoolean<bool>(true);
            client.ClientCapabilities.TextDocument.Completion = Supports.OfBoolean<CompletionCapability>(true);
            client.ClientCapabilities.TextDocument.Hover = new Supports<HoverCapability>(true, new HoverCapability{ ContentFormat = new Container<MarkupKind>(MarkupKind.Markdown, MarkupKind.PlainText)});

            Assert.IsTrue(client.Initialize(DefaultWorkspaceRoot).Wait(5000));
            return client;
        }

        public static void CleanDefaultWorkspace()
        {
            Directory.Delete(DefaultWorkspaceRoot, true);
        }

        public static void WriteMainFile(string mainFileCode)
        {
            File.WriteAllText(DefaultMainFile, mainFileCode);
        }
    }
}