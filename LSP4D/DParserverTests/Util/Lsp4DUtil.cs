using System;
using System.IO;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace DParserverTests.Util
{
    public static class Lsp4DUtil
    {
        public static LanguageClient MakeClient()
        {
            var loggerFactory = new LoggerFactory(new[] {new InMemoryLoggerProvider()});
            
            var client = new LanguageClient(loggerFactory, new DummyServerProcess(loggerFactory));
            client.ClientCapabilities.Workspace.WorkspaceEdit = Supports.OfBoolean<WorkspaceEditCapability>(true);
            client.ClientCapabilities.Workspace.WorkspaceFolders = Supports.OfBoolean<bool>(true);
            client.ClientCapabilities.TextDocument.Completion = Supports.OfBoolean<CompletionCapability>(true);

            client.Initialize(Directory.GetCurrentDirectory()).Wait(10000);
            return client;
        }
    }
}