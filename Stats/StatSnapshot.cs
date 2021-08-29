using System.Collections.Generic;

namespace KRpgLib.Stats
{
    /// <summary>
    /// A frozen collection of stat values from a single moment. Missing stat templates are considered to be at default value. Created by calculating with stat deltas. Set is not modifiable after instantiation.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class StatSnapshot<TValue> : IStatSet<TValue> where TValue : struct
    {
        private readonly Dictionary<IStatTemplate<TValue>, TValue> _statDict = new Dictionary<IStatTemplate<TValue>, TValue>();

        private StatSnapshot()
        {
            _statDict = new Dictionary<IStatTemplate<TValue>, TValue>();
        }
        private StatSnapshot(Dictionary<IStatTemplate<TValue>, TValue> statValueDict)
        {
            _statDict = statValueDict;
        }

        /// <summary>
        /// Get the raw value of a stat. If the stat has no recorded value, returns the stat's default value.
        /// </summary>
        /// <param name="statTemplate">any stat template</param>
        /// <returns>current raw stat value, accurate to the represented moment in time</returns>
        public TValue GetStatValue(IStatTemplate<TValue> statTemplate)
        {
            if (statTemplate == null)
            {
                throw new System.ArgumentNullException(nameof(statTemplate));
            }

            if (_statDict.ContainsKey(statTemplate))
            {
                return _statDict[statTemplate];
            }

            // If no value, return default value.
            return statTemplate.DefaultValue;
        }

        /// <summary>
        /// Get the legalized value of a stat. If the stat has no recorded value, returns the stat's legalized default value.
        /// </summary>
        /// <param name="statTemplate">any stat template</param>
        /// <returns>current legal stat value, accurate to the represented moment in time</returns>
        public TValue GetStatValueLegalized(IStatTemplate<TValue> statTemplate)
        {
            // Null check after the jump.
            return statTemplate.GetLegalizedValue(GetStatValue(statTemplate));
        }

        /// <summary>
        /// Create a blank stat set. All values will be considered default.
        /// </summary>
        public static StatSnapshot<TValue> Create() => new StatSnapshot<TValue>();

        /// <summary>
        /// Create a stat set from a dictionary of raw stat values.
        /// </summary>
        public static StatSnapshot<TValue> Create(IReadOnlyDictionary<IStatTemplate<TValue>, TValue> statValueDict)
        {
            // For some reason, you can't automatically create a new dictionary from a read-only dictionary.

            var newDict = new Dictionary<IStatTemplate<TValue>, TValue>();
            foreach (var kvp in statValueDict ?? throw new System.ArgumentNullException(nameof(statValueDict)))
            {
                newDict[kvp.Key ?? throw new System.ArgumentNullException("stat template")] = kvp.Value;
            }

            return new StatSnapshot<TValue>(newDict);
        }

        /// <summary>
        /// Create a stat set from a stat delta collection. Same effect as using GetStatSnapshot() on the provided collection.
        /// </summary>
        public static StatSnapshot<TValue> Create(StatDeltaCollection<TValue> statDeltaCollection) => statDeltaCollection.GetStatSnapshot();
    }
}