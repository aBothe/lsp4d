using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Client;

namespace DParserverTests.Util
{
    public class LspTest
    {
        public LanguageClient Client { protected get; set; }

        [SetUp]
        public void Setup()
        {
            TearDown();
            Client = Lsp4DUtil.MakeClient();
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