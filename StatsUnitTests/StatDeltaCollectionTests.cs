using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using KRpgLib.Stats;
using System;

namespace KRpgLib.UnitTests.StatsTests
{
    [TestClass]
    public class StatDeltaCollectionTests
    {
        private static StatDeltaType<int> StubDeltaType1;
        private static StatDeltaType<int> StubDeltaType2;
        private static readonly IStatTemplate<int> StubStatTemplate1 = new FakeStatTemplate(0, 1, 1, 0);
        private static readonly IStatTemplate<int> StubStatTemplate2 = new FakeStatTemplate(0, 1, 1, 0);

        [TestInitialize]
        public void TestInitialize()
        {
            // Simple addition.
            int deltaTypeFunc(int left, int right) => left + right;
            int combineFunc(int left, int right) => left + right;
            StubDeltaType1 = StatDeltaType<int>.RegisterStatDeltaType(deltaTypeFunc, 0, combineFunc, 0);
            StubDeltaType2 = StatDeltaType<int>.RegisterStatDeltaType(deltaTypeFunc, 0, combineFunc, 0);
        }
        [TestCleanup]
        public void TestCleanup()
        {
            StatDeltaType<int>.UnregisterStatDeltaType(StubDeltaType1);
        }

        [TestMethod]
        public void ManualCtor_WithNullCollection_ReturnsEmptyInstance()
        {
            IEnumerable<StatTemplateAndDelta<int>> stubNullCollection = null;

            var mockDeltaCollection = new StatDeltaCollection<int>(stubNullCollection);

            Assert.AreEqual(0, mockDeltaCollection.CountValues());
        }
        [TestMethod]
        public void ManualCtor_WithDefaultValueInCollection_IgnoresValue()
        {
            const int DEFAULT_VALUE = 0;
            const int EXPECTED_COUNT = 0;

            var stubStatTemplateAndDelta = new StatTemplateAndDelta<int>(StubStatTemplate1, new StatDelta<int>(StubDeltaType1, DEFAULT_VALUE));

            var mockDeltaCollection = new StatDeltaCollection<int>(new StatTemplateAndDelta<int>[] { stubStatTemplateAndDelta });
            int resultCount = mockDeltaCollection.CountValues();

            Assert.AreEqual(EXPECTED_COUNT, resultCount);
        }
        [TestMethod]
        public void CombineCtor_WithNullSuperCollection_ReturnsEmptyInstance()
        {
            IEnumerable<StatDeltaCollection<int>> stubNullSuperCollection = null;

            const int EXPECTED_COUNT = 0;

            var mockDeltaCollection = new StatDeltaCollection<int>(stubNullSuperCollection);
            int resultCount = mockDeltaCollection.CountValues();

            Assert.AreEqual(EXPECTED_COUNT, resultCount);
        }
        [TestMethod]
        public void CombineCtor_WithDifferentStatTemplatesAndSameDeltaType_ReturnsCorrectResult()
        {
            const int STAT_1_VALUE = 1;
            const int STAT_2_VALUE = 2;

            var stubSuperCollection = new List<StatDeltaCollection<int>>()
            {
                new StatDeltaCollection<int>(new StatTemplateAndDelta<int>[] {new StatTemplateAndDelta<int>(StubStatTemplate1, StubDeltaType1, STAT_1_VALUE) }),
                new StatDeltaCollection<int>(new StatTemplateAndDelta<int>[] {new StatTemplateAndDelta<int>(StubStatTemplate2, StubDeltaType1, STAT_2_VALUE) }),
            };

            var mockDeltaCollection = new StatDeltaCollection<int>(stubSuperCollection);
            int result1 = mockDeltaCollection.GetDeltaValue(StubStatTemplate1, StubDeltaType1);
            int result2 = mockDeltaCollection.GetDeltaValue(StubStatTemplate2, StubDeltaType1);

            Assert.AreEqual(STAT_1_VALUE, result1);
            Assert.AreEqual(STAT_2_VALUE, result2);
        }
        [TestMethod]
        public void CombineCtor_WithSameStatTemplateAndDifferentDeltaTypes_ReturnsCorrectResult()
        {
            const int DELTA_TYPE_1_VALUE = 1;
            const int DELTA_TYPE_2_VALUE = 2;

            var stubSuperCollection = new List<StatDeltaCollection<int>>()
            {
                new StatDeltaCollection<int>(new StatTemplateAndDelta<int>[] {new StatTemplateAndDelta<int>(StubStatTemplate1, StubDeltaType1, DELTA_TYPE_1_VALUE) }),
                new StatDeltaCollection<int>(new StatTemplateAndDelta<int>[] {new StatTemplateAndDelta<int>(StubStatTemplate1, StubDeltaType2, DELTA_TYPE_2_VALUE) }),
            };

            var mockDeltaCollection = new StatDeltaCollection<int>(stubSuperCollection);
            int result1 = mockDeltaCollection.GetDeltaValue(StubStatTemplate1, StubDeltaType1);
            int result2 = mockDeltaCollection.GetDeltaValue(StubStatTemplate1, StubDeltaType2);

            Assert.AreEqual(DELTA_TYPE_1_VALUE, result1);
            Assert.AreEqual(DELTA_TYPE_2_VALUE, result2);
        }
        [TestMethod]
        public void CombineCtor_WithSameStatTemplateAndSameDeltaType_ReturnsCorrectResult()
        {
            const int VALUE_1 = 1;
            const int VALUE_2 = 2;
            const int EXPECTED_RESULT = VALUE_1 + VALUE_2;

            var stubSuperCollection = new List<StatDeltaCollection<int>>()
            {
                new StatDeltaCollection<int>(new StatTemplateAndDelta<int>[] {new StatTemplateAndDelta<int>(StubStatTemplate1, StubDeltaType1, VALUE_1) }),
                new StatDeltaCollection<int>(new StatTemplateAndDelta<int>[] {new StatTemplateAndDelta<int>(StubStatTemplate1, StubDeltaType1, VALUE_2) }),
            };

            var mockDeltaCollection = new StatDeltaCollection<int>(stubSuperCollection);
            int result = mockDeltaCollection.GetDeltaValue(StubStatTemplate1, StubDeltaType1);

            Assert.AreEqual(EXPECTED_RESULT, result);
        }
        [TestMethod]
        public void GetDeltaValue_WithMissingValue_ReturnsDefault()
        {
            const int DEFAULT_VALUE = default;

            var mockEmptyDeltaCollection = new StatDeltaCollection<int>();
            var result = mockEmptyDeltaCollection.GetDeltaValue(StubStatTemplate1, StubDeltaType1);

            Assert.AreEqual(DEFAULT_VALUE, result);
        }
    }
}
