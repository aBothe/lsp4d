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
            TearDown();
            _client = Lsp4DUtil.MakeClient();
        }

        [TearDown]
        public void TearDown()
        {
            if (_client != null)
            {
                Assert.IsTrue(_client.Shutdown().Wait(5000));
            }
            _client = null;
            //Lsp4DUtil.CleanDefaultWorkspace();
        }

        [Test]
        public void InitalizeClientAndServer_initializesEverything()
        {
            Assert.IsTrue(_client.Shutdown().Wait(5000));
        }
    }
}