using System;

namespace KRpgLib.Stats
{
    /// <summary>
    /// Abstract base class for an object that legalizes a stat value by a minimum, maximum, and precision value.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public abstract class AbstractStatLegalizer<TValue> where TValue : struct
    {
        /// <summary>
        /// Optional minimum legal value for the stat.
        /// </summary>
        public TValue? MinValue { get; }
        /// <summary>
        /// Optional maximum legal value for the stat.
        /// </summary>
        public TValue? MaxValue { get; }
        /// <summary>
        /// Optional minimum precision step by which the stat value moves. A legal stat value must be a multiple of this value. If this value is less than or equal to 0, it will be treated as null.
        /// </summary>
        public TValue? Precision { get; }

        protected AbstractStatLegalizer(TValue? min, TValue? max, TValue? precision)
        {
            MinValue = min;
            MaxValue = max;
            Precision = precision;
        }

        public abstract TValue GetLegalizedValue(TValue rawValue);
    }
    /// <summary>
    /// Concrete class for an object that legalizes an int (System.Int32) value by a minimum, maximum, and precision value.
    /// </summary>
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
    /// <summary>
    /// Concrete class for an object that legalizes a float (System.Single) value by a minimum, maximum, and precision value. Additionally requires specifying a number of decimals of precision while rounding.
    /// </summary>
    public sealed class StatLegalizer_Float : AbstractStatLegalizer<float>
    {
        /// <summary>
        /// Number of decimal places to consider for rounding purposes. Number is internally clamped between 0 and 8.
        /// </summary>
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

                    if (DecimalsOfPrecisionForRounding >= 0)
                    {
                        // Clamp decimals of precision between 0 and 8.
                        // This could be done in the constructor.
                        int appliedDecimalsOfPrecision = Math.Min(DecimalsOfPrecisionForRounding, 8);
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