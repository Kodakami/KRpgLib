using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRpgLib.Affixes;
using KRpgLib.Utility.KomponentObject;
using System;

namespace KRpgLib.UnitTests.AffixesTests
{
    [TestClass]
    public static class ModTemplateTests
    {
        private static readonly Random _stubRandom = new Random();

        [TestClass]
        public static class ModTemplateNoArgTests
        {
            [TestMethod]
            public static void Modify_WithValidInput_ReturnsExpectedResult()
            {
                var mockModTemplate = new FakeModTemplateNoArgs();

                Mod resultMod = mockModTemplate.CreateNewModInstance(_stubRandom);

                var stubEffectCollection = new ModEffectCollection(new IModEffect[] { resultMod.GetModEffect() });

                Assert.AreEqual(FakeModTemplateNoArgs.EXPECTED_VALUE, stubEffectCollection.GetKomponent<FakeModEffect>().Value);
            }
        }
        [TestClass]
        public static class ModTemplateWithArgTests
        {
            [TestMethod]
            public static void Modify_WithValidInput_ReturnsExpectedResult()
            {
                var mockModTemplate = new FakeModTemplateWithArgs();

                Mod resultMod = mockModTemplate.CreateNewModInstance(_stubRandom);

                var stubEffectCollection = new ModEffectCollection(new IModEffect[] { resultMod.GetModEffect() });

                Assert.AreEqual(FakeModTemplateWithArgs.ASSIGNED_VALUE, stubEffectCollection.GetKomponent<FakeModEffect>().Value);
            }
        }

    }
    public sealed class FakeModEffect : IModEffect
    {
        public int Value { get; set; } = 0;
    }
    public sealed class FakeModTemplateNoArgs : ModTemplate
    {
        public const int EXPECTED_VALUE = 1;

        public override IModEffect GetModEffect(Mod modInstance)
        {
            // Ignore internal arg value.

            return new FakeModEffect() { Value = EXPECTED_VALUE };
        }
    }
    public sealed class FakeModTemplateWithArgs : ModTemplate<int>
    {
        public const int ASSIGNED_VALUE = 1;

        public override IModEffect GetModEffect(Mod<int> modInstance)
        {
            return new FakeModEffect() { Value = modInstance.Arg };
        }

        public override int GetNewArg(Random rng)
        {
            // Dodge random for testing.

            return ASSIGNED_VALUE;
        }
    }
}
