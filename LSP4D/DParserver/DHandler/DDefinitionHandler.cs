using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using D_Parser.Dom;
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
    public class DDefinitionHandler : DefinitionHandler
    {
        public DDefinitionHandler(ProgressManager progressManager)
            : base(new DefinitionRegistrationOptions{ DocumentSelector = TextDocumentHandler.DocumentSelector }, progressManager)
        {
            SetCapability(new DefinitionCapability{LinkSupport = false});
        }

        public override Task<LocationOrLocationLinks> Handle(DefinitionParams request, CancellationToken cancellationToken)
        {
            var editorData = DResolverWrapper.CreateEditorData(request, cancellationToken);
            var results = DResolverWrapper.ResolveHoveredCodeLoosely(editorData, out LooseResolution.NodeResolutionAttempt resolutionAttempt, out ISyntaxRegion syntaxRegion);

            return Task.FromResult(new LocationOrLocationLinks(
                results.Select(ExpressionTypeEvaluation.GetResultMember)
                    .Select(node => ToLocationOrLocationLink(node, Capability.LinkSupport, syntaxRegion))));
        }

        public static LocationOrLocationLink ToLocationOrLocationLink(INode node, bool linkSupport, ISyntaxRegion sourceSyntaxRegion = null)
        {
            var targetUri = new Uri(((DModule) node.NodeRoot).FileName);
            var nodeNameLocation = new Range(
                new Position(node.NameLocation.Line - 1, node.NameLocation.Column - 1), 
                new Position(node.NameLocation.Line - 1, node.NameLocation.Column + node.Name.Length - 1));

            if (linkSupport)
            {
                return new LocationOrLocationLink(new LocationLink
                {
                    TargetUri = targetUri,
                    TargetSelectionRange = nodeNameLocation,
                    TargetRange = new Range(
                        new Position(node.Location.Line - 1, node.Location.Column - 1), 
                        new Position(node.EndLocation.Line - 1, node.EndLocation.Column - 1)),
                    OriginSelectionRange = sourceSyntaxRegion == null ? null : new Range(
                        new Position(sourceSyntaxRegion.Location.Line - 1, sourceSyntaxRegion.Location.Column - 1), 
                        new Position(sourceSyntaxRegion.EndLocation.Line - 1, sourceSyntaxRegion.EndLocation.Column - 1))
                });
            }
            return new LocationOrLocationLink(new Location
            {
                Uri = targetUri,
                Range = nodeNameLocation
            });
        }
    }
}