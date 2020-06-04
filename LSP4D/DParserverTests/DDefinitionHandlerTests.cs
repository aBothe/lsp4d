using System;
using System.Linq;
using DParserverTests.Util;
using Newtonsoft.Json;
using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace DParserverTests
{
    public class DDefinitionHandlerTests : LspTest
    {
        protected override void ConfigureClientCapabilities(ClientCapabilities clientCapabilities)
        {
            clientCapabilities.TextDocument.Definition =
                new Supports<DefinitionCapability>(true, new DefinitionCapability {LinkSupport = true});
        }

        [Test]
        public void CallGotoDeclarationHandler_ReturnsLocations()
        {
            var caret = OpenMainFile(@"module main;
class MyClass {}

My§Class instance;");

            var definitions = Client.TextDocument.Definition(Lsp4DUtil.DefaultMainFile, (int) caret.Line, (int) caret.Character).Result.ToList();

            Assert.AreEqual("[{\"IsLocation\":false,\"Location\":null,\"IsLocationLink\":true,\"LocationLink\":{" +
                            "\"OriginSelectionRange\":{\"Start\":{\"Line\":3,\"Character\":0},\"End\":{\"Line\":3,\"Character\":7}}," +
                            $"\"TargetUri\":\"{new Uri(Lsp4DUtil.DefaultMainFile).AbsoluteUri}\"," +
                            "\"TargetRange\":{\"Start\":{\"Line\":1,\"Character\":0},\"End\":{\"Line\":1,\"Character\":16}}," +
                            "\"TargetSelectionRange\":{\"Start\":{\"Line\":1,\"Character\":6},\"End\":{\"Line\":1,\"Character\":13}}" +
                            "}}]", JsonConvert.SerializeObject(definitions));
        }

        [Test]
        public void CallGotoDeclarationHandler_OnNewExpression_ReturnsConstructorMethodLocations()
        {
            var caret = OpenMainFile(@"module main;
class MyClass(T) {
    this() {}
    this(T a) {}
}
auto b = new MyCla§ss!int();");

            var definitions = Client.TextDocument
                .Definition(Lsp4DUtil.DefaultMainFile, (int) caret.Line, (int) caret.Character).Result.ToList();

            Assert.AreEqual("[{\"IsLocation\":false,\"Location\":null,\"IsLocationLink\":true,\"LocationLink\":{" +
                            "\"OriginSelectionRange\":{\"Start\":{\"Line\":5,\"Character\":9},\"End\":{\"Line\":5,\"Character\":26}}," +
                            $"\"TargetUri\":\"{new Uri(Lsp4DUtil.DefaultMainFile).AbsoluteUri}\"," +
                            "\"TargetRange\":{\"Start\":{\"Line\":2,\"Character\":4},\"End\":{\"Line\":2,\"Character\":13}}," +
                            "\"TargetSelectionRange\":{\"Start\":{\"Line\":2,\"Character\":4},\"End\":{\"Line\":2,\"Character\":10}}" +
                            "}}]", JsonConvert.SerializeObject(definitions));
        }
    }
}