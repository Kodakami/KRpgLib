using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRpgLib.Utility;

namespace KRpgLibTests.Utility
{
    [TestClass]
    public class PriorityRegistryTests
    {
        private void AssertReadOnlyListsAreEqual<T>(IReadOnlyList<T> expected, IReadOnlyList<T> result)
        {
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], result[i]);
            }
        }
        [TestMethod]
        public void RegisterItem_WithAlreadyRegisteredItem_ThrowsArgEx()
        {
            const string ANY_ITEM = "any item";
            var mockRegistry = new PriorityRegistry<string>();
            void action() => mockRegistry.RegisterItem(ANY_ITEM, 0);

            // Once.
            action();

            // Twice.
            Assert.ThrowsException<ArgumentException>(action);
        }
        [TestMethod]
        public void GetAllByPriority_WithDisorganizedRegistrations_ReturnsCorrectResult()
        {
            // Assemble.
            const string FIRST_ITEM = "first";
            const string SECOND_ITEM = "second";
            const string THIRD_ITEM = "third";

            var mockRegistry = new PriorityRegistry<string>();

            mockRegistry.RegisterItem(SECOND_ITEM, 1);
            mockRegistry.RegisterItem(THIRD_ITEM, 2);
            mockRegistry.RegisterItem(FIRST_ITEM, 0);

            var expectedList = new List<string>()
            {
                FIRST_ITEM,
                SECOND_ITEM,
                THIRD_ITEM,
            };

            // Act.
            var resultList = mockRegistry.GetAllByPriority();

            // Assert.
            AssertReadOnlyListsAreEqual(expectedList, resultList);
        }
        [TestMethod]
        public void GetAllByPriority_AfterUnregisteringItem_ReturnsCorrectResult()
        {
            // Assemble.
            const string FIRST_ITEM = "first";
            const string SECOND_ITEM = "second";

            var mockRegistry = new PriorityRegistry<string>();

            mockRegistry.RegisterItem(FIRST_ITEM, 0);
            mockRegistry.RegisterItem(SECOND_ITEM, 1);

            var expectedListBefore = new List<string>()
            {
                FIRST_ITEM,
                SECOND_ITEM,
            };

            // Act.
            var resultListBefore = mockRegistry.GetAllByPriority();

            // Assert.
            AssertReadOnlyListsAreEqual(expectedListBefore, resultListBefore);

            // Assemble.
            var expectedListAfter = new List<string>()
            {
                SECOND_ITEM,
            };

            // Act.
            mockRegistry.UnregisterItem(FIRST_ITEM);
            var resultListAfter = mockRegistry.GetAllByPriority();

            // Assert.
            AssertReadOnlyListsAreEqual(expectedListAfter, resultListAfter);
        }
    }
}
