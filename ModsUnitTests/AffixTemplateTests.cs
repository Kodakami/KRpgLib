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
        private static AffixType StubAffixType { get; } = new AffixType(STUB_MAX_AFFIXES_OF_TYPE);

        [TestMethod]
        public void Constructor_WithNullName_ThrowsArgNullEx()
        {
            var stubModTemplates = new ModTemplateBase[0];

            void exceptionalAction() => new AffixTemplate(null, StubAffixType, stubModTemplates);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
    }
}
