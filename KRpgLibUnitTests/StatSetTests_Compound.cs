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
            var set = new TestStatSet();    //S1 = 13

            TestAlgorithmStep step = new TestAlgorithmStep();   //+23
            var algo = new CompoundStatAlgorithm<int>(new StatLiteral<int>(set.TestStat1, false), step);  //13 + 23 = 36

            TestCompoundStat cs = new TestCompoundStat("Test Compound Stat", 0, 100, 1, algo);  //36 (36 legal)

            Assert.AreEqual(36, set.GetCompoundStatValue(cs));
        }
        [TestMethod]
        public void StatSet_ForCompoundStat_ReturnsCorrectValueLegalized()
        {
            var set = new TestStatSet();

            TestAlgorithmStep step = new TestAlgorithmStep();
            var algo = new CompoundStatAlgorithm<int>(new StatLiteral<int>(set.TestStat1, false), step);

            TestCompoundStat cs = new TestCompoundStat("Test Compound Stat", 0, 100, 5, algo);  //36 (35 legal)

            Assert.AreEqual(35, set.GetCompoundStatValueLegalized(cs));
        }
    }
}
