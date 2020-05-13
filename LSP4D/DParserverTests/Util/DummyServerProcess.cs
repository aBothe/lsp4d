using System.IO;
using System.Threading.Tasks;
using D_Parserver;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Client.Processes;
using OmniSharp.Extensions.LanguageServer.Server;

namespace DParserverTests.Util
{
    class DummyServerProcess : ServerProcess
    {
        private Task<ILanguageServer> _languageServer;
        private bool _running;

        public DummyServerProcess(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            InputStream = new MemoryQueueBufferStream();
            OutputStream = new MemoryQueueBufferStream();
        }

        public override async Task Start()
        {
            _languageServer = DLanguageServerFactory.CreateServer(InputStream, OutputStream)
                .ContinueWith(task =>
            {
                task.Result.WaitForExit.ContinueWith(task1 =>
                {
                    OnExited();
                    ServerExitCompletion.TrySetResult(null);
                    ServerStartCompletion = new TaskCompletionSource<object>();
                });
                return task.Result;
            });
            _running = true;
            await Task.CompletedTask;
        }

        public override async Task Stop()
        {
            _running = false;
            await Task.CompletedTask;
        }

        public override bool IsRunning => _running;
        public override Stream InputStream { get; }
        public override Stream OutputStream { get; }
    }
}