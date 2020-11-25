namespace KRpgLib.Stats
{
    /// <summary>
    /// Interface for an object that provides the default value of a stat, and a method for getting a legalized stat value given a raw value.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public interface IStatTemplate<TValue> : ILegalizeableValue<TValue> where TValue : struct
    {
        TValue DefaultValue { get; }
    }
    /// <summary>
    /// Abstract base class for an object which provides the default and legalized values of a stat.
    /// </summary>
    /// <typeparam name="TValue">stat backing value</typeparam>
    public abstract class AbstractStatTemplate<TValue> : IStatTemplate<TValue> where TValue : struct
    {
        public TValue DefaultValue { get; }
        protected AbstractStatLegalizer<TValue> StatLegalizer { get; }

        protected AbstractStatTemplate(AbstractStatLegalizer<TValue> newStatLegalizer, TValue defaultValue)
        {
            StatLegalizer = newStatLegalizer ?? throw new System.ArgumentNullException(nameof(newStatLegalizer));
            DefaultValue = defaultValue;
        }
        public TValue GetLegalizedValue(TValue rawValue) => StatLegalizer.GetLegalizedValue(rawValue);
    }
    /// <summary>
    /// Abstract base class for an object which provides the default and legalized values of a stat backed by int (System.Int32) values.
    /// </summary>
    public class AbstractStatTemplate_Int : AbstractStatTemplate<int>
    {
        protected AbstractStatTemplate_Int(int? min, int? max, int? precision, int defaultValue)
            : base(new StatLegalizer_Int(min, max, precision), defaultValue) { }
    }
    /// <summary>
    /// Abstract base class for an object which provides the default and legalized values of a stat backed by float (System.Single) values.
    /// </summary>
    public class AbstractStatTemplate_Float : AbstractStatTemplate<float>
    {
        protected AbstractStatTemplate_Float(float? min, float? max, float? precision, float defaultValue, int decimalsOfPrecisionForRounding)
            : base(new StatLegalizer_Float(min, max, precision, decimalsOfPrecisionForRounding), defaultValue) { }
    }
}