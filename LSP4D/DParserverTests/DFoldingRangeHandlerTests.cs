using System;
using System.Linq;
using DParserverTests.Util;
using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace DParserverTests
{
    public class DFoldingRangeHandlerTests : LspTest
    {
        protected override void ConfigureClientCapabilities(ClientCapabilities clientCapabilities)
        {
            clientCapabilities.TextDocument.FoldingRange =
                new Supports<FoldingRangeCapability>(true, new FoldingRangeCapability {LineFoldingOnly = false});
        }
        
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
        public void GenerateFoldings_HandleSubsequentSingleLineComments()
        {
            Client.TextDocument.DidOpen(Lsp4DUtil.DefaultMainFile, Lsp4DUtil.DLANG, @"module main;

/// a
/// b
/// c

// d
// e
// f
");

            var foldingRanges = Client.TextDocument.FoldingRanges(Lsp4DUtil.DefaultMainFile).Result
                .ToList();
            foldingRanges.Sort((r1, r2) => r1.StartLine.CompareTo(r2.StartLine));

            WorkAndProgressTester.AssertResultEquality(foldingRanges, "DParserverTests.DFoldingRangeHandlerTests.SubsequentSingleLineComments-result.json");
        }
        
        [Test]
        public void GenerateFoldings_HandleRanges()
        {
            Client.TextDocument.DidOpen(Lsp4DUtil.DefaultMainFile, Lsp4DUtil.DLANG, @"module main;

// region asdf

void main() {}

//endregion
");

            var foldingRanges = Client.TextDocument.FoldingRanges(Lsp4DUtil.DefaultMainFile).Result
                .ToList();
            foldingRanges.Sort((r1, r2) => r1.StartLine.CompareTo(r2.StartLine));

            WorkAndProgressTester.AssertResultEquality(foldingRanges, "DParserverTests.DFoldingRangeHandlerTests.RegionComments-result.json");
        }
        
        
        [Test]
        public void GenerateFoldings_HandleMetaBlocks()
        {
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

            WorkAndProgressTester.AssertResultEquality(foldingRanges, "DParserverTests.DFoldingRangeHandlerTests.MetaBlocks-result.json");
        }

        [Test]
        public void GenerateFoldings_HandleImportBlocks()
        {
            Client.TextDocument.DidOpen(Lsp4DUtil.DefaultMainFile, Lsp4DUtil.DLANG, @"module main;//0
import a;//1
import b;import b2;

import c=d;
import e;
import f;

static if(true) {
    import g;
    import g.h.i;
    import k;

    void main(string[] args) {
        writeln(`asdf`);
    }
}
");
            var request = new FoldingRangeRequestParam {
                TextDocument = new TextDocumentItem {Uri = new Uri(Lsp4DUtil.DefaultMainFile)}
            }; 
            var foldings = Client.SendRequest<Container<FoldingRange>>(DocumentNames.FoldingRange, request).Result.ToList();
            foldings.Sort((r1, r2) => r1.StartLine.CompareTo(r2.StartLine));
            
            WorkAndProgressTester.AssertResultEquality(foldings, "DParserverTests.DFoldingRangeHandlerTests.ImportRanges-result.json");
        }
    }
}