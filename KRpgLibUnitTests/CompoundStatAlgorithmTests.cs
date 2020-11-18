using KRpgLib.Stats.Compound;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace KRpgLibUnitTests.Stats.Compound
{
    [TestClass]
    public class CompoundStatAlgorithmTests
    {
        [TestMethod]
        public void CompoundStatAlgorithm_WhenConstructing_ReturnsFunctionalAlgorithm()
        {
            var set = new TestStatSet();
            var algo = new CompoundStatAlgorithm<int>(new TestExpression());    //33

            Assert.AreEqual(33, algo.CalculateValue(set));
        }
        [TestMethod]
        public void CompoundStatAlgorithm_WhenConstructingWithNullExpression_ThrowsArgNullEx()
        {
            TestExpression exp = null;                          //null
            Assert.ThrowsException<ArgumentNullException>(() => new CompoundStatAlgorithm<int>(exp));
        }
        [TestMethod]
        public void CompoundStatAlgorithm_WhenCalculatingValueWithNullStatSet_ThrowsArgNullEx()
        {
            var algo = new CompoundStatAlgorithm<int>(new TestExpression());    //33
            TestStatSet set = null;

            Assert.ThrowsException<ArgumentNullException>(() => algo.CalculateValue(set));
        }
    }
}
