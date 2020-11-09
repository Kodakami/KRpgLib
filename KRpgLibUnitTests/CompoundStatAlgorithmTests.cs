using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using KRpgLib.Stats.Compound;

namespace KRpgLibUnitTests.Stats.Compound
{
    [TestClass]
    public class CompoundStatAlgorithmTests
    {
        [TestMethod]
        public void CompoundStatAlgorithm_WhenConstructing_ReturnsFunctionalAlgorithm()
        {
            TestAlgorithmStep step = new TestAlgorithmStep();

            var algo = new CompoundStatAlgorithm(step);
            var set = new TestStatSet();

            Assert.AreEqual(1, algo.CalculateValue(set));
        }
        [TestMethod]
        public void CompoundStatAlgorithm_WhenConstructingWithInitialExpression_ReturnsFunctionalAlgorithm()
        {
            TestExpression exp = new TestExpression();
            TestAlgorithmStep step = new TestAlgorithmStep();
            var set = new TestStatSet();

            Assert.AreEqual(34, new CompoundStatAlgorithm(exp, step).CalculateValue(set));
        }
        [TestMethod]
        public void CompoundStatAlgorithm_WhenConstructingWithNullInitializer_ThrowsArgNullEx()
        {
            TestExpression exp = null;
            TestAlgorithmStep step = new TestAlgorithmStep();

            Assert.ThrowsException<ArgumentNullException>(() => new CompoundStatAlgorithm(exp, step));
        }
        [TestMethod]
        public void CompoundStatAlgorithm_WhenConstructingWithNullStep_ThrowsArgNullEx()
        {
            TestAlgorithmStep nullStep = null;
            Assert.ThrowsException<ArgumentNullException>(() => new CompoundStatAlgorithm(nullStep));
        }
        [TestMethod]
        public void CompoundStatAlgorithm_WhenCalculatingValueWithNullStatSet_ThrowsArgNullEx()
        {
            TestAlgorithmStep step = new TestAlgorithmStep();

            var algo = new CompoundStatAlgorithm(step);
            TestStatSet set = null;

            Assert.ThrowsException<ArgumentNullException>(() => algo.CalculateValue(set));
        }
    }
}
