namespace KRpgLib.Stats.Compound
{
    /// <summary>
    /// Interface for an object that provides the value of a compound stat given a stat set. A compound stat is usually the result of an algorithm applied to other stats, such as "Total Defense Power" or a damage-calculation algorithm.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public interface ICompoundStat<TValue> : ILegalizeableValue<TValue> where TValue : struct
    {
        /// <summary>
        /// Calculate and return the value of this compound stat using the provided stat set for reference.
        /// </summary>
        TValue CalculateValue(IStatSet<TValue> statSet);
    }
    /// <summary>
    /// A compound stat which uses a StatLegalizer and CompoundStatAlgorithm to calculate the current value.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public class CompoundStat<TValue> : ICompoundStat<TValue> where TValue : struct
    {
        protected CompoundStatAlgorithm<TValue> Algorithm { get; }
        protected StatLegalizerBase<TValue> StatLegalizer { get; }

        public CompoundStat(StatLegalizerBase<TValue> legalizer, CompoundStatAlgorithm<TValue> algorithm)
        {
            StatLegalizer = legalizer ?? throw new System.ArgumentNullException(nameof(legalizer));
            Algorithm = algorithm ?? throw new System.ArgumentNullException(nameof(algorithm));
        }
        public TValue CalculateValue(IStatSet<TValue> statSet) => Algorithm.CalculateValue(statSet ?? throw new System.ArgumentNullException(nameof(statSet)));

        /// <inheritdoc cref="StatLegalizerBase{TValue}.GetLegalizedValue(TValue)"/>
        public TValue GetLegalizedValue(TValue rawValue) => StatLegalizer.GetLegalizedValue(rawValue);
    }
    /// <summary>
    /// A compound stat with int (System.Int32) backing values. This subclass of CompoundStat<TValue> implements the default StatLegalizer for ints.
    /// </summary>
    public class CompoundStat_Int : CompoundStat<int>
    {
        // Silently pass in default System.Int32 implementation of the stat legalizer.
        public CompoundStat_Int(int? min, int? max, int? precison, CompoundStatAlgorithm<int> algorithm)
            : base(new StatLegalizer_Int(min, max, precison), algorithm) { }
    }
    /// <summary>
    /// A compound stat with float (System.Single) backing values. This subclass of CompoundStat<TValue> implements the default StatLegalizer for floats.
    /// </summary>
    public class CompoundStat_Float : CompoundStat<float>
    {
        // Silently pass in default System.Single implementation of the stat legalizer.
        public CompoundStat_Float(float? min, float? max, float? precison, int decimalsOfPrecisionForRounding, CompoundStatAlgorithm<float> algorithm)
            : base(new StatLegalizer_Float(min, max, precison, decimalsOfPrecisionForRounding), algorithm) { }
    }
}
