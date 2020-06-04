using System;
using System.Collections.Generic;
using System.Linq;
using DParserverTests.Util;
using Newtonsoft.Json;
using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

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
        public void CheckServerCaps()
        {
            Assert.IsTrue(Client.ServerCapabilities.DefinitionProvider.Value.WorkDoneProgress);
        }

        [Test]
        public void CallGotoDeclarationHandler_ReturnsLocations()
        {
            var caret = OpenMainFile(@"module main;
class MyClass {}

My§Class instance;");

            var workAndProgress = WorkAndProgressTester.Setup(Client);

            Assert.IsEmpty(Client.SendRequest<List<LocationOrLocationLink>>(DocumentNames.Definition, new DefinitionParams
            {
                PartialResultToken = WorkAndProgressTester.PartialResultToken,
                Position = caret,
                TextDocument = new TextDocumentIdentifier(new Uri(Lsp4DUtil.DefaultMainFile)),
                WorkDoneToken = WorkAndProgressTester.WorkDoneToken
            }).Result);
            
            workAndProgress.AssertProgressLogExpectations("DParserverTests.DDefinitionHandlerTests1");
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