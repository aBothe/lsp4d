using System;
using System.IO;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace DParserverTests.Util
{
    public static class Lsp4DUtil
    {
        public static readonly string DLANG = "d";
        public static readonly string EscapedNewLine = Environment.NewLine.Replace("\r", "\\r").Replace("\n", "\\n");
        public static readonly string DefaultWorkspaceRoot = Path.Combine(Directory.GetCurrentDirectory(), "defaultworkspace");
        public static readonly string DefaultSrcFolder = Path.Combine(DefaultWorkspaceRoot, "src");
        public static readonly string DefaultMainFile = Path.Combine(DefaultSrcFolder, "main.d");
        
        public static LanguageClient MakeClient(Action<ClientCapabilities> configureClientCaps)
        {
            Directory.CreateDirectory(DefaultSrcFolder);

            var loggerFactory = new LoggerFactory(new[] {new InMemoryLoggerProvider()});

            var client = new LanguageClient(loggerFactory, new DummyServerProcess(loggerFactory));
            client.ClientCapabilities.Window = new WindowClientCapabilities
            {
                WorkDoneProgress = new Supports<bool>(true, true)
            };
            client.ClientCapabilities.Workspace.WorkspaceEdit = Supports.OfBoolean<WorkspaceEditCapability>(true);
            client.ClientCapabilities.Workspace.WorkspaceFolders = Supports.OfBoolean<bool>(true);
            client.ClientCapabilities.TextDocument.Completion = Supports.OfBoolean<CompletionCapability>(true);

            configureClientCaps(client.ClientCapabilities);
            
            bool hasInitialized = client.Initialize(DefaultWorkspaceRoot).Wait(3000);
            Assert.IsTrue(hasInitialized);
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