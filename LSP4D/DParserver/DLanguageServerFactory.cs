using System.IO;
using System.Threading.Tasks;
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
                    .WithHandler<TextDocumentHandler>()
            );
        }
    }
}