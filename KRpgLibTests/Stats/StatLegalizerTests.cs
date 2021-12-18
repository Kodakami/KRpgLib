using KRpgLib.Stats;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KRpgLibTests.Stats
{
    [TestClass]
    public class StatLegalizerTests
    {
        [TestMethod]
        public void GetLegalizedValue_RawValueLessThanMin_ReturnsMinValue()
        {
            var mockLegalizer = new StatLegalizer(1, null, null);

            LegalizationTest(mockLegalizer, 1, 0);
        }
        [TestMethod]
        public void GetLegalizedValue_RawValueGreaterThanMax_ReturnsMaxValue()
        {
            var mockLegalizer = new StatLegalizer(null, 0, null);

            LegalizationTest(mockLegalizer, 0, 1);
        }
        [TestMethod]
        public void GetLegalizedValue_RawValueOutsideOfPrecision_ReturnsClosestLegalValue()
        {
            var mockLegalizer = new StatLegalizer(null, null, 2);

            LegalizationTest(mockLegalizer, 2, 3);
        }
        private static void LegalizationTest(StatLegalizer statLegalizer, int expected, int inputValue)
        {
            var resultValue = statLegalizer.GetLegalizedValue(inputValue);

            Assert.AreEqual(expected, resultValue);
        }
    }
}
