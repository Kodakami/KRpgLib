using System;

namespace KRpgLib.Stats
{
    public abstract class AbstractStatLegalizer<TValue> where TValue : struct
    {
        public TValue? MinValue { get; }
        public TValue? MaxValue { get; }
        public TValue? Precision { get; }

        protected AbstractStatLegalizer(TValue? min, TValue? max, TValue? precision)
        {
            MinValue = min;
            MaxValue = max;
            Precision = precision;
        }

        public abstract TValue GetLegalizedValue(TValue rawValue);
    }
    public sealed class StatLegalizer_Int : AbstractStatLegalizer<int>
    {
        public StatLegalizer_Int(int? min, int? max, int? precision)
            : base(min, max, precision) { }

        public override int GetLegalizedValue(int rawValue)
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
                int appliedPrecision = Precision.Value;

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
    }
    public sealed class StatLegalizer_Float : AbstractStatLegalizer<float>
    {
        public int DecimalsOfPrecisionForRounding { get; }
        public StatLegalizer_Float(float? min, float? max, float? precision, int decimalsOfPrecisionForRounding)
            : base(min, max, precision)
        {
            DecimalsOfPrecisionForRounding = decimalsOfPrecisionForRounding;
        }

        public override float GetLegalizedValue(float rawValue)
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
                    // Divide by precision value, then multiply by precision value again.
                    // floor(15 / 2) * 2 = 14
                    // floor(166 / 50) * 50 = 150

                    // Calculate the value factoring in precision.
                    float temp = (float)Math.Floor(rawValue / appliedPrecision) * appliedPrecision;

                    // Round to the closest
                    return (float)Math.Round(temp, DecimalsOfPrecisionForRounding, MidpointRounding.AwayFromZero);
                }
                // If Precision is 0 or less, we use integer precision as usual.
            }

            return rawValue;
        }
    }
}