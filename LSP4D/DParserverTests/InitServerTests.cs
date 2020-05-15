using System;
using System.Linq;
using DParserverTests.Util;
using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace DParserverTests
{
    public class InitServerTests : LspTest
    {
        [Test]
        public void InitalizeClientAndServer_initializesServerCaps()
        {
            Assert.IsNotNull(Client.ServerCapabilities.FoldingRangeProvider);
        }
        
        [Test]
        public void TextDocumentChanges_appliesIncrementalChanges()
        {
            Client.TextDocument.DidOpen(Lsp4DUtil.DefaultMainFile, Lsp4DUtil.DLANG, @"module main;//0
import stdio;//1
//2", 1);

            Client.SendNotification(DocumentNames.DidChange, new DidChangeTextDocumentParams
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

            var foldingRanges = Client.TextDocument.FoldingRanges(Lsp4DUtil.DefaultMainFile).Result.ToList();

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
        
        [Test]
        public void GenerateFoldings_HandleMetaBlocks()
        {
            Assert.IsNotNull(Client.ServerCapabilities.FoldingRangeProvider);
            
            Client.TextDocument.DidOpen(Lsp4DUtil.DefaultMainFile, Lsp4DUtil.DLANG, @"module main;//0
import stdio;//1

static if(true) {
//4
void main(string[] args) {
    writeln(`asdf`);//6
 }
}
//9
");

            var foldingRanges = Client.TextDocument.FoldingRanges(Lsp4DUtil.DefaultMainFile).Result
                .ToList();
            foldingRanges.Sort((r1, r2) => r1.StartLine.CompareTo(r2.StartLine));

            Assert.AreEqual(2, foldingRanges.Count);

            Assert.AreEqual(FoldingRangeKind.Region, foldingRanges[0].Kind);
            Assert.AreEqual(3, foldingRanges[0].StartLine);
            Assert.AreEqual(16, foldingRanges[0].StartCharacter);
            Assert.AreEqual(8, foldingRanges[0].EndLine);
            Assert.AreEqual(1, foldingRanges[0].EndCharacter);

            Assert.AreEqual(FoldingRangeKind.Region, foldingRanges[1].Kind);
            Assert.AreEqual(5, foldingRanges[1].StartLine);
            Assert.AreEqual(25, foldingRanges[1].StartCharacter);
            Assert.AreEqual(7, foldingRanges[1].EndLine);
            Assert.AreEqual(2, foldingRanges[1].EndCharacter);
        }
    }
}