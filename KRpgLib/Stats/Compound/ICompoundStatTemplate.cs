namespace KRpgLib.Stats.Compound
{
    /// <summary>
    /// Interface for an object that provides the value of a compound stat given a stat set.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public interface ICompoundStatTemplate<TValue> : ILegalizeableValue<TValue> where TValue : struct
    {
        TValue CalculateValue(IStatSet<TValue> statSet);
    }
    /// <summary>
    /// Abstract base class for a compound stat template. Uses an AbstractStatLegalizer and CompoundStatAlgorithm to calculate the current value of a compound stat.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public abstract class AbstractCompoundStatTemplate<TValue> : ICompoundStatTemplate<TValue> where TValue : struct
    {
        protected CompoundStatAlgorithm<TValue> Algorithm { get; }
        protected AbstractStatLegalizer<TValue> StatLegalizer { get; }

        protected AbstractCompoundStatTemplate(AbstractStatLegalizer<TValue> newStatLegalizer, CompoundStatAlgorithm<TValue> algorithm)
        {
            StatLegalizer = newStatLegalizer ?? throw new System.ArgumentNullException(nameof(newStatLegalizer));
            Algorithm = algorithm ?? throw new System.ArgumentNullException(nameof(algorithm));
        }
        public TValue CalculateValue(IStatSet<TValue> statSet) => Algorithm.CalculateValue(statSet ?? throw new System.ArgumentNullException(nameof(statSet)));
        public TValue GetLegalizedValue(TValue rawValue) => StatLegalizer.GetLegalizedValue(rawValue);
    }
    /// <summary>
    /// Abstract base class for a compound stat template with int (System.Int32) backing values.
    /// </summary>
    public abstract class AbstractCompoundStatTemplate_Int : AbstractCompoundStatTemplate<int>
    {
        protected AbstractCompoundStatTemplate_Int(int? min, int? max, int? precison, CompoundStatAlgorithm<int> algorithm)
            : base(new StatLegalizer_Int(min, max, precison), algorithm) { }
    }
    /// <summary>
    /// Abstract base class for a compound stat template with float (System.Single) backing values.
    /// </summary>
    public abstract class AbstractCompoundStatTemplate_Float : AbstractCompoundStatTemplate<float>
    {
        protected AbstractCompoundStatTemplate_Float(float? min, float? max, float? precison, int decimalsOfPrecisionForRounding, CompoundStatAlgorithm<float> algorithm)
            : base(new StatLegalizer_Float(min, max, precison, decimalsOfPrecisionForRounding), algorithm) { }
    }
}
