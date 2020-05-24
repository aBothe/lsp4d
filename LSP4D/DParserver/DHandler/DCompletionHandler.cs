using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using D_Parser.Completion;
using D_Parser.Dom;
using D_Parserver.DHandler.Completion;
using D_Parserver.DHandler.Resolution;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace D_Parserver.DHandler
{
    public class DCompletionHandler : CompletionHandler
    {
        public DCompletionHandler(ProgressManager progressManager)
            : base(new CompletionRegistrationOptions
            {
                DocumentSelector = TextDocumentHandler.DocumentSelector,
                ResolveProvider = true,
                AllCommitCharacters = new Container<string>(" ", "\n", ".", "\t"),
                TriggerCharacters = new Container<string>("."),
                WorkDoneProgress = true
            }, progressManager)
        {
        }

        public override Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken)
        {
            var ed = CreateCompletionEditorData(request, cancellationToken, out char triggerChar);
            var workDone = ProgressManager.WorkDone(request,
                new WorkDoneProgressBegin {Message = "Begin generating completion info", Percentage = 0});
            var progress = ProgressManager.For(request, cancellationToken);

            var allResults = new List<CompletionItem>();

            var completionDataGenerator = new CompletionDataGeneratorImpl(item =>
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                allResults.Add(item);
                if (allResults.Count % 15 == 0)
                {
                    progress?.OnNext(new CompletionList(allResults));
                }
            });
            CodeCompletion.GenerateCompletionData(ed, completionDataGenerator, triggerChar);

            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<CompletionList>(cancellationToken);
            }
            workDone.OnCompleted();
            progress?.OnNext(new CompletionList(allResults));
            progress?.OnCompleted();

            return Task.FromResult(progress != null ? new CompletionList() : new CompletionList(allResults));
        }

        private static EditorData CreateCompletionEditorData(CompletionParams completionParams,
            CancellationToken cancellationToken, out char triggerChar)
        {
            var triggerString = completionParams.Context.TriggerCharacter ?? string.Empty;
            triggerChar = triggerString.Length > 0 ? triggerString[0] : '\0';

            var editorData = DResolverWrapper.CreateEditorData(completionParams, cancellationToken);

            bool removeChar = char.IsLetter(triggerChar) || triggerChar == '_';
            if (removeChar)
            {
                editorData.CaretOffset -= 1;
                editorData.CaretLocation = new CodeLocation(editorData.CaretLocation.Column - 1,
                    editorData.CaretLocation.Line);
            }

            return editorData;
        }

        public override Task<CompletionItem> Handle(CompletionItem request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override bool CanResolve(CompletionItem value)
        {
            throw new NotImplementedException();
        }
    }
}