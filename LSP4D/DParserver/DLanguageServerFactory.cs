using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using D_Parserver.DHandler;
using D_Parserver.DHandler.Resolution;
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
                    .WithHandler<DDefinitionHandler>()
                    .WithHandler<TextDocumentHandler>()
                    .WithHandler<DHoverHandler>()
                    .WithHandler<DReferencesHandler>()
                    .WithHandler<DSelectionRangeHandler>()
                    .WithHandler<DCompletionHandler>()
                    .OnInitialize(async (server, request) =>
                    {
                        var workspaceFolders = new HashSet<string>();
                        workspaceFolders.Add(request.RootPath);
                        if (request.WorkspaceFolders != null)
                        {
                            request.WorkspaceFolders.Select(folder => workspaceFolders.Add(folder.Uri.AbsolutePath));
                        }

                        await WorkspaceManager.SetWorkspaceRoots(workspaceFolders, server.ProgressManager, request);
                    })
            );
        }
    }
}