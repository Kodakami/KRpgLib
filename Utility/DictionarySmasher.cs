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
        public delegate TValue ValueSmashDelegate(TKey key, IEnumerable<TValue> allValuesForKey);

        public static Dictionary<TKey, TValue> Smash(ValueSmashDelegate valueSmasher, IEnumerable<Dictionary<TKey, TValue>> dictionaries)
        {
            // New dictionary.
            var outDict = new Dictionary<TKey, TValue>();

            // Quick escape if arg null.
            if (dictionaries == null)
            {
                return outDict;
            }

            // Populate dictionary with all keys.

            // For each Dictionary we are combining...
            foreach (var dict in dictionaries)
            {
                // For each key in their dictionary...
                foreach (var key in dict.Keys)
                {
                    // Establish it in our new one (will check for duplicates).
                    outDict[key] = default;
                }
            }

            // Collect all values for each key and create a new smashed value.

            // For each key we just added to the new dictionary (which is one copy of each key across all the dictionaries we are combining)...
            foreach (var newKey in outDict.Keys)
            {
                // Make a new list of values (for the ones related to this key).
                var valueList = new List<TValue>();

                // For each Dictionary we are combining...
                foreach (var dict in dictionaries)
                {
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
