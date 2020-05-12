using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using D_Parserver;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Client.Processes;
using OmniSharp.Extensions.LanguageServer.Server;

namespace DParserverTests.Util
{
    class DummyServerProcess : ServerProcess
    {
        private ILanguageServer _languageServer;
        
        public DummyServerProcess(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            InputStream = new MemoryQueueBufferStream();
            OutputStream = new MemoryQueueBufferStream();
        }

        public override Task Start()
        {
            return DLanguageServerFactory.CreateServer(OutputStream, InputStream)
                .ContinueWith(task => _languageServer = task.Result);
        }

        public override async Task Stop()
        {
            await _languageServer.Shutdown;
        }

        public override bool IsRunning => true;
        public override Stream InputStream { get; }
        public override Stream OutputStream { get; }
    }
}