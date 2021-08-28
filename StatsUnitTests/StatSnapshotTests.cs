using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KRpgLib.UnitTests.StatsTests
{
    [TestClass]
    public class StatSnapshotTests
    {
        // Values.
        [TestMethod]
        public void GetStatValue_ForProvidedStat_ReturnsCorrectValue()
        {
            var mock = new FakeStatSet().Snapshot;
            const int RAW_VALUE = 3;

            var resultValue = mock.GetStatValue(FakeStatSet.TestStat_Raw3_Legal2_Provided);

            Assert.AreEqual(RAW_VALUE, resultValue);
        }
        [TestMethod]
        public void GetStatValueLegalized_ForRepresentedStat_ReturnsCorrectValue()
        {
            var mock = new FakeStatSet().Snapshot;
            const int LEGAL_VALUE = 2;

            var resultValue = mock.GetStatValueLegalized(FakeStatSet.TestStat_Raw3_Legal2_Provided);

            Assert.AreEqual(LEGAL_VALUE, resultValue);
        }

        [TestMethod]
        public void GetStatValue_ForMissingStat_ReturnsDefaultValue()
        {
            var mockSet = new FakeStatSet().Snapshot;

            var resultValue = mockSet.GetStatValue(FakeStatSet.TestStat_Default7_Legal6_Missing);
            const int DEFAULT_VALUE = 7;

            Assert.AreEqual(DEFAULT_VALUE, resultValue);
        }
        [TestMethod]
        public void GetStatValueLegalized_ForMissingStat_ReturnsDefaultValue()
        {
            var mockSet = new FakeStatSet().Snapshot;

            var resultValue = mockSet.GetStatValueLegalized(FakeStatSet.TestStat_Default7_Legal6_Missing);
            const int LEGAL_VALUE = 6;

            Assert.AreEqual(LEGAL_VALUE, resultValue);
        }

        // Exceptions.
        [TestMethod]
        public void GetStatValue_ForNullStat_ThrowsArgNullEx()
        {
            var mock = new FakeStatSet().Snapshot;

            void exceptionalAction() => mock.GetStatValue(null);
            
            Assert.ThrowsException<System.ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void GetStatValueLegalized_ForNullStat_ThrowsArgNullEx()
        {
            var mock = new FakeStatSet().Snapshot;

            void exceptionalAction() => mock.GetStatValueLegalized(null);

            Assert.ThrowsException<System.ArgumentNullException>(exceptionalAction);
        }
    }
}
