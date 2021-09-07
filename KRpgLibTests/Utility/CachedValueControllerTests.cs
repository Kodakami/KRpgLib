using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KRpgLibTests.Utility
{
    [TestClass]
    public class CachedValueControllerTests
    {
        [TestMethod]
        [DataRow(new int[] { 1 }, 1, DisplayName = "1 = 1")]
        [DataRow(new int[] { 1, 2, 3 }, 6, DisplayName = "1 + 2 + 3 = 6")]
        public void GetCachedValue_AfterItemsAdded_ReturnsCorrectResult(int[] itemsAdded, int expected)
        {
            var mock = new FakeCachedValueController();
            foreach (var x in itemsAdded)
            {
                mock.Add(x);
            }

            int result = mock.GetCacheCopy();

            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        public void GetCachedValue_WhenManyItemsAdded_UpdatesOnce()
        {
            const int EXPECTED_UPDATE_COUNT_BEFORE_RETRIEVAL = 0;
            const int EXPECTED_UPDATE_COUNT_AFTER_RETRIEVAL = 1;
            const int ITERATION_COUNT = 2;
            var mock = new FakeCachedValueController();

            int updateCount = 0;
            void incrementUpdateCounter() => updateCount++;
            mock.OnCacheUpdated += incrementUpdateCounter;

            for (int i = 0; i < ITERATION_COUNT; i++)
            {
                mock.Add(i);
            }

            Assert.AreEqual(EXPECTED_UPDATE_COUNT_BEFORE_RETRIEVAL, updateCount);

            int _ = mock.GetCacheCopy();

            Assert.AreEqual(EXPECTED_UPDATE_COUNT_AFTER_RETRIEVAL, updateCount);

            mock.OnCacheUpdated -= incrementUpdateCounter;
        }
    }
}
