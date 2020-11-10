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
            Assert.AreEqual(79, set.GetStatValue(set.TestStat3));
        }
        [TestMethod]
        public void StatSet_ForRepresentedStat_ReturnsStatValueLegalized()
        {
            TestStatSet set = new TestStatSet();
            Assert.AreEqual(78, set.GetStatValueLegalized(set.TestStat3));
        }

        [TestMethod]
        public void StatSet_ForMissingStat_ReturnsDefaultValue()
        {
            TestStatSet set = new TestStatSet();
            Assert.AreEqual(1337, set.GetStatValue(new TestStatTemplate("Missing Stat", null, null, 1, 1337)));
        }
        [TestMethod]
        public void StatSet_ForMissingStat_ReturnsDefaultValueLegalized()
        {
            TestStatSet set = new TestStatSet();
            Assert.AreEqual(1336, set.GetStatValueLegalized(new TestStatTemplate("Missing Stat", null, null, 2, 1337)));
        }

        // Exceptions.
        [TestMethod]
        public void StatSet_ForNullStat_ThrowsArgNullEx()
        {
            TestStatSet set = new TestStatSet();
            Assert.ThrowsException<System.ArgumentNullException>(() => set.GetStatValue(null));
        }
        [TestMethod]
        public void StatSet_ForNullStatLegalized_ThrowsArgNullEx()
        {
            TestStatSet set = new TestStatSet();
            Assert.ThrowsException<System.ArgumentNullException>(() => set.GetStatValueLegalized(null));
        }
    }
}
