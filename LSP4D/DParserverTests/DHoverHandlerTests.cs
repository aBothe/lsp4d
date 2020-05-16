using DParserverTests.Util;
using Newtonsoft.Json;
using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace DParserverTests
{
    public class DHoverHandlerTests : LspTest
    {
        protected override void ConfigureClientCapabilities(ClientCapabilities clientCapabilities)
        {
            clientCapabilities.TextDocument.Hover = new Supports<HoverCapability>(true, new HoverCapability
            {
                ContentFormat = new Container<MarkupKind>(MarkupKind.Markdown, MarkupKind.PlainText)
            });
        }

        [Test]
        public void InvokesHover_ReturnsSignature()
        {
            Client.TextDocument.DidOpen(Lsp4DUtil.DefaultMainFile, Lsp4DUtil.DLANG, @"module main;
void main(string[] args);
");

            var hoverResult = Client.TextDocument.Hover(Lsp4DUtil.DefaultMainFile, 1, 20).Result;

            Assert.AreEqual(
                "{\"Contents\":{\"MarkedStrings\":null,\"HasMarkedStrings\":false,\"MarkupContent\":{\"Kind\":\"markdown\"," +
                "\"Value\":\"(parameter) `immutable(char)[][]` args\"},\"HasMarkupContent\":true}," +
                "\"Range\":{" +
                "\"Start\":{\"Line\":1,\"Character\":10}," +
                "\"End\":{\"Line\":1,\"Character\":23}" +
                "}}", JsonConvert.SerializeObject(hoverResult));
        }
    }
}