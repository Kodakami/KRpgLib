using KRpgLib.Stats.Compound;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KRpgLibUnitTests.Stats.Compound
{
    [TestClass]
    public class StatSetTests_Compound
    {
        private const int RAW_VALUE = 3;
        private const int PRECISION = 2;
        private const int LEGAL_VALUE = 2;

        private void CompoundStatValueTest(System.Func<KRpgLib.Stats.AbstractStatSet<int>, ICompoundStatTemplate<int> , int> testFunc, int expectedValue)
        {
            var mockSet = new FakeStatSet();
            var stubValueExpression = new FakeValueExpression(RAW_VALUE);
            var algorithm = new CompoundStatAlgorithm<int>(stubValueExpression);
            var stubCompoundStatTemplate = new FakeCompoundStatTemplate(null, null, PRECISION, algorithm);

            var resultValue = testFunc(mockSet, stubCompoundStatTemplate);

            Assert.AreEqual(expectedValue, resultValue);
        }
        [TestMethod]
        public void GetCompoundStatValue_WithValidAlgorithm_ReturnsCorrectValue()
        {
            CompoundStatValueTest((set, template) => set.GetCompoundStatValue(template), RAW_VALUE);
        }
        [TestMethod]
        public void GetCompoundStatValueLegalized_WithValidAlgorithm_ReturnsCorrectValue()
        {
            CompoundStatValueTest((set, template) => set.GetCompoundStatValueLegalized(template), LEGAL_VALUE);
        }
    }
}
