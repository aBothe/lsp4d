using System;
using DParserverTests.Util;
using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace DParserverTests
{
    public class DDocumentHighlightHandlerTests : LspTest
    {
        protected override void ConfigureClientCapabilities(ClientCapabilities clientCapabilities)
        {
            clientCapabilities.TextDocument.DocumentHighlight = new Supports<DocumentHighlightCapability>(true,
                new DocumentHighlightCapability { });
        }

        [Test]
        public void GetDocumentHighlights_ReturnsHighlights()
        {
            var caret = OpenMainFile(@"module modA;
class AClass {
    AClass foo(T = AClass)(T t);
}
void bar() {
    auto a = new ACla§ss();
}
");

            var workTester = WorkAndProgressTester.Setup(Client);

            Assert.IsEmpty(Client.SendRequest<DocumentHighlightContainer>("textDocument/documentHighlight",
                new DocumentHighlightParams
                {
                    Position = caret,
                    PartialResultToken = WorkAndProgressTester.PartialResultToken,
                    TextDocument = new TextDocumentIdentifier(new Uri(Lsp4DUtil.DefaultMainFile)),
                    WorkDoneToken = WorkAndProgressTester.WorkDoneToken
                }).Result);

            workTester.AssertProgressLogExpectations("DParserverTests.DDocumentHighlightHandlerTests1");
        }

        [Test]
        public void GetDocumentHighlights_OnNewExpression_ReturnsClassNamesHighlights()
        {
            var caret = OpenMainFile(@"module modA;
class AClass {
    this() {}
    this(int asdf) {}
}
void bar() {
    auto a = new ACla§ss();
}
");

            var results = Client.SendRequest<DocumentHighlightContainer>("textDocument/documentHighlight",
                new DocumentHighlightParams
                {
                    Position = caret,
                    TextDocument = new TextDocumentIdentifier(new Uri(Lsp4DUtil.DefaultMainFile))
                }).Result;

            WorkAndProgressTester.AssertResultEquality(results,
                "DParserverTests.DDocumentHighlightHandlerTests2-result.json");
        }
    }
}