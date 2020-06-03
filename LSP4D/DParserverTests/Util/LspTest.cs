using D_Parser;
using D_Parser.Dom;
using D_Parser.Misc;
using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

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
            GlobalParseCache.RemoveRoot(Lsp4DUtil.DefaultWorkspaceRoot);
            if (Client != null)
            {
                Client.Dispose();
                Assert.IsTrue(Client.Shutdown().Wait(5000));
            }
            Client = null;
            //Lsp4DUtil.CleanDefaultWorkspace();
        }
        
        /// <summary>
        /// Use § for caret location!
        /// </summary>
        public Position OpenMainFile(string withCode)
        {
            return OpenFile(Lsp4DUtil.DefaultMainFile, withCode);
        }
        
        public Position OpenFile(string file, string withCode)
        {
            var caretOffset = withCode.IndexOf('§');
            CodeLocation caret;
            if (caretOffset != -1)
            {
                withCode = withCode.Substring(0, caretOffset) + withCode.Substring(caretOffset + 1);
                caret = DocumentHelper.OffsetToLocation(withCode, caretOffset);
            }
            else
            {
                caret = new CodeLocation(1, 1);
            }
            
            Client.TextDocument.DidOpen(file, Lsp4DUtil.DLANG, withCode);
            
            return new Position(caret.Line - 1, caret.Column - 1);
        }
    }
}