using System.IO;
using DParserverTests.Util;
using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace DParserverTests
{
    public class DReferencesHandlerTests : LspTest
    {
        public static readonly string ModuleAFile = Path.Combine(Lsp4DUtil.DefaultSrcFolder, "a.d");
        public static readonly string ModuleBFile = Path.Combine(Lsp4DUtil.DefaultSrcFolder, "b.d");
        
        [Test]
        public void EnumReferences_ReturnsReferences()
        {
            Client.TextDocument.DidOpen(ModuleAFile, Lsp4DUtil.DLANG, @"module modA;
class AClass {}
class BClass {}

void bar() {
    auto a = new AClass();
}
");
            
            Client.TextDocument.DidOpen(ModuleBFile, Lsp4DUtil.DLANG, @"module modB;
AClass ainstance;
void foo(T = AClass)() {}
");

            Client.SendRequest<LocationContainer>("textDocument/references", new ReferenceParams
            {
                Context = new ReferenceContext {IncludeDeclaration = true}
            });//WIP
        }
    }
}