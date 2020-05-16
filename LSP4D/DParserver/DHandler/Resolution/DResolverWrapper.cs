using System.Linq;
using System.Threading;
using D_Parser;
using D_Parser.Completion;
using D_Parser.Dom;
using D_Parser.Misc;
using D_Parser.Resolver;
using D_Parser.Resolver.TypeResolution;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace D_Parserver.DHandler.Resolution
{
    public class DResolverWrapper
    {
        private static ParseCacheView CreateParseCacheView()
        {
            return new LegacyParseCacheView(WorkspaceManager.WorkspaceFolders.Value);
        }

        public static EditorData CreateEditorData(TextDocumentPositionParams documentPositionParams, CancellationToken cancellationToken)
        {
            var module = TextDocumentHandler.GetAstModule(documentPositionParams.TextDocument.Uri);

            var code = TextDocumentHandler.GetModuleCode(documentPositionParams.TextDocument.Uri);
            var caret = new CodeLocation((int)documentPositionParams.Position.Character + 1, (int)documentPositionParams.Position.Line + 1);

            var editorData = new EditorData
            {
                ParseCache = CreateParseCacheView(),
                CancelToken = cancellationToken,
                CaretLocation = caret,
                CaretOffset = DocumentHelper.LocationToOffset(code, caret),
                ModuleCode = code,
                SyntaxTree = module,
                GlobalVersionIds = VersionIdEvaluation.GetOSAndCPUVersions()
            };
            return documentPositionParams is CompletionParams completionParams
                ? PrepareForCompletionRequest(editorData, completionParams)
                : editorData;
        }

        private static EditorData PrepareForCompletionRequest(EditorData ed, CompletionParams completionParams)
        {
            /*TODO
            if (documentPositionParams is CompletionParams completionParams)
            {
                bool removeChar = completionParams.Context.TriggerKind == 
            }
            bool removeChar = char.IsLetter(triggerChar) || triggerChar == '_';
            var deltaOffset = 0;//removeChar ? 1 : 0;

            var caretOffset = ctx.TriggerOffset - (removeChar ? 1 : 0);
            var caretLocation = new CodeLocation(ctx.TriggerLineOffset - deltaOffset, ctx.TriggerLine);
            */
            return ed;
        }


        public static AbstractType[] ResolveHoveredCode(out ResolutionContext ResolverContext, IEditorData edData)
        {
            ResolverContext = ResolutionContext.Create(edData, false);

            // Resolve the hovered piece of code
            return AmbiguousType.TryDissolve(DResolver.ResolveType(edData, ctxt:ResolverContext)).ToArray();
        }

        public static AbstractType[] ResolveHoveredCodeLoosely(IEditorData ed, out LooseResolution.NodeResolutionAttempt resolutionAttempt, out ISyntaxRegion sr)
        {
            return AmbiguousType.TryDissolve(LooseResolution.ResolveTypeLoosely(ed, out resolutionAttempt, out sr, true)).ToArray();
        }
    }
}