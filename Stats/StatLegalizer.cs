namespace KRpgLib.Stats
{
    public sealed class StatLegalizer
    {
        public StatLegalizer(int? minValue, int? maxValue, int? precision)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            Precision = precision;
        }

        /// <summary>
        /// The optional minimum value of the stat. If the raw value of a stat is less than this number (including the default value), the stat value will be this value instead. Minimum value takes precedence over maximum.
        /// </summary>
        public int? MinValue { get; }
        /// <summary>
        /// The optional maximum value of the stat. If the raw value of a stat is greater than this number (including the default value), the stat value will be this value instead. Minimum value takes precedence over maximum.
        /// </summary>
        public int? MaxValue { get; }
        /// <summary>
        /// The optional minimum precision step by which the stat value moves. A legal stat value must be a multiple of this value. If this value is less than or equal to 1 (or is null), it will be treated as 1.
        /// </summary>
        public int? Precision { get; }

        /// <summary>
        /// Return the value legalized by minimum, maximum, and precision values.
        /// </summary>
        public int GetLegalizedValue(int rawValue)
        {
            return StatUtilities.LegalizeIntValue(rawValue, MinValue, MaxValue, Precision);
        }
    }
}