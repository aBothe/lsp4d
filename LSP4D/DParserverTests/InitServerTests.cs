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
                ContentChanges = new TextDocumentContentChangeEvent[]
                {
                    new TextDocumentContentChangeEvent
                    {
                        Range = new Range(new Position(2, 0), new Position(2, 0)),
                        Text = @"
void main(string[] args) {//3
    writeln(`asdf`);//4
}

/// singlelinecomment
"
                    }
                }
            });

            var foldingRanges = _client.TextDocument.FoldingRanges(Lsp4DUtil.DefaultMainFile).Result;
            
            Assert.AreEqual(2, foldingRanges.Count());
            /*
            var locationLinks = _client.TextDocument
                .Definition(Lsp4DUtil.DefaultMainFile, 3, 19, CancellationToken.None).Result.First();
            Assert.IsTrue(locationLinks.IsLocation);
            Assert.AreEqual(Lsp4DUtil.DefaultMainFile, locationLinks.Location.Uri.AbsolutePath);
            Assert.AreEqual(3, locationLinks.Location.Range.Start.Line);
            Assert.AreEqual(19, locationLinks.Location.Range.Start.Character);
            Assert.AreEqual(3, locationLinks.Location.Range.Start.Line);
            Assert.AreEqual(19 + 4, locationLinks.Location.Range.Start.Character);*/
        }
    }
}