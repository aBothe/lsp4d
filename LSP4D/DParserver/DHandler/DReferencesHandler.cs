using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using D_Parser.Dom;
using D_Parser.Dom.Expressions;
using D_Parser.Refactoring;
using D_Parser.Resolver;
using D_Parser.Resolver.ExpressionSemantics;
using D_Parserver.DHandler.Resolution;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace D_Parserver.DHandler
{
    public class DReferencesHandler : ReferencesHandler
    {
        public DReferencesHandler(ProgressManager progressManager)
            : base(new ReferenceRegistrationOptions{ DocumentSelector = TextDocumentHandler.DocumentSelector, WorkDoneProgress = true }, progressManager)
        {
            SetCapability(new ReferenceCapability());
        }

        public override Task<LocationContainer> Handle(ReferenceParams request, CancellationToken cancellationToken)
        {
            var workDone = ProgressManager.WorkDone(request, new WorkDoneProgressBegin
            {
                Title = "Begin resolving references",
                Percentage = 0
            });
            
            var editorData = DResolverWrapper.CreateEditorData(request, cancellationToken);
            var nodeSymbolToFind = DResolverWrapper
                .ResolveHoveredCodeLoosely(editorData, out LooseResolution.NodeResolutionAttempt _, out ISyntaxRegion _)
                .Select(ExpressionTypeEvaluation.GetResultMember)
                .FirstOrDefault();

            if (nodeSymbolToFind == null)
            {
                workDone.OnNext(new WorkDoneProgressReport()
                {
                    Message = "No symbol found",
                    Percentage = 100
                });
                workDone.OnCompleted();
                return Task.FromResult(new LocationContainer());
            }

            var modules = editorData.ParseCache.EnumRootPackagesSurroundingModule(editorData.SyntaxTree)
                .SelectMany(package => (IEnumerable<DModule>)package)
                .Where(module => module != null)
                .OrderBy(module => module.FileName)
                .ToList();

            var progress = ProgressManager.For(request, cancellationToken);
            var allFoundReferences = new List<Location>();
            var ctxt = ResolutionContext.Create (editorData, true);
            int progressIndex = 0;
            foreach (var module in modules)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    var references = ReferencesFinder.SearchModuleForASTNodeReferences(module, nodeSymbolToFind, ctxt, request.Context.IncludeDeclaration)
                        .Where(region => region != null)
                        .OrderBy(region => region.Location)
                        .Select(region => ToLocation(region, module))
                        .Where(location => location != default)
                        .ToList();
                    progressIndex++;
                    if (references.Count > 0)
                    {
                        workDone.OnNext(new WorkDoneProgressReport
                        {
                            Message = "Scanned " + module.FileName,
                            Percentage = ((double) progressIndex / modules.Count) * 100.0
                        });
                        allFoundReferences.AddRange(references);
                        progress?.OnNext(new Container<Location>(allFoundReferences));
                    }
                }
                catch (Exception ex)
                {
                    workDone.OnError(ex);
                    if (progress != null)
                    {
                        progress.OnError(ex);
                        return Task.FromResult(new LocationContainer());
                    }
                    return Task.FromException<LocationContainer>(ex);
                }
            }
            
            workDone.OnCompleted();
            progress?.OnCompleted();

            // if $progress sent, result must be empty
            return Task.FromResult(progress != null ? new LocationContainer() 
                : new LocationContainer(allFoundReferences));
        }

        static Location ToLocation(ISyntaxRegion reference, DModule module)
        {
            CodeLocation startLocation;
            CodeLocation endLocation;

            switch (reference)
            {
                case AbstractTypeDeclaration abstractTypeDeclaration:
                    startLocation = abstractTypeDeclaration.NonInnerTypeDependendLocation;
                    endLocation = abstractTypeDeclaration.EndLocation;
                    break;
                case IExpression _:
                    startLocation = reference.Location;
                    endLocation = reference.EndLocation;
                    break;
                case TemplateParameter templateParameter:
                    startLocation = templateParameter.NameLocation;
                    endLocation = new CodeLocation(templateParameter.NameLocation.Column + templateParameter.Name.Length, 
                        templateParameter.NameLocation.Line);
                    break;
                case INode n:
                    startLocation = n.NameLocation;
                    endLocation = new CodeLocation(n.NameLocation.Column + n.Name.Length, n.NameLocation.Line);
                    break;
                default:
                    return default;
            }
            
            return new Location
            {
                Range = new Range(new Position(startLocation.Line - 1, startLocation.Column - 1), 
                    new Position(endLocation.Line - 1, endLocation.Column - 1)),
                Uri = new Uri(module.FileName)
            };
        }
    }
}