using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using D_Parser.Misc;
using D_Parserver.DHandler;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;

namespace D_Parserver
{
    public static class DLanguageServerFactory
    {
        public static Task<ILanguageServer> CreateServer(Stream input, Stream output)
        {
            return LanguageServer.From(options =>
                options
                    .WithInput(input)
                    .WithOutput(output)
                    .WithHandler<FoldingRangeHandler>()
                    .WithHandler<TextDocumentHandler>()
                    .OnInitialize(async (server, request) => {
                        var completionSource = new TaskCompletionSource<bool>();
                        var initialParseProgress = server.ProgressManager.WorkDone(request, new WorkDoneProgressBegin() {
                            Title = "Begin parsing workspace",
                            Percentage = 0
                        });
                        
                        var workspaceFolders = new HashSet<string>();
                        workspaceFolders.Add(request.RootPath);
                        if (request.WorkspaceFolders != null)
                        {
                            request.WorkspaceFolders.Select(folder => workspaceFolders.Add(folder.Uri.AbsolutePath));
                        }
                        
                        var partProgresses = GlobalParseCache.BeginAddOrUpdatePaths(workspaceFolders, finishedHandler:
                            ea =>
                            {
                                initialParseProgress.OnNext(new WorkDoneProgressReport() {
                                    Message = "Done parsing workspace",
                                    Percentage = 100
                                });
                                initialParseProgress.OnCompleted();
                                completionSource.SetResult(true);
                            });

                        int partIndex = 1;
                        foreach (var statisticsHandle in partProgresses)
                        {
                            initialParseProgress.OnNext(new WorkDoneProgressReport() {
                                Percentage = partIndex / partProgresses.Count,
                                Message = "Parse " + statisticsHandle.basePath
                            });
                            statisticsHandle.WaitForCompletion();
                            partIndex++;
                            
                            initialParseProgress.OnNext(new WorkDoneProgressReport() {
                                Percentage = partIndex / partProgresses.Count,
                                Message = "Finished parsing " + statisticsHandle.basePath
                            });
                        }

                        await completionSource.Task;
                    })
            );
        }
    }
}