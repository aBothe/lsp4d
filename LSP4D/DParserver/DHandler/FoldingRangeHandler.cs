using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using D_Parser.Dom;
using D_Parser.Dom.Statements;
using D_Parser.Misc;
using D_Parser.Parser;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;

namespace D_Parserver.DHandler
{
    class FoldingRangeHandler : IFoldingRangeHandler
    {
        private FoldingRangeCapability _capability;
        private readonly ILanguageServerConfiguration _configuration;

        public FoldingRangeHandler(ILanguageServerConfiguration configuration)
        {
            _configuration = configuration;
        }

        public FoldingRangeRegistrationOptions GetRegistrationOptions()
        {
            return new FoldingRangeRegistrationOptions
            {
                DocumentSelector = TextDocumentHandler.DocumentSelector
            };
        }

        public void SetCapability(FoldingRangeCapability capability)
        {
            _capability = capability;
        }

        public async Task<Container<FoldingRange>> Handle(FoldingRangeRequestParam request,
            CancellationToken cancellationToken)
        {
            await Task.Yield();
            var module = GlobalParseCache.GetModule(request.TextDocument.Uri.AbsolutePath)
                         ?? TextDocumentHandler.OpenFiles[request.TextDocument.Uri].Module;
            var l = new List<FoldingRange>();

            // Add primary node folds
            FoldingVisitor.GenerateFoldsInternal(l, module);

            // Add multiline comment folds
            for (int i = 0; i < module.Comments.Length; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var c = module.Comments[i];
                if (c.CommentType == Comment.Type.SingleLine)
                {
                    int nextIndex = i + 1;
                    Comment lastComment;

                    // Customly foldable code regions
                    if (c.CommentText.TrimStart().StartsWith("region"))
                    {
                        bool cont = false;
                        for (int j = i + 1; j < module.Comments.Length; j++)
                        {
                            lastComment = module.Comments[j];
                            if (lastComment.CommentType == Comment.Type.SingleLine &&
                                lastComment.CommentText.Trim() == "endregion")
                            {
                                //TODO: Inhibit fold-processing the endregion comment in other cases.
                                var text = c.CommentText.Trim().Substring(6).Trim();
                                if (text == string.Empty)
                                    text = "//region";
                                l.Add(new FoldingRange
                                {
                                    StartLine = c.StartPosition.Line - 1,
                                    StartCharacter = c.StartPosition.Column - 1,
                                    EndLine = lastComment.EndPosition.Line - 1,
                                    EndCharacter = lastComment.EndPosition.Column - 1,
                                    Kind = FoldingRangeKind.Region
                                });
                                cont = true;
                                break;
                            }
                        }

                        if (cont)
                            continue;
                    }

                    lastComment = null;


                    for (int j = i + 1; j < module.Comments.Length; j++)
                    {
                        lastComment = module.Comments[j];
                        if (lastComment.CommentType != c.CommentType ||
                            lastComment.StartPosition.Column != c.StartPosition.Column ||
                            lastComment.StartPosition.Line != module.Comments[j - 1].StartPosition.Line + 1)
                        {
                            lastComment = j == nextIndex ? null : module.Comments[j - 1];
                            break;
                        }

                        i++;
                    }

                    if (lastComment == null)
                        continue;

                    l.Add(new FoldingRange
                    {
                        StartLine = c.StartPosition.Line - 1,
                        StartCharacter = c.StartPosition.Column - 1,
                        EndLine = lastComment.StartPosition.Line - 1,
                        EndCharacter = lastComment.EndPosition.Column + 1 - 1,
                        Kind = FoldingRangeKind.Comment
                    });
                }
                else
                    l.Add(new FoldingRange
                    {
                        StartLine = c.StartPosition.Line - 1,
                        StartCharacter = c.StartPosition.Column - 1,
                        EndLine = c.EndPosition.Line - 1,
                        EndCharacter = c.EndPosition.Column + 1 - 1,
                        Kind = FoldingRangeKind.Comment
                    });
            }

            return new Container<FoldingRange>(l);
        }
    }

    class FoldingVisitor : DefaultDepthFirstVisitor
    {
        readonly List<FoldingRange> l;

        private FoldingVisitor(List<FoldingRange> l)
        {
            this.l = l;
        }

        public static void GenerateFoldsInternal(List<FoldingRange> l, IBlockNode block)
        {
            block?.Accept(new FoldingVisitor(l));
        }

        public static FoldingRange GetBlockBodyRegion(IBlockNode n)
        {
            return new FoldingRange
            {
                StartLine = n.BlockStartLocation.Line - 1,
                StartCharacter = n.BlockStartLocation.Column - 1,
                EndLine = n.EndLocation.Line - 1,
                EndCharacter = n.EndLocation.Column - 1 + 1,
                Kind = FoldingRangeKind.Region
            };
        }

        public override void Visit(BlockStatement s)
        {
            l.Add(new FoldingRange
            {
                StartLine = s.Location.Line - 1,
                StartCharacter = s.Location.Column - 1,
                EndLine = s.EndLocation.Line - 1,
                EndCharacter = s.EndLocation.Column - 1 + 1,
                Kind = FoldingRangeKind.Region
            });

            base.Visit(s);
        }

        public override void Visit(D_Parser.Dom.Expressions.StructInitializer x)
        {
            l.Add(new FoldingRange
            {
                StartLine = x.Location.Line - 1,
                StartCharacter = x.Location.Column - 1,
                EndLine = x.EndLocation.Line - 1,
                EndCharacter = x.EndLocation.Column - 1 + 1,
                Kind = FoldingRangeKind.Region
            });

            base.Visit(x);
        }

        public override void Visit(DClassLike n)
        {
            l.Add(GetBlockBodyRegion(n));
            base.Visit(n);
        }

        public override void Visit(DEnum n)
        {
            l.Add(GetBlockBodyRegion(n));

            base.Visit(n);
        }

        public override void VisitIMetaBlock(IMetaDeclarationBlock mdb)
        {
            l.Add(new FoldingRange
            {
                StartLine = mdb.BlockStartLocation.Line - 1,
                StartCharacter = mdb.BlockStartLocation.Column - 1,
                EndLine = mdb.EndLocation.Line - 1,
                EndCharacter = mdb.EndLocation.Column - 1 + 1,
                Kind = FoldingRangeKind.Region
            });
        }
    }
}