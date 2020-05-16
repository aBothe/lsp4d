using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace DParserverTests.Util
{
    public class LspTest
    {
        public LanguageClient Client { protected get; set; }

        protected virtual void ConfigureClientCapabilities(ClientCapabilities clientCapabilities)
        {
        }
        
        [SetUp]
        public void Setup()
        {
            TearDown();
            Client = Lsp4DUtil.MakeClient(ConfigureClientCapabilities);
        }

        [TearDown]
        public void TearDown()
        {
            if (Client != null)
            {
                Client.Dispose();
                Assert.IsTrue(Client.Shutdown().Wait(5000));
            }
            Client = null;
            //Lsp4DUtil.CleanDefaultWorkspace();
        }
    }
}