using System.Collections.Generic;

namespace KRpgLib.Stats
{
    /// <summary>
    /// A frozen collection of stat values from a single moment. Missing stats are considered to be at default value. Created by calculating with stat deltas. Set is not modifiable after instantiation.
    /// </summary>
    public sealed class StatSnapshot : IStatSet
    {
        private readonly Dictionary<Stat, int> _statDict;

        private StatSnapshot()
        {
            _statDict = new Dictionary<Stat, int>();
        }
        public StatSnapshot(IEnumerable<KeyValuePair<Stat, int>> statValueDict)
        {
            _statDict = new Dictionary<Stat, int>();
            foreach (var kvp in statValueDict ?? throw new System.ArgumentNullException(nameof(statValueDict)))
            {
                _statDict[kvp.Key ?? throw new System.ArgumentNullException("stat")] = kvp.Value;
            }
        }

        /// <summary>
        /// Get the raw value of a stat. If the stat has no recorded value, returns the stat's default value.
        /// </summary>
        /// <param name="stat">any stat</param>
        /// <returns>current raw stat value, accurate to the represented moment in time</returns>
        public int GetStatValue(Stat stat)
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
        public int GetStatValueLegalized(Stat stat)
        {
            // Null check after the jump.
            return stat.GetLegalizedValue(GetStatValue(stat));
        }
    }
}