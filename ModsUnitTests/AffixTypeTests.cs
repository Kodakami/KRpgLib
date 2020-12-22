using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRpgLib.Affixes;
using System;

namespace ModsUnitTests
{
    [TestClass]
    public class AffixTypeTests
    {
        [TestMethod]
        public void Constructor_WithNullApplyNameDelegate_ThrowsArgNullEx()
        {
            const int NON_NEGATIVE_NUMBER = 1;

            static void exceptionalAction() => new AffixType(null, NON_NEGATIVE_NUMBER);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void Constructor_WithOutOfRangeMaxCount_ThrowsArgOutOfRangeEx()
        {
            const int NEGATIVE_NUMBER = -1;
            static string stubApplyNameFunc(string _, string __) => "";

            static void exceptionalAction() => new AffixType(stubApplyNameFunc, NEGATIVE_NUMBER);

            Assert.ThrowsException<ArgumentOutOfRangeException>(exceptionalAction);
        }
        [TestMethod]
        [DataRow(0, DisplayName = "With 0")]
        [DataRow(1, DisplayName = "With positive number (1)")]
        public void Constructor_WithValidInputs_DoesNotThrowExceptions(int maxAffixes)
        {
            static string stubApplyNameFunc(string _, string __) => "";

            var result = new AffixType(stubApplyNameFunc, maxAffixes);

            Assert.IsNotNull(result);
        }
        [TestMethod]
        [DataRow(null, "", DisplayName = "NULL, \"\"")]
        [DataRow("", null, DisplayName = "\"\", NULL")]
        public void ApplyName_WithNullArg_ThrowsArgNullEx(string affixName, string objectName)
        {
            const int NON_NEGATIVE = 1;
            static string stubApplyNameFunc(string _, string __) => "";
            var mockAffixType = new AffixType(stubApplyNameFunc, NON_NEGATIVE);

            void exceptionalAction() => mockAffixType.ApplyName(affixName, objectName);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void ApplyName_WithValidInputs_ReturnsCorrectResult()
        {
            const int NON_NEGATIVE = 1;
            static string stubApplyNameFunc(string aff, string obj) => $"{aff} {obj}";
            var mockAffixType = new AffixType(stubApplyNameFunc, NON_NEGATIVE);

            const string AFFIX_NAME = "a";
            const string OBJECT_NAME = "b";
            const string EXPECTED_RESULT = "a b";

            var result = mockAffixType.ApplyName(AFFIX_NAME, OBJECT_NAME);

            Assert.AreEqual(EXPECTED_RESULT, result);
        }
    }
}
