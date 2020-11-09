using System;

namespace KRpgLib.Stats
{
    public sealed class StatLegalizer
    {
        public float? MinValue { get; }
        private float? MaxValue { get; }
        private float? Precision { get; }

        public StatLegalizer(float? min, float? max, float? precision)
        {
            MinValue = min;
            MaxValue = max;
            Precision = precision;
        }

        public float GetLegalizedValue(float rawValue)
        {
            // Start with raw value.

            // Factor in MinValue.
            if (MinValue.HasValue)
            {
                rawValue = Math.Max(rawValue, MinValue.Value);
            }

            // Factor in MaxValue.
            if (MaxValue.HasValue)
            {
                rawValue = Math.Min(rawValue, MaxValue.Value);
            }

            if (Precision.HasValue)
            {
                float appliedPrecision = Precision.Value;

                if (appliedPrecision > 0)
                {
                    // Divide by precision value, floor (truncate), then multiply by precision value again.
                    // floor(15 / 2) * 2 = 14
                    // floor(166.7234 / 0.5) * 0.5 = 166.5
                    return (float)(Math.Floor(rawValue / appliedPrecision) * appliedPrecision);
                }
                // If Precision is 0 or less, we use single floating-point precision.
            }

            return rawValue;
        }
    }
}