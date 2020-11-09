using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRpgLib.Stats;

namespace KRpgLibUnitTests.Stats
{
    [TestClass]
    public class StatLegalizerTests
    {
        [TestMethod]
        public void StatLegalizer_HasMinValue_LegalizesByMinValue()
        {
            LegalizationTest(new StatLegalizer(-15.4f, null, null), -15.4f, -15.5f);
        }
        [TestMethod]
        public void StatLegalizer_HasMaxValue_LegalizesByMaxValue()
        {
            LegalizationTest(new StatLegalizer(null, 99, null), 99, 99.9f);
        }
        [TestMethod]
        public void StatLegalizer_HasPrecision_LegalizesByPrecision()
        {
            LegalizationTest(new StatLegalizer(null, null, 0.5f), 3.5f, 3.59f);
        }
        private void LegalizationTest(StatLegalizer statLegalizer, float expected, float inputValue)
        {
            Assert.AreEqual(expected, statLegalizer.GetLegalizedValue(inputValue));
        }
    }
}
