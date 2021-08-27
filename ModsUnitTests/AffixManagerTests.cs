using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRpgLib.Affixes;
using System;

namespace ModsUnitTests
{
    [TestClass]
    public static class AffixManagerTests
    {
        private static readonly Random StubRandom = new Random();
        private static readonly AffixType StubAffixType = new AffixType(1);
        private static ModdableDataManager GetStubDataManager() => new ModdableDataManager(new FakeModdableDataSet());

        [TestMethod]
        public static void CreateNewAffixInstance_WithValidArgs_ReturnsExpectedResult()
        {
            ModdableDataManager stubDataManager = GetStubDataManager();

            var mockTemplate = new FakeAffixTemplate(StubAffixType);
            var resultAffix = mockTemplate.CreateNewAffixInstance(StubRandom);
            resultAffix.Modify(stubDataManager);

            Assert.AreEqual(FakeModTemplateNoArgs.EXPECTED_VALUE, stubDataManager.GetKomponent<FakeModdableDataSet>().Value);
        }
    }
    public sealed class FakeAffixTemplate : AffixTemplate
    {
        public FakeAffixTemplate(AffixType affixType)
            :base("Test", affixType, new ModTemplateBase[] { new FakeModTemplateNoArgs() })
        { }
    }
}
