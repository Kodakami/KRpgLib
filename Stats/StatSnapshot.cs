using System.Collections.Generic;

namespace KRpgLib.Stats
{
    /// <summary>
    /// A frozen collection of stat values from a single moment. Missing stats are considered to be at default value. Created by calculating with stat deltas. Set is not modifiable after instantiation.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class StatSnapshot<TValue> : IStatSet<TValue> where TValue : struct
    {
        private readonly Dictionary<IStat<TValue>, TValue> _statDict = new Dictionary<IStat<TValue>, TValue>();

        private StatSnapshot()
        {
            _statDict = new Dictionary<IStat<TValue>, TValue>();
        }
        private StatSnapshot(Dictionary<IStat<TValue>, TValue> statValueDict)
        {
            _statDict = statValueDict;
        }

        /// <summary>
        /// Get the raw value of a stat. If the stat has no recorded value, returns the stat's default value.
        /// </summary>
        /// <param name="stat">any stat</param>
        /// <returns>current raw stat value, accurate to the represented moment in time</returns>
        public TValue GetStatValue(IStat<TValue> stat)
        {
            if (stat == null)
            {
                throw new System.ArgumentNullException(nameof(stat));
            }

            if (_statDict.ContainsKey(stat))
            {
                return _statDict[stat];
            }

            // If no value, return default value.
            return stat.DefaultValue;
        }

        /// <summary>
        /// Get the legalized value of a stat. If the stat has no recorded value, returns the stat's legalized default value.
        /// </summary>
        /// <param name="stat">any stat</param>
        /// <returns>current legal stat value, accurate to the represented moment in time</returns>
        public TValue GetStatValueLegalized(IStat<TValue> stat)
        {
            // Null check after the jump.
            return stat.GetLegalizedValue(GetStatValue(stat));
        }

        /// <summary>
        /// Create a blank stat snapshot. All values will be considered default.
        /// </summary>
        public static StatSnapshot<TValue> Create() => new StatSnapshot<TValue>();

        /// <summary>
        /// Create a stat snapshot from a dictionary of raw stat values.
        /// </summary>
        public static StatSnapshot<TValue> Create(IEnumerable<KeyValuePair<IStat<TValue>, TValue>> statValues)
        {
            var newDict = new Dictionary<IStat<TValue>, TValue>();
            foreach (var kvp in statValues ?? throw new System.ArgumentNullException(nameof(statValues)))
            {
                newDict[kvp.Key ?? throw new System.ArgumentNullException("stat")] = kvp.Value;
            }

            return new StatSnapshot<TValue>(newDict);
        }

        /// <summary>
        /// Create a stat snapshot from a delta collection. Same effect as using GetSnapshot() on the provided collection.
        /// </summary>
        public static StatSnapshot<TValue> Create(DeltaCollection<TValue> deltaCollection) => deltaCollection.GetSnapshot();
    }
}