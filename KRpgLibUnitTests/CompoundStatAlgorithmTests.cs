using KRpgLib.Stats.Compound;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace KRpgLibUnitTests.Stats.Compound
{
    [TestClass]
    public class CompoundStatAlgorithmTests
    {
        [TestMethod]
        public void CalculateValue_WithValidAlgorithm_ReturnsCorrectValue()
        {
            var stubSet = new FakeStatSet();
            var stubExpression = new FakeValueExpression(0);
            var mockAlgorithm = new CompoundStatAlgorithm<int>(stubExpression);

            var resultValue = mockAlgorithm.CalculateValue(stubSet);

            Assert.AreEqual(0, resultValue);
        }
        [TestMethod]
        public void Constructor_WithNullAlgorithm_ThrowsArgNullEx()
        {
            FakeValueExpression stubNullExpression = null;

            void exceptionalAction() => new CompoundStatAlgorithm<int>(stubNullExpression);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void CalculateValue_WithNullStatSet_ThrowsArgNullEx()
        {
            var stubValueExpression = new FakeValueExpression(0);
            var mockAlgorithm = new CompoundStatAlgorithm<int>(stubValueExpression);
            FakeStatSet stubNullSet = null;

            void exceptionalAction() => mockAlgorithm.CalculateValue(stubNullSet);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
    }
}
