using DParserverTests.Util;
using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace DParserverTests
{
    public class InitServerTests : LspTest
    {
        [Test]
        public void StartsServer()
        {
            Assert.IsTrue(Client.ServerCapabilities.TextDocumentSync.Kind.HasFlag(TextDocumentSyncKind.Incremental));
        }
    }
}