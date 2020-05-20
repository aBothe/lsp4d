using System;
using System.IO;
using DParserverTests.Util;
using Newtonsoft.Json;
using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace DParserverTests
{
    public class DReferencesHandlerTests : LspTest
    {
        public static readonly string ModuleAFile = Path.Combine(Lsp4DUtil.DefaultSrcFolder, "a.d");
        public static readonly string ModuleBFile = Path.Combine(Lsp4DUtil.DefaultSrcFolder, "b.d");

        [Test]
        public void CheckWorkDoneProgressCap()
        {
            Assert.IsTrue(Client.ServerCapabilities.ReferencesProvider.Value.WorkDoneProgress);
        }
        
        [Test]
        public void EnumReferences_IncludeDeclaration_ReturnsReferences()
        {
            Client.TextDocument.DidOpen(ModuleAFile, Lsp4DUtil.DLANG, @"module modA;
class AClass {}
class BClass {}

void bar() {
    auto a = new AClass();
}
");

            Client.TextDocument.DidOpen(ModuleBFile, Lsp4DUtil.DLANG, @"module modB;
import modA;
AClass ainstance;
void foo(T: AClass)() {}
");

            var workTester = WorkAndProgressTester.Setup(Client);

            var result = Client.SendRequest<LocationContainer>("textDocument/references", new ReferenceParams
            {
                Context = new ReferenceContext {IncludeDeclaration = true},
                PartialResultToken = WorkAndProgressTester.PartialResultToken,
                Position = new Position(2, 3),
                TextDocument = new TextDocumentIdentifier(new Uri(ModuleBFile)),
                WorkDoneToken = WorkAndProgressTester.WorkDoneToken
            }).Result;

            workTester.AssertProgressLogExpectations("DParserverTests.DReferencesHandlerTests.ExpectedProgress1");

            Assert.AreEqual("[]", JsonConvert.SerializeObject(result));
        }

        [Test]
        public void EnumReferences_DontIncludeDeclaration_ReturnsReferences()
        {
            Client.TextDocument.DidOpen(ModuleAFile, Lsp4DUtil.DLANG, @"module modA;
class AClass {}
class BClass {}

void bar() {
    auto a = new AClass();
}
");

            Client.TextDocument.DidOpen(ModuleBFile, Lsp4DUtil.DLANG, @"module modB;
import modA;
AClass ainstance;
void foo(T: AClass)() {}
");

            var partialResultToken = new ProgressToken("partialResult");
            var workdoneToken = new ProgressToken("workDone");

            var workTester = WorkAndProgressTester.Setup(Client);

            var result = Client.SendRequest<LocationContainer>("textDocument/references", new ReferenceParams
            {
                Context = new ReferenceContext {IncludeDeclaration = false},
                PartialResultToken = partialResultToken,
                Position = new Position(2, 3),
                TextDocument = new TextDocumentIdentifier(new Uri(ModuleBFile)),
                WorkDoneToken = workdoneToken
            }).Result;

            workTester.AssertProgressLogExpectations("DParserverTests.DReferencesHandlerTests.ExpectedProgress2");

            Assert.AreEqual("[]", JsonConvert.SerializeObject(result));
        }

        [Test]
        public void EnumReferences_NoProgressResult_ReturnsReferencesAsSingleResult()
        {
            Client.TextDocument.DidOpen(ModuleAFile, Lsp4DUtil.DLANG, @"module modA;
class AClass {}
class BClass {}

void bar() {
    auto a = new AClass();
}
");

            Client.TextDocument.DidOpen(ModuleBFile, Lsp4DUtil.DLANG, @"module modB;
import modA;
AClass ainstance;
void foo(T: AClass)() {}
");

            var result = Client.SendRequest<LocationContainer>("textDocument/references", new ReferenceParams
            {
                Context = new ReferenceContext {IncludeDeclaration = true},
                Position = new Position(2, 3),
                TextDocument = new TextDocumentIdentifier(new Uri(ModuleBFile))
            }).Result;

            WorkAndProgressTester.AssertResultEquality(result,
                "DParserverTests.DReferencesHandlerTests.ExpectedProgress3-result.json");
        }
    }
}