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
        public static int LegalizeIntValue(int rawValue, int? min, int? max, int? precision)
        {
            // Start with raw value.

            // Factor in MinValue.
            if (min.HasValue)
            {
                rawValue = Math.Max(rawValue, min.Value);
            }

            // Factor in MaxValue.
            if (max.HasValue)
            {
                rawValue = Math.Min(rawValue, max.Value);
            }

            if (precision.HasValue)
            {
                int appliedPrecision = precision.Value;

                if (appliedPrecision > 0)
                {
                    // Divide by precision value, then multiply by precision value again.
                    // floor(15 / 2) * 2 = 14
                    // floor(166 / 50) * 50 = 150

                    // Flooring (truncation) is natural for int.
                    return rawValue / appliedPrecision * appliedPrecision;
                }
                // If Precision is 0 or less, we use integer precision as usual.
            }

            return rawValue;
        }
        public static float LegalizeFloatValue(float rawValue, float? min, float? max, float? precision, int decimalsOfPrecisionForRounding)
        {
            // Start with raw value.

            // Factor in MinValue.
            if (min.HasValue)
            {
                rawValue = Math.Max(rawValue, min.Value);
            }

            // Factor in MaxValue.
            if (max.HasValue)
            {
                rawValue = Math.Min(rawValue, max.Value);
            }

            if (precision.HasValue)
            {
                float appliedPrecision = precision.Value;

                if (appliedPrecision > 0)
                {
                    // Divide by precision value, then multiply by precision value again.
                    // floor(15 / 2) * 2 = 14
                    // floor(166 / 50) * 50 = 150

                    // Calculate the value factoring in precision.
                    float temp = (float)Math.Floor(rawValue / appliedPrecision) * appliedPrecision;

                    if (decimalsOfPrecisionForRounding >= 0)
                    {
                        // Clamp decimals of precision between 0 and 8.
                        // This could be done in the constructor.
                        int appliedDecimalsOfPrecision = Math.Min(decimalsOfPrecisionForRounding, 8);
                        appliedDecimalsOfPrecision = Math.Max(appliedDecimalsOfPrecision, 0);

                        // Round to the closest
                        return (float)Math.Round(temp, appliedDecimalsOfPrecision, MidpointRounding.AwayFromZero);
                    }
                }
                // If Precision is 0 or less, we use floating-point precision as usual.
            }

            return rawValue;
        }
    }
}
