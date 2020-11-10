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
            var set = new TestStatSet();    //S1 = 13, S2 = 43
            var algo = new CompoundStatAlgorithm<int>(
                new Operation_Binary<int>(
                    new StatLiteral<int>(set.TestStat1, false),
                    CommonInstances.Int.Max,
                    new StatLiteral<int>(set.TestStat2, false)
                ));  //Greater value of Stat1 and Stat2 (Stat2 = 43 unlegalized)

            TestCompoundStatTemplate cs = new TestCompoundStatTemplate("Test Compound Stat", 0, 100, 2, algo);  //43 (42 legal)

            Assert.AreEqual(43, set.GetCompoundStatValue(cs));
        }
        [TestMethod]
        public void StatSet_ForCompoundStat_ReturnsCorrectValueLegalized()
        {
            var set = new TestStatSet();    //S1 = 13, S2 = 43
            var algo = new CompoundStatAlgorithm<int>(
                new Operation_Binary<int>(
                    new StatLiteral<int>(set.TestStat1, false),
                    CommonInstances.Int.Max,
                    new StatLiteral<int>(set.TestStat2, false)
                ));  //Greater value of Stat1 and Stat2 (Stat2 = 43 unlegalized)

            TestCompoundStatTemplate cs = new TestCompoundStatTemplate("Test Compound Stat", 0, 100, 2, algo);  //43 (42 legal)

            Assert.AreEqual(42, set.GetCompoundStatValueLegalized(cs));
        }
    }
}
