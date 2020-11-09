namespace KRpgLib.Stats
{
    public interface IStatTemplate : ILegalizeableNumber<float>
    {
        float DefaultValue { get; }
    }
    public abstract class AbstractStatTemplate : IStatTemplate
    {
        public float DefaultValue { get; }
        protected StatLegalizer StatLegalizer { get; }

        protected AbstractStatTemplate(float? min, float? max, float? precision, float defaultValue)
        {
            StatLegalizer = new StatLegalizer(min, max, precision);
            DefaultValue = defaultValue;
        }
        public float GetLegalizedValue(float rawValue) => StatLegalizer.GetLegalizedValue(rawValue);
    }
}