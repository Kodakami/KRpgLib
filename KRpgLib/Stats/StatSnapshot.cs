using KRpgLib.Stats.Compound;
using System.Collections.Generic;

namespace KRpgLib.Stats
{
    /// <summary>
    /// A frozen collection of stat values from a single moment. Missing stat templates are considered to be at default value. Created by calculating with stat deltas. Set should not be modified after instantiation.
    /// </summary>
    public sealed class StatSnapshot : AbstractStatSet
    {
        private readonly Dictionary<IStatTemplate, float> _statDict;

        /// <summary>
        /// Blank stat set. All values will be considered default.
        /// </summary>
        public StatSnapshot()
        {
            _statDict = new Dictionary<IStatTemplate, float>();
        }
        /// <summary>
        /// Create a stat set from a dictionary of raw stat values.
        /// </summary>
        public StatSnapshot(Dictionary<IStatTemplate, float> statValueDict)
        {
            // New dictionary.
            _statDict = new Dictionary<IStatTemplate, float>(statValueDict);
        }

        protected override float GetStatValue_Internal(IStatTemplate safeStatTemplate)
        {
            if (_statDict.ContainsKey(safeStatTemplate))
            {
                return _statDict[safeStatTemplate];
            }

            // If no value, return default value.
            return safeStatTemplate.DefaultValue;
        }
        protected override float GetCompoundStatValue_Internal(ICompoundStatTemplate safeCompoundStatTemplate)
        {
            return safeCompoundStatTemplate.Algorithm.CalculateValue(this);
        }

        /// <summary>
        /// Is the stat contained in the set? If not, values will be treated as default for the stat.
        /// </summary>
        /// <param name="stat">a stat template</param>
        public bool HasValue(IStatTemplate stat) => _statDict.ContainsKey(stat);

        // Perform an action for each stat in the set (after possibly converting the value).
        private void ForEachValue_Internal(System.Action<IStatTemplate, float> forEachAction, System.Func<IStatTemplate, float, float> valueConverter)
        {
            foreach (var kvp in _statDict)
            {
                // Get converted value.
                float convertedValue = valueConverter(kvp.Key, kvp.Value);

                // Perform action with converted value.
                forEachAction(kvp.Key, convertedValue);
            }
        }

        /// <summary>
        /// Perform an action for each raw stat value in the set.
        /// </summary>
        /// <param name="forEach">an action accepting a stat template and a raw stat value.</param>
        public void ForEachValueRaw(System.Action<IStatTemplate, float> forEach) => ForEachValue_Internal(forEach, (_, value) => value);
        /// <summary>
        /// Perform an action for each legalized stat value in the set.
        /// </summary>
        /// <param name="forEach">an action accepting a stat template and a legalized stat value.</param>
        public void ForEachValue(System.Action<IStatTemplate, float> forEach) => ForEachValue_Internal(forEach,
            (template, value) => template.GetLegalizedValue(value)
            );
    }
}