using System;
using System.Linq;
using DParserverTests.Util;
using Newtonsoft.Json;
using NUnit.Framework;

namespace DParserverTests
{
    public class DDefinitionHandlerTests : LspTest
    {
        [Test]
        public void CallGotoDeclarationHandler_ReturnsLocations()
        {
            Client.TextDocument.DidOpen(Lsp4DUtil.DefaultMainFile, Lsp4DUtil.DLANG, @"module main;
class MyClass {}

MyClass instance;");

            var definitions = Client.TextDocument.Definition(Lsp4DUtil.DefaultMainFile, 3, 2).Result.ToList();
            
            Assert.AreEqual("[{\"IsLocation\":false,\"Location\":null,\"IsLocationLink\":true,\"LocationLink\":{" +
                            "\"OriginSelectionRange\":{\"Start\":{\"Line\":3,\"Character\":0},\"End\":{\"Line\":3,\"Character\":7}}," +
                            $"\"TargetUri\":\"{new Uri(Lsp4DUtil.DefaultMainFile).AbsoluteUri}\"," +
                            "\"TargetRange\":{\"Start\":{\"Line\":1,\"Character\":0},\"End\":{\"Line\":1,\"Character\":16}}," +
                            "\"TargetSelectionRange\":{\"Start\":{\"Line\":1,\"Character\":6},\"End\":{\"Line\":1,\"Character\":13}}" +
                            "}}]", JsonConvert.SerializeObject(definitions));
        }
    }
}