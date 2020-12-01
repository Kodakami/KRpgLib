using System;
using System.Collections.Generic;
using KRpgLib.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UtilityTests
{
    [TestClass]
    public class DictionarySmasherTests
    {
        [TestMethod]
        public void Smash_WithNullSmashDelegate_ThrowsArgNullEx()
        {
            var stubDictCollection = new List<Dictionary<int, int>>();
            void exceptionalAction() => DictionarySmasher<int, int>.Smash(null, stubDictCollection);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void Smash_WithNullCollection_ReturnsEmptyDictionary()
        {
            const int EXPECTED_RESULT_COUNT = 0;
            static int stubSmasher(int _, List<int> __) => 0;

            var result = DictionarySmasher<int, int>.Smash(stubSmasher, null);

            Assert.AreEqual(EXPECTED_RESULT_COUNT, result.Count);
        }
        [TestMethod]
        public void Smash_WithEmptyCollection_ReturnsEmptyDictionary()
        {
            const int EXPECTED_RESULT_COUNT = 0;
            var stubDictCollection = new List<Dictionary<int, int>>();
            static int stubSmasher(int _, List<int> __) => 0;

            var result = DictionarySmasher<int, int>.Smash(stubSmasher, stubDictCollection);

            Assert.AreEqual(EXPECTED_RESULT_COUNT, result.Count);
        }
        [TestMethod]
        public void Smash_WithValidInputs_ReturnsCorrectResult()
        {
            /*  Dictionaries:
             *      Dictionary 0: {2, 0}
             *      Dictionary 1: {2, 1}
             *      Dictionary 2: {2, 2}
             *      
             *  Value Smash Delegate: ((sum of values) * key)
             *      
             *  Expected Result:  {2, 6}
             */

            var stubDictCollection = new List<Dictionary<int, int>>();
            for (int dictIndex = 0; dictIndex < 3; dictIndex++)
            {
                stubDictCollection.Add(new Dictionary<int, int>() { { 2, dictIndex } });
            }

            static int stubSmasher(int key, List<int> valueCollection)
            {
                // Add values together, multiply by key.
                int valueTotal = 0;
                foreach (var value in valueCollection)
                {
                    valueTotal += value;
                }
                return valueTotal * key;
            }

            const int EXPECTED_RESULT_VALUE = 6;

            var result = DictionarySmasher<int, int>.Smash(stubSmasher, stubDictCollection);

            Assert.AreEqual(EXPECTED_RESULT_VALUE, result[2]);
        }
    }
}
