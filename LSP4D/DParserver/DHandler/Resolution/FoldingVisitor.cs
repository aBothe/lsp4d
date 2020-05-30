using System.Collections.Generic;
using D_Parser.Dom;
using D_Parser.Dom.Statements;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace D_Parserver.DHandler.Resolution
{
    public class FoldingVisitor : DefaultDepthFirstVisitor
    {
        private readonly List<FoldingRange> _resultRanges;

        private FoldingVisitor(List<FoldingRange> resultRanges)
        {
            _resultRanges = resultRanges;
        }

        public static List<FoldingRange> GenerateFoldsInternal(IBlockNode block)
        {
            var l = new List<FoldingRange>();
            block?.Accept(new FoldingVisitor(l));
            return l;
        }

        public override void Visit(BlockStatement s)
        {
            _resultRanges.Add(new FoldingRange
            {
                StartLine = s.Location.Line - 1,
                StartCharacter = s.Location.Column - 1,
                EndLine = s.EndLocation.Line - 1,
                EndCharacter = s.EndLocation.Column - 1,
                Kind = FoldingRangeKind.Region
            });

            base.Visit(s);
        }

        public override void Visit(D_Parser.Dom.Expressions.StructInitializer x)
        {
            _resultRanges.Add(new FoldingRange
            {
                StartLine = x.Location.Line - 1,
                StartCharacter = x.Location.Column - 1,
                EndLine = x.EndLocation.Line - 1,
                EndCharacter = x.EndLocation.Column - 1,
                Kind = FoldingRangeKind.Region
            });

            base.Visit(x);
        }

        public override void VisitBlock(DBlockNode n)
        {
            if (!(n is DModule))
            {
                _resultRanges.Add(new FoldingRange
                {
                    StartLine = n.BlockStartLocation.Line - 1,
                    StartCharacter = n.BlockStartLocation.Column - 1,
                    EndLine = n.EndLocation.Line - 1,
                    EndCharacter = n.EndLocation.Column - 1,
                    Kind = FoldingRangeKind.Region
                });
            }

            HandleImportRanges(n.StaticStatements);

            base.VisitBlock(n);
        }

        private void HandleImportRanges(List<StaticStatement> statements)
        {
            for (var stmtIndex = 0; stmtIndex < statements.Count; stmtIndex++)
            {
                var importStmt = statements[stmtIndex] as ImportStatement;
                if (importStmt == null)
                    continue;

                ImportStatement endStmt = importStmt;

                // Search subsequent import
                for (var nextStmtIndex = stmtIndex + 1; nextStmtIndex < statements.Count; nextStmtIndex++)
                {
                    var nextImportStmt = statements[nextStmtIndex] as ImportStatement;
                    if (nextImportStmt == null || nextImportStmt.Location.Line > endStmt.EndLocation.Line + 1)
                        break;

                    endStmt = nextImportStmt;
                    stmtIndex++;
                }

                if (importStmt != endStmt)
                {
                    _resultRanges.Add(new FoldingRange
                    {
                        StartLine = importStmt.Location.Line - 1,
                        StartCharacter = importStmt.Location.Column - 1,
                        EndLine = endStmt.EndLocation.Line - 1,
                        EndCharacter = endStmt.EndLocation.Column - 1,
                        Kind = FoldingRangeKind.Imports
                    });
                }
            }
        }

        public override void VisitIMetaBlock(IMetaDeclarationBlock mdb)
        {
            _resultRanges.Add(new FoldingRange
            {
                StartLine = mdb.BlockStartLocation.Line - 1,
                StartCharacter = mdb.BlockStartLocation.Column - 1,
                EndLine = mdb.EndLocation.Line - 1,
                EndCharacter = mdb.EndLocation.Column - 1,
                Kind = FoldingRangeKind.Region
            });
        }
    }
}