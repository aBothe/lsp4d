using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using D_Parser.Dom;
using D_Parser.Resolver;
using D_Parserver.DHandler.Resolution;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace D_Parserver.DHandler
{
    public class DHoverHandler : HoverHandler
    {
        public DHoverHandler()
            : base(new HoverRegistrationOptions{DocumentSelector = TextDocumentHandler.DocumentSelector})
        {
            SetCapability(new HoverCapability{ ContentFormat = new Container<MarkupKind>(MarkupKind.PlainText)});
        }

        public override Task<Hover> Handle(HoverParams request, CancellationToken cancellationToken)
        {
            var editorData = DResolverWrapper.CreateEditorData(request, cancellationToken);
            var resolvedHoveredTypes = LooseResolution.ResolveTypeLoosely(editorData, out LooseResolution.NodeResolutionAttempt resolutionAttempt, out ISyntaxRegion syntaxRegion, true);
            
            var markup = string.Join("\n", AmbiguousType.TryDissolve(resolvedHoveredTypes)
                    .Where(t => t != null)
                    .Select(t => TooltipMarkupGen.CreateSignatureMarkdown(t)));

            return Task.FromResult(new Hover
            {
                Contents = new MarkedStringsOrMarkupContent(new MarkupContent
                {
                    Kind = MarkupKind.Markdown, 
                    Value = markup
                }),
                Range = syntaxRegion.ToRange()
            });
        }
    }
}