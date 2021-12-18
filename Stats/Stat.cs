using KRpgLib.Utility;

namespace KRpgLib.Stats
{
    /// <summary>
    /// An object which provides the default and legalized values of a stat. The values are internally stored as System.Int32 values with an optional DisplayMultiplier which may be applied to the legalized value before displaying the value to the user.
    /// </summary>
    public class Stat
    {
        public StatLegalizer StatLegalizer { get; }

        /// <summary>
        /// The default value of the stat. The value it will start at and will be if no changes are made to it.
        /// </summary>
        public int DefaultValue { get; }
        /// <summary>
        /// The legalized value of a stat is optionally multiplied by this value before being shown to a user. Use this for stats which appear to have decimal places (e.g. "+0.1% Critical Strike Chance" might have a legalized value of 10 and a display multiplier of 0.01f).
        /// </summary>
        public float? DisplayMultiplier { get; }

        // Internal ctor.
        protected Stat(StatLegalizer statLegalizer, int defaultValue, float? displayMultiplier)
        {
            StatLegalizer = statLegalizer ?? throw new System.ArgumentNullException(nameof(statLegalizer));
            DefaultValue = defaultValue;
            DisplayMultiplier = displayMultiplier;
        }
        // External ctor.
        public Stat(int defaultValue, int? minValue = 0, int? maxValue = null, int? precision = null, float? displayMultiplier = null)
            :this(new StatLegalizer(minValue, maxValue, precision), defaultValue, displayMultiplier)
        { }

        /// <inheritdoc cref="StatLegalizer.GetLegalizedValue(int)"/>
        /// <param name="rawValue">the raw stat value to legalize</param>
        public int GetLegalizedValue(int rawValue) => StatLegalizer.GetLegalizedValue(rawValue);
        public float GetLegalizedDisplayValue(int rawValue)
        {
            float output = GetLegalizedValue(rawValue);
            return DisplayMultiplier.HasValue ? output * DisplayMultiplier.Value : output;
        }
    }
}