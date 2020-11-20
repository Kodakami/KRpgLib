using KRpgLib.Stats;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KRpgLibUnitTests.Stats
{
    [TestClass]
    public class StatLegalizerTests
    {
        [TestMethod]
        public void GetLegalizedValue_RawValueLessThanMin_ReturnsMinValue()
        {
            var mockLegalizer = new StatLegalizer_Int(1, null, null);

            LegalizationTest(mockLegalizer, 1, 0);
        }
        [TestMethod]
        public void GetLegalizedValue_RawValueGreaterThanMax_ReturnsMaxValue()
        {
            var mockLegalizer = new StatLegalizer_Int(null, 0, null);

            LegalizationTest(mockLegalizer, 0, 1);
        }
        [TestMethod]
        public void GetLegalizedValue_RawValueOutsideOfPrecision_ReturnsClosestLegalValue()
        {
            var mockLegalizer = new StatLegalizer_Int(null, null, 2);

            LegalizationTest(mockLegalizer, 2, 3);
        }
        private void LegalizationTest<TValue>(AbstractStatLegalizer<TValue> statLegalizer, TValue expected, TValue inputValue) where TValue : struct
        {
            var resultValue = statLegalizer.GetLegalizedValue(inputValue);

            Assert.AreEqual(expected, resultValue);
        }
    }
}
