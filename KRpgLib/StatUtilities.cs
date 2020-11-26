using System;
using System.Collections.Generic;

namespace KRpgLib.Stats
{
    /// <summary>
    /// Static class with utility functions for the stat system.
    /// </summary>
    public static class StatUtilities
    {
        /// <summary>
        /// The core game math for calculating a stat.
        /// </summary>
        /// <param name="statTemplate">any stat template</param>
        /// <param name="statDeltas">a list of stat deltas</param>
        /// <returns>a raw stat value as a float</returns>
        // Start with the default value, take each type of delta in order of priority, combine the values of that type and apply them to the output.
        // TODO: The speed of this process could be improved by keeping all deltas as separate collections.
        public static TValue ApplyStatDeltasByType<TValue>(IStatTemplate<TValue> statTemplate, List<StatDelta<TValue>> statDeltas) where TValue : struct
        {
            if (statTemplate == null)
            {
                throw new ArgumentNullException(nameof(statTemplate));
            }

            // Shortcut for empty delta list.
            if (statDeltas == null || statDeltas.Count == 0)
            {
                return statTemplate.DefaultValue;
            }

            // Start with the default value.
            TValue output = statTemplate.DefaultValue;

            // For each type of stat delta (addition, multiplication)...
            foreach (StatDeltaType<TValue> deltaType in StatDeltaType<TValue>.GetAllByPriority())
            {
                // Track the total delta value of this type.
                TValue totalDelta = deltaType.BaselineValue;

                // For each stat delta provided...
                foreach (StatDelta<TValue> delta in statDeltas)
                {
                    // If the delta is of the correct type
                    if (delta.Type == deltaType)
                    {
                        // Combine the value to the current total delta value (method by which is defined in the delta type).
                        totalDelta = delta.Type.Combine(totalDelta, delta.Value);
                    }
                }

                // Apply the delta to the current output value (each delta type will apply one after the other, in order of priority).
                output = deltaType.Apply(output, totalDelta);
            }

            // Return the total result.
            return output;
        }
    }
}
