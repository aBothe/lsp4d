using System;
using DParserverTests.Util;
using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace DParserverTests
{
    public class DSelectionRangeHandlerTests : LspTest
    {
        protected override void ConfigureClientCapabilities(ClientCapabilities clientCapabilities)
        {
            clientCapabilities.TextDocument.SelectionRange =
                new Supports<SelectionRangeCapability>(true, new SelectionRangeCapability());
        }

        [Test]
        public void CheckWorkDoneProgressCap()
        {
            Assert.IsTrue(Client.ServerCapabilities.SelectionRangeProvider.Value.WorkDoneProgress);
        }

        [Test]
        public void RequestSelectionRange_ReturnsAstStackAsSelectionRangeStack()
        {
            var caret = OpenMainFile(@"module modA;
static if(true) {
    class AClass {
        void foo(T = bo§ol)() {
            return bar();
        }
    }
}
");

            var tester = WorkAndProgressTester.Setup(Client);

            var result = Client.SendRequest<Container<SelectionRange>>(DocumentNames.SelectionRange,
                new SelectionRangeParam
                {
                    TextDocument = new TextDocumentIdentifier(new Uri(Lsp4DUtil.DefaultMainFile)),
                    Positions = new Container<Position>(
                        new Position(4, 22),
                        caret),
                    PartialResultToken = WorkAndProgressTester.PartialResultToken,
                    WorkDoneToken = WorkAndProgressTester.WorkDoneToken
                }).Result;

            tester.AssertProgressLogExpectations("DParserverTests.DSelectionRangeHandlerTests1");
            Assert.IsEmpty(result);
        }
    }
}