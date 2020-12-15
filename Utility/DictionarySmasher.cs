using System;
using System.Collections.Generic;
using System.Text;

namespace KRpgLib.Utility
{
    /// <summary>
    /// Static class with a method for combining multiple dictionaries into a single new one with all values from duplicate keys being combined in a particular way.
    /// </summary>
    public static class DictionarySmasher<TKey, TValue>
    {
        /// <summary>
        /// A function taking a dictionary's key and all values found for that key, and returning the result of combining all those values.
        /// </summary>
        /// <param name="key">the key from the Dictionary<TKey, TValue></param>
        /// <param name="allValuesForKey">all values found for the key across the provided collection of Dictionary<TKey, TValue></param>
        /// <returns>combined value for new dictionary</returns>
        public delegate TValue ValueSmashDelegate(TKey key, List<TValue> allValuesForKey);

        public static Dictionary<TKey, TValue> Smash(ValueSmashDelegate valueSmasher, IEnumerable<Dictionary<TKey, TValue>> dictionaries)
        {
            if (valueSmasher == null)
            {
                throw new ArgumentNullException(nameof(valueSmasher));
            }

            // New dictionary.
            var outDict = new Dictionary<TKey, TValue>();

            // Quick escape if dictionary collection is null (empty collection will result in no loops).
            if (dictionaries == null)
            {
                return outDict;
            }

            // (No quick escape if dictionary collection has only one item. Provided value smasher could be a form of conversion.)

            // Populate dictionary with all keys.

            // For each Dictionary we are combining...
            foreach (var dict in dictionaries)
            {
                // Ignore null dictionaries.
                if (dict == null)
                {
                    continue;
                }

                // For each key in their dictionary...
                foreach (var key in dict.Keys)
                {
                    // Establish it in our new one (will check for duplicates).
                    outDict[key] = default;
                }
            }

            // Collect all values for each key and create a new smashed value.

            // For each key we just added to the new dictionary (which is one copy of each key across all the dictionaries we are combining)...
            foreach (var newKey in new List<TKey>(outDict.Keys))    // Collection clone.
            {
                // Make a new list of values (for the ones related to this key).
                var valueList = new List<TValue>();

                // For each Dictionary we are combining...
                foreach (var dict in dictionaries)
                {
                    // Ignore null dictionaries.
                    if (dict == null)
                    {
                        continue;
                    }

                    // Try to get a value for the key.
                    if (dict.TryGetValue(newKey, out TValue found))
                    {
                        // And add it to the list of values we are collecting.
                        valueList.Add(found);
                    }
                }

                // After all related values have been found, smash them together according to the provided delegate.
                var smashedResult = valueSmasher.Invoke(newKey, valueList);

                // And assign the new value to its space in the new dictionary.
                outDict[newKey] = smashedResult;
            }

            // You may now have what's left of your books.
            return outDict;
        }
    }
}
