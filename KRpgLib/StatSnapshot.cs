using System.Collections.Generic;

namespace KRpgLib.Stats
{
    /// <summary>
    /// A frozen collection of stat values from a single moment. Missing stat templates are considered to be at default value. Created by calculating with stat deltas. Set is not modifiable after instantiation.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class StatSnapshot<TValue> : AbstractStatSet<TValue> where TValue : struct
    {
        private readonly Dictionary<IStatTemplate<TValue>, TValue> _statDict = new Dictionary<IStatTemplate<TValue>, TValue>();

        /// <summary>
        /// Blank stat set. All values will be considered default.
        /// </summary>
        public StatSnapshot()
        {
            // Nothing! :D
        }
        /// <summary>
        /// Create a stat set from a dictionary of raw stat values.
        /// </summary>
        public StatSnapshot(Dictionary<IStatTemplate<TValue>, TValue> statValueDict)
        {
            // New dictionary.
            _statDict = new Dictionary<IStatTemplate<TValue>, TValue>(statValueDict);
        }

        protected override TValue GetStatValue_Internal(IStatTemplate<TValue> safeStatTemplate)
        {
            if (_statDict.ContainsKey(safeStatTemplate))
            {
                return _statDict[safeStatTemplate];
            }

            // If no value, return default value.
            return safeStatTemplate.DefaultValue;
        }

        /// <summary>
        /// Is the stat contained in the set? If not, values will be treated as default for the stat.
        /// </summary>
        /// <param name="stat">a stat template</param>
        public bool HasValue(IStatTemplate<TValue> stat) => _statDict.ContainsKey(stat);
    }
}