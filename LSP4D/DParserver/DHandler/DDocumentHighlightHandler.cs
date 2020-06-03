using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using D_Parser.Dom;
using D_Parser.Refactoring;
using D_Parser.Resolver;
using D_Parser.Resolver.ExpressionSemantics;
using D_Parserver.DHandler.Resolution;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace D_Parserver.DHandler
{
    public class DDocumentHighlightHandler : DocumentHighlightHandler
    {
        public DDocumentHighlightHandler(ProgressManager progressManager)
            : base(new DocumentHighlightRegistrationOptions {DocumentSelector = TextDocumentHandler.DocumentSelector},
                progressManager)
        {
            SetCapability(new DocumentHighlightCapability());
        }

        static DNode GetResultMember(AbstractType abstractType)
        {
            var nodeSymbolToFind = ExpressionTypeEvaluation.GetResultMember(abstractType);

            // Slightly hacky: To keep highlighting the id of e.g. a NewExpression, take the ctor's parent node (i.e. the class node)
            if (nodeSymbolToFind is DMethod dm && dm.SpecialType == DMethod.MethodType.Constructor)
                nodeSymbolToFind = ((abstractType as DSymbol).Base as DSymbol)?.Definition;

            return nodeSymbolToFind;
        }

        public override Task<DocumentHighlightContainer> Handle(DocumentHighlightParams request,
            CancellationToken cancellationToken)
        {
            var workDone = ProgressManager.WorkDone(request, new WorkDoneProgressBegin
            {
                Title = "Begin finding highlights",
                Percentage = 0
            });

            var editorData = DResolverWrapper.CreateEditorData(request, cancellationToken);
            var nodeSymbolToFind = DResolverWrapper
                .ResolveHoveredCodeLoosely(editorData, out LooseResolution.NodeResolutionAttempt _,
                    out ISyntaxRegion _)
                .Select(GetResultMember)
                .FirstOrDefault();

            if (nodeSymbolToFind == null)
            {
                workDone.OnNext(new WorkDoneProgressReport()
                {
                    Message = "No symbol found",
                    Percentage = 100
                });
                workDone.OnCompleted();
                return Task.FromResult(new DocumentHighlightContainer());
            }

            var progress = ProgressManager.For(request, cancellationToken);
            List<DocumentHighlight> allFoundReferences;
            var ctxt = ResolutionContext.Create(editorData, true);

            try
            {
                allFoundReferences = cancellationToken.IsCancellationRequested
                    ? new List<DocumentHighlight>()
                    : ReferencesFinder
                        .SearchModuleForASTNodeReferences(editorData.SyntaxTree, nodeSymbolToFind, ctxt)
                        .Where(region => region != null)
                        .OrderBy(region => region.Location)
                        .Select(ToDocumentHighlight)
                        .ToList();
            }
            catch (Exception ex)
            {
                workDone.OnError(ex);
                if (progress != null)
                {
                    progress.OnError(ex);
                    return Task.FromResult(new DocumentHighlightContainer());
                }

                return Task.FromException<DocumentHighlightContainer>(ex);
            }

            if (allFoundReferences.Count > 0)
                progress?.OnNext(new DocumentHighlightContainer(allFoundReferences));

            workDone.OnCompleted();
            progress?.OnCompleted();

            return Task.FromResult(progress != null
                ? new DocumentHighlightContainer()
                : new DocumentHighlightContainer(allFoundReferences));
        }

        static DocumentHighlight ToDocumentHighlight(ISyntaxRegion syntaxRegion)
        {
            return new DocumentHighlight
            {
                Range = syntaxRegion.GetNameRange(),
                Kind = DocumentHighlightKind.Text
            };
        }
    }
}