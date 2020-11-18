namespace KRpgLib.Stats
{
    public interface IStatTemplate<TValue> : ILegalizeableValue<TValue> where TValue : struct
    {
        TValue DefaultValue { get; }
    }
    public abstract class AbstractStatTemplate<TValue> : IStatTemplate<TValue> where TValue : struct
    {
        public TValue DefaultValue { get; }
        protected AbstractStatLegalizer<TValue> StatLegalizer { get; }

        protected AbstractStatTemplate(AbstractStatLegalizer<TValue> newStatLegalizer, TValue defaultValue)
        {
            StatLegalizer = newStatLegalizer;
            DefaultValue = defaultValue;
        }
        public TValue GetLegalizedValue(TValue rawValue) => StatLegalizer.GetLegalizedValue(rawValue);
    }
    public class AbstractStatTemplate_Int : AbstractStatTemplate<int>
    {
        protected AbstractStatTemplate_Int(int? min, int? max, int? precision, int defaultValue)
            : base(new StatLegalizer_Int(min, max, precision), defaultValue) { }
    }
    public class AbstractStatTemplate_Float : AbstractStatTemplate<float>
    {
        protected AbstractStatTemplate_Float(float? min, float? max, float? precision, float defaultValue, int decimalsOfPrecisionForRounding)
            : base(new StatLegalizer_Float(min, max, precision, decimalsOfPrecisionForRounding), defaultValue) { }
    }
}