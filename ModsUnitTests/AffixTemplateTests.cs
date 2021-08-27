using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using KRpgLib.Affixes;

namespace ModsUnitTests
{
    [TestClass]
    public class AffixTemplateTests
    {
        private const int STUB_MAX_AFFIXES_OF_TYPE = 1;
        private const string STUB_NAME = "";
        private static AffixType StubAffixType { get; } = new AffixType(STUB_MAX_AFFIXES_OF_TYPE);
        private static IEnumerable<ModTemplateBase> StubModTemplates { get; } = new ModTemplateBase[0];

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
            IEnumerable<ModTemplateBase> mockNullModTemplates = null;

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
