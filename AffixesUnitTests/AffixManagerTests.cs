using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRpgLib.Affixes;
using System;

namespace KRpgLib.UnitTests.AffixesTests
{
    [TestClass]
    public static class AffixManagerTests
    {
        private static readonly Random StubRandom = new Random();
        private static readonly AffixType StubAffixType = new AffixType(1);

        [TestMethod]
        public static void CreateNewAffixInstance_WithValidArgs_ReturnsExpectedResult()
        {
            var mockTemplate = new FakeAffixTemplate(StubAffixType);
            var resultAffix = mockTemplate.CreateNewAffixInstance(StubRandom);

            var mockAffixManager = new AffixManager();
            mockAffixManager.AddAffix(resultAffix);

            Assert.AreEqual(FakeModTemplateNoArgs.EXPECTED_VALUE, mockAffixManager.HasAffix(mockTemplate));
        }
    }
    public sealed class FakeAffixTemplate : AffixTemplate
    {
        public FakeAffixTemplate(AffixType affixType)
            :base("Test", affixType, new ModTemplate[] { new FakeModTemplateNoArgs() })
        { }
    }
}
