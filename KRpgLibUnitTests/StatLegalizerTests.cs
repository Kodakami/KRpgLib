using KRpgLib.Stats;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KRpgLibUnitTests.Stats
{
    [TestClass]
    public class StatLegalizerTests
    {
        [TestMethod]
        public void StatLegalizer_HasMinValue_LegalizesByMinValue()
        {
            LegalizationTest(new StatLegalizer_Int(-15, null, null), -15, -16);
        }
        [TestMethod]
        public void StatLegalizer_HasMaxValue_LegalizesByMaxValue()
        {
            LegalizationTest(new StatLegalizer_Int(null, 99, null), 99, 100);
        }
        [TestMethod]
        public void StatLegalizer_HasPrecision_LegalizesByPrecision()
        {
            LegalizationTest(new StatLegalizer_Int(null, null, 3), 3, 5);
        }
        private void LegalizationTest<TValue>(AbstractStatLegalizer<TValue> statLegalizer, TValue expected, TValue inputValue) where TValue : struct
        {
            Assert.AreEqual(expected, statLegalizer.GetLegalizedValue(inputValue));
        }
    }
}
