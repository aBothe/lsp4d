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

            if (linkSupport)
            {
                return new LocationOrLocationLink(new LocationLink
                {
                    TargetUri = targetUri,
                    TargetSelectionRange = node.ToNameLocationRange(),
                    TargetRange = node.ToRange(),
                    OriginSelectionRange = sourceSyntaxRegion.ToRange()
                });
            }
            return new LocationOrLocationLink(new Location
            {
                Uri = targetUri,
                Range = node.ToNameLocationRange()
            });
        }
    }
}