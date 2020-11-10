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

            var algo = new CompoundStatAlgorithm<int>(step);
            var set = new TestStatSet();

            Assert.AreEqual(23, algo.CalculateValue(set));
        }
        [TestMethod]
        public void CompoundStatAlgorithm_WhenConstructingWithInitialExpression_ReturnsFunctionalAlgorithm()
        {
            TestExpression exp = new TestExpression();          //33
            TestAlgorithmStep step = new TestAlgorithmStep();   //+23
            var set = new TestStatSet();

            Assert.AreEqual(56, new CompoundStatAlgorithm<int>(exp, step).CalculateValue(set));
        }
        [TestMethod]
        public void CompoundStatAlgorithm_WhenConstructingWithNullInitializer_ThrowsArgNullEx()
        {
            TestExpression exp = null;                          //null
            TestAlgorithmStep step = new TestAlgorithmStep();   //+23

            Assert.ThrowsException<ArgumentNullException>(() => new CompoundStatAlgorithm<int>(exp, step));
        }
        [TestMethod]
        public void CompoundStatAlgorithm_WhenConstructingWithNullStep_ThrowsArgNullEx()
        {
            TestAlgorithmStep nullStep = null;
            Assert.ThrowsException<ArgumentNullException>(() => new CompoundStatAlgorithm<int>(nullStep));
        }
        [TestMethod]
        public void CompoundStatAlgorithm_WhenCalculatingValueWithNullStatSet_ThrowsArgNullEx()
        {
            TestAlgorithmStep step = new TestAlgorithmStep();

            var algo = new CompoundStatAlgorithm<int>(step);
            TestStatSet set = null;

            Assert.ThrowsException<ArgumentNullException>(() => algo.CalculateValue(set));
        }
    }
}
