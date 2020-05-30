using DParserverTests.Util;
using NUnit.Framework;

namespace DParserverTests
{
    public class InitServerTests : LspTest
    {
        [Test]
        public void StartsServer()
        {
            Assert.IsNotNull(Client.ServerCapabilities.CompletionProvider);
        }
    }
}