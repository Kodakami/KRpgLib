using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRpgLib.Affixes;
using KRpgLib.Utility.KomponentObject;
using System;

namespace ModsUnitTests
{
    [TestClass]
    public static class ModTemplateTests
    {
        private static readonly Random _stubRandom = new Random();
        private static ModdableDataManager GetStubModdableDataManager() => new ModdableDataManager(new FakeModdableDataSet());

        [TestClass]
        public static class ModTemplateNoArgTests
        {
            [TestMethod]
            public static void Modify_WithValidInput_ReturnsExpectedResult()
            {
                var mockModTemplate = new FakeModTemplateNoArgs();
                var stubManager = GetStubModdableDataManager();

                ModBase resultMod = mockModTemplate.CreateNewModInstance(_stubRandom);
                resultMod.Modify(stubManager);

                var resultDataSet = stubManager.GetKomponent<FakeModdableDataSet>();

                Assert.AreEqual(FakeModTemplateNoArgs.EXPECTED_VALUE, resultDataSet.Value);
            }
        }
        [TestClass]
        public static class ModTemplateWithArgTests
        {
            [TestMethod]
            public static void Modify_WithValidInput_ReturnsExpectedResult()
            {
                var mockModTemplate = new FakeModTemplateWithArgs();
                var stubManager = GetStubModdableDataManager();

                ModBase resultMod = mockModTemplate.CreateNewModInstance(_stubRandom);
                resultMod.Modify(stubManager);

                var resultDataSet = stubManager.GetKomponent<FakeModdableDataSet>();

                Assert.AreEqual(FakeModTemplateWithArgs.EXPECTED_VALUE, resultDataSet.Value);
            }
        }

    }
    public sealed class FakeModdableDataSet : IModdableDataSet
    {
        public const int INITIAL_VALUE = 0;
        public int Value { get; set; } = INITIAL_VALUE;
    }
    public sealed class FakeModTemplateNoArgs : ModTemplate<FakeModdableDataSet>
    {
        public const int EXPECTED_VALUE = 1;
        protected override void Modify_Internal(FakeModdableDataSet safeDataSet)
        {
            safeDataSet.Value = EXPECTED_VALUE;
        }
    }
    public sealed class FakeModTemplateWithArgs : ModTemplate<FakeModdableDataSet, int>
    {
        public const int EXPECTED_VALUE = 2;

        public override int GetNewArg(Random rng)
        {
            // Dodge random for testing purposes.
            return EXPECTED_VALUE;
        }

        protected override void Modify_Internal(FakeModdableDataSet safeDataSet, int argValue)
        {
            safeDataSet.Value = argValue;
        }
    }
}
