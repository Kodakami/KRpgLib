using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRpgLib.Affixes;
using System;

namespace KRpgLib.UnitTests.AffixesTests
{
    [TestClass]
    public class AffixTypeTests
    {
        [TestMethod]
        public void Constructor_WithOutOfRangeMaxCount_ThrowsArgOutOfRangeEx()
        {
            const int NEGATIVE_NUMBER = -1;

            static void exceptionalAction() => new AffixType(NEGATIVE_NUMBER);

            Assert.ThrowsException<ArgumentOutOfRangeException>(exceptionalAction);
        }
        [TestMethod]
        [DataRow(0, DisplayName = "With 0")]
        [DataRow(1, DisplayName = "With positive number (1)")]
        public void Constructor_WithValidInput_DoesNotThrowExceptions(int maxAffixes)
        {
            var result = new AffixType(maxAffixes);

            Assert.IsNotNull(result);
        }
    }
}
