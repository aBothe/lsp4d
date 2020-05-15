using System;
using System.Linq;
using DParserverTests.Util;
using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace DParserverTests
{
    public class InitServerTests
    {
        private LanguageClient _client;
        
        [SetUp]
        public void Setup()
        {
            TearDown();
            _client = Lsp4DUtil.MakeClient();
        }

        [TearDown]
        public void TearDown()
        {
            if (_client != null)
            {
                Assert.IsTrue(_client.Shutdown().Wait(5000));
            }
            _client = null;
            //Lsp4DUtil.CleanDefaultWorkspace();
        }

        [Test]
        public void InitalizeClientAndServer_initializesEverything()
        {
            Assert.Pass();
        }
        
        [Test]
        public void TextDocumentChanges_appliesEditChanges()
        {
            Assert.IsNotNull(_client.ServerCapabilities.FoldingRangeProvider);
            
            _client.TextDocument.DidOpen(Lsp4DUtil.DefaultMainFile, Lsp4DUtil.DLANG, @"module main;//0
import stdio;//1
//2", 1);

            _client.SendNotification(DocumentNames.DidChange, new DidChangeTextDocumentParams
            {
                TextDocument = new VersionedTextDocumentIdentifier
                {
                    Version = 2,
                    Uri = new Uri(Lsp4DUtil.DefaultMainFile)
                },
                ContentChanges = new[]
                {
                    new TextDocumentContentChangeEvent
                    {
                        Range = new Range(new Position(2, 3), new Position(2, 3)),
                        Text = @"
void main(string[] args) {//3
    writeln(`asdf`);//4
}//5
/* */
/// singlelinecomment
"
                    }
                }
            });

            var foldingRanges = _client.TextDocument.FoldingRanges(Lsp4DUtil.DefaultMainFile).Result.ToList();

            Assert.AreEqual(2, foldingRanges.Count);

            Assert.AreEqual(FoldingRangeKind.Region, foldingRanges[0].Kind);
            Assert.AreEqual(3, foldingRanges[0].StartLine);
            Assert.AreEqual(25, foldingRanges[0].StartCharacter);
            Assert.AreEqual(5, foldingRanges[0].EndLine);
            Assert.AreEqual(1, foldingRanges[0].EndCharacter);

            Assert.AreEqual(FoldingRangeKind.Comment, foldingRanges[1].Kind);
            Assert.AreEqual(6, foldingRanges[1].StartLine);
            Assert.AreEqual(0, foldingRanges[1].StartCharacter);
            Assert.AreEqual(6, foldingRanges[1].EndLine);
            Assert.AreEqual(5, foldingRanges[1].EndCharacter);
        }
    }
}