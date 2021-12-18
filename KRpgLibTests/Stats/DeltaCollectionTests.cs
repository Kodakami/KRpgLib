using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using KRpgLib.Stats;
using System;

namespace KRpgLibTests.Stats
{
    [TestClass]
    public class DeltaCollectionTests
    {
        private static DeltaType StubDeltaType1;
        private static DeltaType StubDeltaType2;
        private static readonly Stat StubStat1 = new FakeStat(0, 0, 1, 1);
        private static readonly Stat StubStat2 = new FakeStat(0, 0, 1, 1);

        [TestInitialize]
        public void TestInitialize()
        {
            // Simple addition
            static int deltaTypeFunc(int left, int right) => left + right;

            // Delta Types
            StubDeltaType1 = new DeltaType(deltaTypeFunc, 0);
            StubDeltaType2 = new DeltaType(deltaTypeFunc, 0);

            // Register
            var builder = new StatEnvironmentBuilder();
            builder.RegisterDeltaType(0, StubDeltaType1);
            builder.RegisterDeltaType(0, StubDeltaType2);
            builder.Build(true);
        }

        [TestMethod]
        public void ManualCtor_WithNullCollection_ReturnsEmptyInstance()
        {
            IEnumerable<StatDelta> stubNullCollection = null;

            var mockDeltaCollection = new DeltaCollection(stubNullCollection);

            Assert.AreEqual(0, mockDeltaCollection.CountValues());
        }
        [TestMethod]
        public void ManualCtor_WithDefaultValueInCollection_IgnoresValue()
        {
            const int DEFAULT_VALUE = 0;
            const int EXPECTED_COUNT = 0;

            var stubStatTemplateAndDelta = new StatDelta(StubStat1, new Delta(StubDeltaType1, DEFAULT_VALUE));

            var mockDeltaCollection = new DeltaCollection(new StatDelta[] { stubStatTemplateAndDelta });
            int resultCount = mockDeltaCollection.CountValues();

            Assert.AreEqual(EXPECTED_COUNT, resultCount);
        }
        [TestMethod]
        public void CombineCtor_WithNullSuperCollection_ReturnsEmptyInstance()
        {
            IEnumerable<DeltaCollection> stubNullSuperCollection = null;

            const int EXPECTED_COUNT = 0;

            var mockDeltaCollection = new DeltaCollection(stubNullSuperCollection);
            int resultCount = mockDeltaCollection.CountValues();

            Assert.AreEqual(EXPECTED_COUNT, resultCount);
        }
        [TestMethod]
        public void CombineCtor_WithDifferentStatTemplatesAndSameDeltaType_ReturnsCorrectResult()
        {
            const int STAT_1_VALUE = 1;
            const int STAT_2_VALUE = 2;

            var stubSuperCollection = new List<DeltaCollection>()
            {
                new DeltaCollection(new StatDelta(StubStat1, StubDeltaType1, STAT_1_VALUE)),
                new DeltaCollection(new StatDelta(StubStat2, StubDeltaType1, STAT_2_VALUE)),
            };

            var mockDeltaCollection = new DeltaCollection(stubSuperCollection);
            int result1 = mockDeltaCollection.GetDeltaValue(StubStat1, StubDeltaType1);
            int result2 = mockDeltaCollection.GetDeltaValue(StubStat2, StubDeltaType1);

            Assert.AreEqual(STAT_1_VALUE, result1);
            Assert.AreEqual(STAT_2_VALUE, result2);
        }
        [TestMethod]
        public void CombineCtor_WithSameStatTemplateAndDifferentDeltaTypes_ReturnsCorrectResult()
        {
            const int DELTA_TYPE_1_VALUE = 1;
            const int DELTA_TYPE_2_VALUE = 2;

            var stubSuperCollection = new List<DeltaCollection>()
            {
                new DeltaCollection(new StatDelta(StubStat1, StubDeltaType1, DELTA_TYPE_1_VALUE)),
                new DeltaCollection(new StatDelta(StubStat1, StubDeltaType2, DELTA_TYPE_2_VALUE)),
            };

            var mockDeltaCollection = new DeltaCollection(stubSuperCollection);
            int result1 = mockDeltaCollection.GetDeltaValue(StubStat1, StubDeltaType1);
            int result2 = mockDeltaCollection.GetDeltaValue(StubStat1, StubDeltaType2);

            Assert.AreEqual(DELTA_TYPE_1_VALUE, result1);
            Assert.AreEqual(DELTA_TYPE_2_VALUE, result2);
        }
        [TestMethod]
        public void CombineCtor_WithSameStatTemplateAndSameDeltaType_ReturnsCorrectResult()
        {
            const int VALUE_1 = 1;
            const int VALUE_2 = 2;
            const int EXPECTED_RESULT = VALUE_1 + VALUE_2;

            var stubSuperCollection = new List<DeltaCollection>()
            {
                new DeltaCollection(new StatDelta(StubStat1, StubDeltaType1, VALUE_1)),
                new DeltaCollection(new StatDelta(StubStat1, StubDeltaType1, VALUE_2)),
            };

            var mockDeltaCollection = new DeltaCollection(stubSuperCollection);
            int result = mockDeltaCollection.GetDeltaValue(StubStat1, StubDeltaType1);

            Assert.AreEqual(EXPECTED_RESULT, result);
        }
        [TestMethod]
        public void GetDeltaValue_WithMissingValue_ReturnsDefault()
        {
            const int DEFAULT_VALUE = default;

            var mockEmptyDeltaCollection = new DeltaCollection();
            var result = mockEmptyDeltaCollection.GetDeltaValue(StubStat1, StubDeltaType1);

            Assert.AreEqual(DEFAULT_VALUE, result);
        }
    }
}
