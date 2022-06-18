using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRpgLib.Affixes;
using KRpgLib.Affixes.AffixTypes;
using System;

namespace KRpgLibTests.Affixes
{
    [TestClass]
    public static class AffixManagerTests
    {
        private static readonly Random StubRandom = new();
        private static readonly AffixType StubAffixType = new AffixType_AdHoc();    // Empty ctor places no restrictions.
        private static readonly ModTemplate[] StubModTemplates = Array.Empty<ModTemplate>();

        [TestMethod]
        public static void CreateNewAffixInstance_WithValidArgs_ReturnsExpectedResult()
        {
            var mockTemplate = new AffixTemplate(StubAffixType, StubModTemplates);
            var resultAffix = mockTemplate.CreateNewAffixInstance(StubRandom);

            var mockAffixManager = new AffixManager();
            mockAffixManager.AddAffix(resultAffix);

            Assert.IsTrue(mockAffixManager.HasAffix(mockTemplate));
        }
    }
}
