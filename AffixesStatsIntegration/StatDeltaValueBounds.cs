using KRpgLib.Stats;
using System;

namespace KRpgLib.AffixesStatsIntegration
{
    public abstract class StatDeltaValueBounds<TValue> where TValue : struct
    {
        protected StatDeltaValueBounds(IStatTemplate<TValue> statTemplate, StatDeltaType<TValue> deltaType, TValue minValue, TValue maxValue, TValue precision)
        {
            StatTemplate = statTemplate ?? throw new ArgumentNullException(nameof(statTemplate));
            DeltaType = deltaType ?? throw new ArgumentNullException(nameof(deltaType));
            MinValue = minValue;
            MaxValue = maxValue;
            Precision = precision;
        }

        public IStatTemplate<TValue> StatTemplate { get; }
        public StatDeltaType<TValue> DeltaType { get; }
        public TValue MinValue { get; }
        public TValue MaxValue { get; }
        public TValue Precision { get; }
    }
    public sealed class StatDeltaValueBounds_Int : StatDeltaValueBounds<int>
    {
        public StatDeltaValueBounds_Int(
            IStatTemplate<int> statTemplate,
            StatDeltaType<int> deltaType,
            int minValue, int maxValue, int precision
            )
            : base(statTemplate, deltaType, minValue, maxValue, precision)
        { }
    }
    public sealed class StatDeltaValueBounds_Float : StatDeltaValueBounds<float>
    {
        public StatDeltaValueBounds_Float(
            IStatTemplate<float> statTemplate,
            StatDeltaType<float> deltaType,
            float minValue, float maxValue, float precision, int decimalsOfPrecisionForRounding
            )
            : base(statTemplate, deltaType, minValue, maxValue, precision)
        {
            DecimalsOfPrecisionForRounding = decimalsOfPrecisionForRounding;
        }

        public int DecimalsOfPrecisionForRounding { get; }
    }
}
