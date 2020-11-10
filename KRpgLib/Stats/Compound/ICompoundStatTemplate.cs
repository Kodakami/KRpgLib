using System.Collections.Generic;

namespace KRpgLib.Stats.Compound
{
    public interface ICompoundStatTemplate<TValue> : ILegalizeableValue<TValue> where TValue : struct
    {
        TValue CalculateValue(IStatSet<TValue> statSet);
    }
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
    public abstract class AbstractCompoundStatTemplate_Int : AbstractCompoundStatTemplate<int>
    {
        protected AbstractCompoundStatTemplate_Int(int? min, int? max, int? precison, CompoundStatAlgorithm<int> algorithm)
            :base(new StatLegalizer_Int(min, max, precison), algorithm) { }
    }
    public abstract class AbstractCompoundStatTemplate_Float : AbstractCompoundStatTemplate<float>
    {
        protected AbstractCompoundStatTemplate_Float(float? min, float? max, float? precison, int decimalsOfPrecisionForRounding, CompoundStatAlgorithm<float> algorithm)
            : base(new StatLegalizer_Float(min, max, precison, decimalsOfPrecisionForRounding), algorithm) { }
    }
}
