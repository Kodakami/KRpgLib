using KRpgLib.Stats.Compound;
using System;
using System.Collections.Generic;

namespace KRpgLib.Stats
{
    public static class StatUtilities
    {
        /// <summary>
        /// The core game math for calculating a stat.
        /// </summary>
        /// <param name="statTemplate"></param>
        /// <param name="statDeltas"></param>
        /// <returns>a raw stat value as a float</returns>
        // Start with the default value, take each type of delta in order of priority, combine the values of that type and apply them to the output.
        // TODO: The speed of this process could be improved by keeping all deltas as separate collections.
        public static float ApplyStatDeltasByType(IStatTemplate statTemplate, List<StatDelta> statDeltas)
        {
            // Shortcut for empty delta list.
            if (statDeltas == null || statDeltas.Count == 0)
            {
                return statTemplate.DefaultValue;
            }

            // Start with the default value.
            float output = statTemplate.DefaultValue;

            // For each type of stat delta (addition, multiplication)...
            foreach (StatDeltaType deltaType in StatDeltaType.AllByPriority)
            {
                // Track the total delta value of this type.
                float totalDelta = deltaType.BaselineValue;

                // For each stat delta provided...
                foreach (StatDelta delta in statDeltas)
                {
                    // If the delta is of the correct type
                    if (delta.Type == deltaType)
                    {
                        // Add the value to the current total delta value.
                        totalDelta += delta.Value;
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
