using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using KRpgLib.Stats.Compound;

namespace KRpgLibUnitTests.Stats.Compound
{
    [TestClass]
    public class StatSetTests_Compound
    {
        [TestMethod]
        public void StatSet_ForCompoundStat_ReturnsCorrectValue()
        {
            var set = new TestStatSet();    //S1 = 15.1 (15 legal)

            TestAlgorithmStep step = new TestAlgorithmStep();   //+1
            var algo = new CompoundStatAlgorithm(new StatLiteral(set.TestStat1, false), step);  //15.1 + 1 = 16.1

            TestCompoundStat cs = new TestCompoundStat("Test Compound Stat", 0, 100, 3, algo);  //(16 legal)

            // TODO: Test fails because of loss of floating-point precision.

            Assert.AreEqual(16.1, set.GetCompoundStatValue(cs, false));
        }
        [TestMethod]
        public void StatSet_ForCompoundStat_ReturnsCorrectValueLegalized()
        {
            var set = new TestStatSet();

            TestAlgorithmStep step = new TestAlgorithmStep();
            var algo = new CompoundStatAlgorithm(new StatLiteral(set.TestStat1, false), step);

            TestCompoundStat cs = new TestCompoundStat("Test Compound Stat", 0, 100, 3, algo);

            Assert.AreEqual(15, set.GetCompoundStatValue(cs, true));
        }
    }
}
