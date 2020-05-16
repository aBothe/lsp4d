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
/**
 * Some arguments
 * Returns: Dummy!
 */
void main(string[] args) {
}
");

            var hoverResult = Client.TextDocument.Hover(Lsp4DUtil.DefaultMainFile, 5, 8).Result;

            Assert.AreEqual(
                "{\"Contents\":{\"MarkedStrings\":null,\"HasMarkedStrings\":false,\"MarkupContent\":{\"Kind\":\"markdown\"," +
                "\"Value\":\"`void` main.main(" + Lsp4DUtil.EscapedNewLine + 
                "  `string[]` args" + Lsp4DUtil.EscapedNewLine + 
                ")" + Lsp4DUtil.EscapedNewLine + 
                Lsp4DUtil.EscapedNewLine + 
                "Some arguments" + Lsp4DUtil.EscapedNewLine + 
                Lsp4DUtil.EscapedNewLine + 
                "**Returns:** Dummy!" + Lsp4DUtil.EscapedNewLine + 
                "\"},\"HasMarkupContent\":true}," +
                "\"Range\":{" +
                "\"Start\":{\"Line\":5,\"Character\":0}," +
                "\"End\":{\"Line\":6,\"Character\":1}" +
                "}}", JsonConvert.SerializeObject(hoverResult));
        }
    }
}