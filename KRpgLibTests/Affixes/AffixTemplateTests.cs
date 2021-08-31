using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using KRpgLib.Affixes;
using KRpgLib.Affixes.AffixTypes;

namespace KRpgLibTests.Affixes
{
    [TestClass]
    public class AffixTemplateTests
    {
        private const string STUB_NAME = "";
        private static AffixType StubAffixType { get; } = new AffixType_AdHoc();
        private static IEnumerable<ModTemplate> StubModTemplates { get; } = new ModTemplate[0];

        [TestMethod]
        public void Ctor_WithNullName_ThrowsArgNullEx()
        {
            static void exceptionalAction() => new AffixTemplate(null, StubAffixType, StubModTemplates);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void Ctor_WithNullAffixType_ThrowsArgNullEx()
        {
            AffixType mockNullAffixType = null;

            void exceptionalAction() => new AffixTemplate(STUB_NAME, mockNullAffixType, StubModTemplates);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void Ctor_WithNullModTemplates_ThrowsArgNullEx()
        {
            IEnumerable<ModTemplate> mockNullModTemplates = null;

            void exceptionalAction() => new AffixTemplate(STUB_NAME, StubAffixType, mockNullModTemplates);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void Ctor_WithValidInputs_ReturnsValidInstance()
        {
            var mockAffixTemplate = new AffixTemplate(STUB_NAME, StubAffixType, StubModTemplates);

            Assert.IsNotNull(mockAffixTemplate);
        }
    }
}
