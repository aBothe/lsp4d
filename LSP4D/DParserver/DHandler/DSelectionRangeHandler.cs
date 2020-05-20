using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using D_Parser.Completion;
using D_Parser.Dom;
using D_Parser.Dom.Expressions;
using D_Parser.Dom.Statements;
using D_Parserver.DHandler.Resolution;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace D_Parserver.DHandler
{
    public class DSelectionRangeHandler : SelectionRangeHandler
    {
        public DSelectionRangeHandler(ProgressManager progressManager)
            : base(
                new SelectionRangeRegistrationOptions
                    {DocumentSelector = TextDocumentHandler.DocumentSelector, WorkDoneProgress = true}, progressManager)
        {
            SetCapability(new SelectionRangeCapability());
        }

        public override Task<Container<SelectionRange>> Handle(SelectionRangeParam request,
            CancellationToken cancellationToken)
        {
            var workDone = ProgressManager.WorkDone(request,
                new WorkDoneProgressBegin {Message = "Begin getting selection ranges", Percentage = 0.0});
            var progress = ProgressManager.For(request, cancellationToken);
            var allResults = new List<SelectionRange>();

            int progressPosition = 0;
            foreach (var position in request.Positions)
            {
                var editorData = DResolverWrapper.CreateEditorData(new HoverParams
                {
                    Position = position,
                    TextDocument = request.TextDocument
                }, cancellationToken);
                allResults.Add(FindIdealSelectionRange(editorData));
                progress?.OnNext(new Container<SelectionRange>(allResults));
                workDone.OnNext(new WorkDoneProgressReport
                    {Percentage = ((double) ++progressPosition / request.Positions.Count()) * 100});
            }

            progress?.OnCompleted();
            workDone.OnCompleted();
            return Task.FromResult(progress != null
                ? new Container<SelectionRange>()
                : new Container<SelectionRange>(allResults));
        }

        private static SelectionRange FindIdealSelectionRange(IEditorData editor)
        {
            var vis = new IdealSelectionVisitor(editor.CaretLocation);

            editor.SyntaxTree.Accept(vis);

            return vis.narrowestSelectionRange;
        }

        class IdealSelectionVisitor : DefaultDepthFirstVisitor
        {
            private readonly CodeLocation caret;
            public SelectionRange narrowestSelectionRange;

            public IdealSelectionVisitor(CodeLocation caret)
            {
                this.caret = caret;
            }

            private bool PushSelectionRange(ISyntaxRegion syntaxRegion)
            {
                if (syntaxRegion is DModule)
                    return true;

                if (syntaxRegion.Location > caret || syntaxRegion.EndLocation < caret)
                    return false;

                if (narrowestSelectionRange == null)
                {
                    narrowestSelectionRange = new SelectionRange {Range = syntaxRegion.ToRange()};
                }
                else if (syntaxRegion.IsWithinRange(narrowestSelectionRange.Range))
                {
                    narrowestSelectionRange = new SelectionRange
                    {
                        Range = syntaxRegion.ToRange(),
                        Parent = narrowestSelectionRange
                    };
                }

                return true;
            }

            public override void VisitDNode(DNode n)
            {
                if (PushSelectionRange(n))
                {
                    base.VisitDNode(n);
                }
            }

            public override void VisitBlock(DBlockNode block)
            {
                // Check metablocks first
                if (block.MetaBlocks.Count != 0)
                    foreach (var mb in block.MetaBlocks)
                        mb.Accept(this);

                if (block.StaticStatements.Count != 0)
                    foreach (var s in block.StaticStatements)
                        s.Accept(this);

                VisitDNode(block); // visit declaration itself first
                VisitChildren(block);
            }

            public override void VisitAbstractStmt(AbstractStatement stmt)
            {
                if (PushSelectionRange(stmt))
                {
                    base.VisitAbstractStmt(stmt);
                }
            }

            public override void VisitChildren(StatementContainingStatement stmtContainer)
            {
                if (PushSelectionRange(stmtContainer))
                {
                    base.VisitChildren(stmtContainer);
                }
            }

            public override void VisitChildren(ContainerExpression x)
            {
                if (PushSelectionRange(x))
                {
                    base.VisitChildren(x);
                }
            }

            public override void VisitPostfixExpression(PostfixExpression x)
            {
                if (PushSelectionRange(x))
                {
                    base.VisitPostfixExpression(x);
                }
            }

            public override void VisitInner(ITypeDeclaration td)
            {
                if (PushSelectionRange(td))
                {
                    base.VisitInner(td);
                }
            }

            public override void VisitTemplateParameter(TemplateParameter tp)
            {
                if (PushSelectionRange(tp))
                {
                    base.VisitTemplateParameter(tp);
                }
            }

            public override void VisitIMetaBlock(IMetaDeclarationBlock block)
            {
                if (PushSelectionRange(block))
                {
                    base.VisitIMetaBlock(block);
                }
            }
        }
    }
}