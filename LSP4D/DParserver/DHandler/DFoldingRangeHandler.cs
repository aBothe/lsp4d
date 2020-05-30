using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using D_Parser.Dom;
using D_Parser.Parser;
using D_Parserver.DHandler.Resolution;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace D_Parserver.DHandler
{
    class DFoldingRangeHandler : FoldingRangeHandler
    {
        public DFoldingRangeHandler(ProgressManager progressManager) :
            base(new FoldingRangeRegistrationOptions
            {
                DocumentSelector = TextDocumentHandler.DocumentSelector,
                WorkDoneProgress = true
            }, progressManager)
        {
        }

        public override async Task<Container<FoldingRange>> Handle(FoldingRangeRequestParam request,
            CancellationToken cancellationToken)
        {
            var progress = ProgressManager.For(request, cancellationToken);
            var workDone = ProgressManager.WorkDone(request, new WorkDoneProgressBegin
            {
                Message = "Begin getting fold ranges",
                Percentage = 0
            });
            var module = TextDocumentHandler.GetAstModule(request.TextDocument.Uri);
            var l = new List<FoldingRange>(FoldingVisitor.GenerateFoldsInternal(module));
            progress?.OnNext(new Container<FoldingRange>(l));

            if (!cancellationToken.IsCancellationRequested)
                l.AddRange(GenerateMultilineCommentFolds(module));
            l.AddRange(GenerateMultiSingleLineCommentFolds(cancellationToken, module));
            l.AddRange(GenerateRegionFolds(cancellationToken, module));

            progress?.OnNext(new Container<FoldingRange>(l));

            workDone.OnCompleted();
            progress?.OnCompleted();
            return progress != null ? new Container<FoldingRange>() : new Container<FoldingRange>(l);
        }

        private static IEnumerable<FoldingRange> GenerateMultilineCommentFolds(DModule module)
        {
            return module.Comments
                .Where(c => c.CommentType.HasFlag(Comment.Type.Block))
                .Select(c => new FoldingRange
                {
                    StartLine = c.StartPosition.Line - 1,
                    StartCharacter = c.StartPosition.Column - 1,
                    EndLine = c.EndPosition.Line - 1,
                    EndCharacter = c.EndPosition.Column - 1,
                    Kind = FoldingRangeKind.Comment
                });
        }

        private static IEnumerable<FoldingRange> GenerateMultiSingleLineCommentFolds(
            CancellationToken cancellationToken, DModule module)
        {
            for (var currentCommentIndex = 0;
                currentCommentIndex < module.Comments.Length && !cancellationToken.IsCancellationRequested;
                currentCommentIndex++)
            {
                var c = module.Comments[currentCommentIndex];
                if (!c.CommentType.HasFlag(Comment.Type.SingleLine))
                    continue;

                var lastComment = SearchNextMatchingSingleLineComment(module, c, ref currentCommentIndex);
                if (lastComment != null)
                {
                    yield return new FoldingRange
                    {
                        StartLine = c.StartPosition.Line - 1,
                        StartCharacter = c.StartPosition.Column - 1,
                        EndLine = lastComment.StartPosition.Line - 1,
                        EndCharacter = lastComment.EndPosition.Column - 1,
                        Kind = FoldingRangeKind.Comment
                    };
                }
            }
        }

        private static Comment SearchNextMatchingSingleLineComment(DModule module, Comment c,
            ref int currentCommentIndex)
        {
            var nextIndex = currentCommentIndex + 1;
            Comment lastComment = null;

            // Peek next comments
            for (var peekCommentIndex = currentCommentIndex + 1;
                peekCommentIndex < module.Comments.Length;
                peekCommentIndex++)
            {
                lastComment = module.Comments[peekCommentIndex];
                if (lastComment.CommentType != c.CommentType ||
                    lastComment.StartPosition.Column != c.StartPosition.Column ||
                    lastComment.StartPosition.Line != module.Comments[peekCommentIndex - 1].StartPosition.Line + 1)
                {
                    return peekCommentIndex == nextIndex ? null : module.Comments[peekCommentIndex - 1];
                }

                currentCommentIndex++;
            }

            return lastComment;
        }

        private static IEnumerable<FoldingRange> GenerateRegionFolds(
            CancellationToken cancellationToken, DModule module)
        {
            for (int i = 0; i < module.Comments.Length && !cancellationToken.IsCancellationRequested; i++)
            {
                var c = module.Comments[i];
                if (c.CommentType != Comment.Type.SingleLine || !c.CommentText.TrimStart().StartsWith("region"))
                    continue;

                var endRegionComment = module.Comments
                    .Skip(i)
                    .FirstOrDefault(c => c.CommentType == Comment.Type.SingleLine && c.CommentText.Trim() == "endregion");
                if (endRegionComment != null)
                {
                    //TODO: Inhibit fold-processing the endregion comment in other cases.
                    yield return new FoldingRange
                    {
                        StartLine = c.StartPosition.Line - 1,
                        StartCharacter = c.StartPosition.Column - 1,
                        EndLine = endRegionComment.StartPosition.Line - 1,
                        EndCharacter = endRegionComment.EndPosition.Column - 1,
                        Kind = FoldingRangeKind.Region
                    };
                }
            }
        }
    }
}