using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using KRpgLib.Mods;
using KRpgLib.Mods.ModTemplates;

namespace ModsUnitTests
{
    [TestClass]
    public class AffixTemplateTests
    {
        private static AffixType StubAffixType { get; } = new AffixType((aff, obj) => $"{aff} {obj}", 1);

        [TestMethod]
        public void Constructor_WithNullName_ThrowsArgNullEx()
        {
            var stubFlagMods = new List<AbstractMTFlag>();
            var stubStatDeltaMods = new List<MTStatDelta<int>>();

            void exceptionalAction() => new AffixTemplate<int>(null, StubAffixType, stubFlagMods, stubStatDeltaMods);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void Constructor_WithNullAffixType_ThrowsArgNullEx()
        {
            const string ANY_NAME = "";
            var stubFlagMods = new List<AbstractMTFlag>();
            var stubStatDeltaMods = new List<MTStatDelta<int>>();

            void exceptionalAction() => new AffixTemplate<int>(ANY_NAME, null, stubFlagMods, stubStatDeltaMods);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void Constructor_WithNullFlagMods_ThrowsArgNullEx()
        {
            const string ANY_NAME = "";
            var stubStatDeltaMods = new List<MTStatDelta<int>>();

            void exceptionalAction() => new AffixTemplate<int>(ANY_NAME, StubAffixType, null, stubStatDeltaMods);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void Constructor_WithNullStatDeltaMods_ThrowsArgNullEx()
        {
            const string ANY_NAME = "";
            var stubFlagMods = new List<AbstractMTFlag>();

            void exceptionalAction() => new AffixTemplate<int>(ANY_NAME, StubAffixType, stubFlagMods, null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
    }
}
