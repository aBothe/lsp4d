using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using D_Parser.Misc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace D_Parserver.DHandler.Resolution
{
    public class WorkspaceManager
    {
        public static readonly BehaviorSubject<ImmutableList<string>> WorkspaceFolders
            = new BehaviorSubject<ImmutableList<string>>(ImmutableList<string>.Empty);

        public static async Task SetWorkspaceRoots(IEnumerable<string> workspaceFolders,
            ProgressManager progressManager, IWorkDoneProgressParams workDoneProgressParams)
        {
            WorkspaceFolders.OnNext(workspaceFolders.ToImmutableList());

            var completionSource = new TaskCompletionSource<bool>();
            var initialParseProgress = progressManager.WorkDone(workDoneProgressParams, new WorkDoneProgressBegin()
            {
                Title = "Begin parsing workspace",
                Percentage = 0
            });

            var partProgresses = GlobalParseCache.BeginAddOrUpdatePaths(WorkspaceFolders.Value, finishedHandler:
                ea =>
                {
                    initialParseProgress.OnNext(new WorkDoneProgressReport()
                    {
                        Message = "Done parsing workspace",
                        Percentage = 100
                    });
                    initialParseProgress.OnCompleted();
                    completionSource.SetResult(true);
                });

            int partIndex = 1;
            foreach (var statisticsHandle in partProgresses)
            {
                initialParseProgress.OnNext(new WorkDoneProgressReport()
                {
                    Percentage = partIndex / partProgresses.Count,
                    Message = "Parse " + statisticsHandle.basePath
                });
                statisticsHandle.WaitForCompletion();
                partIndex++;

                initialParseProgress.OnNext(new WorkDoneProgressReport()
                {
                    Percentage = partIndex / partProgresses.Count,
                    Message = "Finished parsing " + statisticsHandle.basePath
                });
            }

            await completionSource.Task;
        }
    }
}