using System.Collections.Generic;

namespace KRpgLib.Stats.Compound
{
    public interface ICompoundStatTemplate : ILegalizeableNumber<float>
    {
        CompoundStatAlgorithm Algorithm { get; }
    }
    public abstract class AbstractCompoundStatTemplate : ICompoundStatTemplate
    {
        public CompoundStatAlgorithm Algorithm { get; }
        protected StatLegalizer StatLegalizer { get; }

        protected AbstractCompoundStatTemplate(float? min, float? max, float? precision, CompoundStatAlgorithm algorithm)
        {
            StatLegalizer = new StatLegalizer(min, max, precision);
            Algorithm = algorithm ?? throw new System.ArgumentNullException(nameof(algorithm));
        }
        public float GetLegalizedValue(float rawValue) => StatLegalizer.GetLegalizedValue(rawValue);
    }
}
