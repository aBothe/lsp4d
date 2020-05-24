using System;
using System.Linq;
using DParserverTests.Util;
using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace DParserverTests
{
    public class DCompletionHandlerTests : LspTest
    {
        protected override void ConfigureClientCapabilities(ClientCapabilities clientCapabilities)
        {
            clientCapabilities.TextDocument.Completion = new Supports<CompletionCapability>(true, new CompletionCapability
            {
                CompletionItem = new CompletionItemCapability
                {
                    CommitCharactersSupport = true,
                    DeprecatedSupport = true,
                    DocumentationFormat = new Container<MarkupKind>(MarkupKind.Markdown, MarkupKind.PlainText),
                    PreselectSupport = true,
                    SnippetSupport = true,
                    TagSupport = new CompletionItemTagSupportCapability
                    {
                        ValueSet = new Container<CompletionItemTag>(CompletionItemTag.Deprecated)
                    }
                },
                CompletionItemKind = new CompletionItemKindCapability
                {
                    ValueSet = new Container<CompletionItemKind>(Enum.GetValues(typeof(CompletionItemKind)).Cast<CompletionItemKind>())
                },
                ContextSupport = true
            });
        }

        [Test]
        public void TriggersCompletion_WithInitialDot_ReturnsCompletionItems()
        {
            Client.TextDocument.DidOpen(Lsp4DUtil.DefaultMainFile, Lsp4DUtil.DLANG, @"module main;
class MyClass {int propertyA;}
void foo(MyClass i) {
i.
}
");

            var workAndProgress = WorkAndProgressTester.Setup(Client);

            var completions = Client.SendRequest<CompletionList>("textDocument/completion", new CompletionParams
                {
                    Context = new CompletionContext
                    {
                        TriggerCharacter = ".",
                        TriggerKind = CompletionTriggerKind.TriggerCharacter
                    },
                    Position = new Position(3, 2),
                    PartialResultToken = WorkAndProgressTester.PartialResultToken,
                    TextDocument = new TextDocumentIdentifier(new Uri(Lsp4DUtil.DefaultMainFile)),
                    WorkDoneToken = WorkAndProgressTester.WorkDoneToken
                })
                .Result;
            
            workAndProgress.AssertProgressLogExpectations("DParserverTests.DCompletionHandlerTests.DotTrigger1");
            
            Assert.IsEmpty(completions);
        }
        
        [Test]
        public void TriggersCompletion_WithAlreadyBegunIdentifier_ReturnsCompletionItems()
        {
            Client.TextDocument.DidOpen(Lsp4DUtil.DefaultMainFile, Lsp4DUtil.DLANG, @"module main;
class MyClass {int propertyA;}
void foo(MyClass i) {
i.prop
}
");
            var workAndProgress = WorkAndProgressTester.Setup(Client);
            
            var completions = Client.SendRequest<CompletionList>("textDocument/completion", new CompletionParams
                {
                    Context = new CompletionContext
                    {
                        TriggerKind = CompletionTriggerKind.Invoked
                    },
                    Position = new Position(3, 6),
                    TextDocument = new TextDocumentIdentifier(new Uri(Lsp4DUtil.DefaultMainFile)),
                    PartialResultToken = WorkAndProgressTester.PartialResultToken,
                    WorkDoneToken = WorkAndProgressTester.WorkDoneToken
                })
                .Result;

            workAndProgress.AssertProgressLogExpectations("DParserverTests.DCompletionHandlerTests.PropnameTrigger");
            
            Assert.IsEmpty(completions);
            Assert.IsFalse(completions.IsIncomplete);
        }
    }
}