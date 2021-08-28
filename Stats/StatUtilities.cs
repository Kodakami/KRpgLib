using System;
using System.Collections.Generic;

namespace KRpgLib.Stats
{
    /// <summary>
    /// Static class with utility functions for the stat system.
    /// </summary>
    public static class StatUtilities
    {
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

                if (appliedPrecision > 1)
                {
                    // Divide by precision value, then multiply by precision value again.
                    // floor(15 / 2) * 2 = 14
                    // floor(166 / 50) * 50 = 150

                    // Flooring (truncation) is natural for int.
                    return (rawValue / appliedPrecision) * appliedPrecision;
                }
                // If Precision is 1 or less, we use integer precision as usual.
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
