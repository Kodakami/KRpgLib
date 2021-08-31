namespace KRpgLib.Stats
{
    /// <summary>
    /// Interface for an object that provides the default value of a stat, and a method for getting a legalized stat value given a raw value.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public interface IStat<TValue> : ILegalizeableValue<TValue>
        where TValue : struct
    {
        TValue DefaultValue { get; }
    }
    /// <summary>
    /// Base class for an object which provides the default and legalized values of a stat.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public class Stat<TValue> : IStat<TValue> where TValue : struct
    {
        public TValue DefaultValue { get; }
        protected StatLegalizerBase<TValue> StatLegalizer { get; }

        public Stat(StatLegalizerBase<TValue> newStatLegalizer, TValue defaultValue)
        {
            StatLegalizer = newStatLegalizer ?? throw new System.ArgumentNullException(nameof(newStatLegalizer));
            DefaultValue = defaultValue;
        }
        public TValue GetLegalizedValue(TValue rawValue) => StatLegalizer.GetLegalizedValue(rawValue);
    }
    /// <summary>
    /// An object which provides the default and legalized values of a stat backed by int (System.Int32) values. This subclass of Stat<TValue> implements the default StatLegalizer for ints.
    /// </summary>
    public class Stat_Int : Stat<int>
    {
        // Silently pass in default System.Int32 implementation of the stat legalizer.
        public Stat_Int(int? min, int? max, int? precision, int defaultValue)
            : base(new StatLegalizer_Int(min, max, precision), defaultValue) { }
    }
    /// <summary>
    /// An object which provides the default and legalized values of a stat backed by float (System.Single) values. This subclass of Stat<TValue> implements the default StatLegalizer for floats.
    /// </summary>
    public class Stat_Float : Stat<float>
    {
        // Silently pass in default System.Single implementation of the stat legalizer.
        public Stat_Float(float? min, float? max, float? precision, float defaultValue, int decimalsOfPrecisionForRounding)
            : base(new StatLegalizer_Float(min, max, precision, decimalsOfPrecisionForRounding), defaultValue) { }
    }
}