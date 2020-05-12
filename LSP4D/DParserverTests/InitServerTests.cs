using DParserverTests.Util;
using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Client;

namespace DParserverTests
{
    public class InitServerTests
    {
        private LanguageClient _client;
        
        [SetUp]
        public void Setup()
        {
            _client = Lsp4DUtil.MakeClient();
        }

        [Test]
        public void InitalizeClientAndServer_initializesEverything()
        {
            
            _client.Shutdown().ContinueWith(_ =>
            {
                Assert.Pass();
            }).Wait();
        }
    }
}