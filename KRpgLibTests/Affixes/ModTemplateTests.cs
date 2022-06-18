using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRpgLib.Affixes;
using KRpgLib.Utility.KomponentObject;
using System;
using System.Collections.Generic;
using KRpgLib.Utility.Serialization;

namespace KRpgLibTests.Affixes
{
    [TestClass]
    public static class ModTemplateTests
    {
        private static readonly Random _stubRandom = new();
        private static readonly FakeModArgType _stubArgType = new();

        [TestClass]
        public static class ModTemplateNoArgTests
        {
            [TestMethod]
            public static void Modify_WithValidInput_ReturnsExpectedResult()
            {
                var mockModTemplate = new FakeModTemplateNoArgs();

                Mod resultMod = mockModTemplate.CreateNewModInstance(_stubRandom);

                var stubEffectCollection = new ModEffectCollection(new IModEffect[] { resultMod.GetModEffect() });
                var result = new List<FakeModEffect>(stubEffectCollection.GetModEffects<FakeModEffect>())[0];

                Assert.AreEqual(0, result.Value);
            }
        }
        [TestClass]
        public static class ModTemplateWithArgTests
        {
            [TestMethod]
            public static void Modify_WithValidInput_ReturnsExpectedResult()
            {
                var mockModTemplate = new FakeModTemplateWithArgs(_stubArgType);

                Mod resultMod = mockModTemplate.CreateNewModInstance(_stubRandom);

                var stubEffectCollection = new ModEffectCollection(new IModEffect[] { resultMod.GetModEffect() });
                var result = new List<FakeModEffect>(stubEffectCollection.GetModEffects<FakeModEffect>())[0];

                Assert.AreEqual(FakeModTemplateWithArgs.ASSIGNED_VALUE, result.Value);
            }
        }

    }
    public sealed class FakeModEffect : IModEffect
    {
        public int Value { get; set; } = 0;
    }
    public sealed class FakeModTemplateNoArgs : ModTemplate_NoArg
    {
        protected override IModEffect GetModEffect_Internal(Mod safeModInstance)
        {
            return new FakeModEffect();
        }
    }
    public sealed class FakeModTemplateWithArgs : ModTemplate<int>
    {
        public const int ASSIGNED_VALUE = 1;

        public FakeModTemplateWithArgs(ModArgType<int> argType)
            :base(argType)
        { }

        protected override IModEffect GetModEffect_Internal(Mod safeModInstance)
        {
            return new FakeModEffect();
        }

        protected override int GetNewArgStrongValue(Random rng)
        {
            // Dodge random for testing.

            return ASSIGNED_VALUE;
        }
    }
    public sealed class FakeModArgType : ModArgType<int>
    {
        public FakeModArgType()
            :base(Int32Serializer.Singleton)
        { }
    }
}
