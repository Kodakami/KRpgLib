using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KRpgLibUnitTests.Stats
{
    [TestClass]
    public class StatSetTests_NoCompound
    {
        // Values.
        [TestMethod]
        public void StatSet_ForRepresentedStat_ReturnsStatValue()
        {
            TestStatSet set = new TestStatSet();
            Assert.AreEqual(79.1, set.GetStatValue(set.TestStat3, false), 0.001f);
        }
        [TestMethod]
        public void StatSet_ForRepresentedStat_ReturnsStatValueLegalized()
        {
            TestStatSet set = new TestStatSet();
            Assert.AreEqual(79, set.GetStatValue(set.TestStat3, true), 0.001f);
        }

        [TestMethod]
        public void StatSet_ForMissingStat_ReturnsDefaultValue()
        {
            TestStatSet set = new TestStatSet();
            Assert.AreEqual(1337.1, set.GetStatValue(new TestStatTemplate("Missing Stat", null, null, 1, 1337.1f), false), 0.001f);
        }
        [TestMethod]
        public void StatSet_ForMissingStat_ReturnsDefaultValueLegalized()
        {
            TestStatSet set = new TestStatSet();
            Assert.AreEqual(1337, set.GetStatValue(new TestStatTemplate("Missing Stat", null, null, 1, 1337.1f), true), 0.001f);
        }

        // Exceptions.
        [TestMethod]
        public void StatSet_ForNullStat_ThrowsArgNullEx()
        {
            TestStatSet set = new TestStatSet();
            Assert.ThrowsException<System.ArgumentNullException>(() => set.GetStatValue(null, false));
        }
    }
}
