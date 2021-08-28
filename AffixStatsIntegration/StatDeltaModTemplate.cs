using KRpgLib.Affixes;
using KRpgLib.Stats;
using System;
using System.Linq;
using System.Collections.Generic;

namespace KRpgLib.AffixStatsIntegration
{
    public sealed class StatDeltaModEffect<TValue> : IModEffect where TValue : struct
    {
        public StatDeltaCollection<TValue> StatDeltas { get; }

        public StatDeltaModEffect(IEnumerable<StatTemplateAndDelta<TValue>> statDeltas)
        {
            StatDeltas = new StatDeltaCollection<TValue>(statDeltas);
        }
    }
    public abstract class StatDeltaModTemplate<TValue, TStatDeltaValueBounds>: ModTemplate<IReadOnlyList<TValue>>
        where TValue : struct
        where TStatDeltaValueBounds : StatDeltaValueBounds<TValue>
    {
        // List of info to create stat deltas within a range.
        // Example list might contain:
        //      (MaxLife, Additional, (min: 3, max: 7, prec: 1))
        //      (ArmourEvasionLocal, Multiply, (min: 5, max: 10, prec: 1))
        // In PoE would look like (one mod):
        //      +(3-7) Maximum Life
        //      +(5-10)% increased Armour and Evasion Rating

        public IReadOnlyList<TStatDeltaValueBounds> ArgBounds { get; }

        protected StatDeltaModTemplate(IReadOnlyList<TStatDeltaValueBounds> argBounds)
        {
            ArgBounds = new List<TStatDeltaValueBounds>(argBounds);
        }

        public override IReadOnlyList<TValue> GetNewArg(Random rng, Mod<IReadOnlyList<TValue>> mod)
        {
            return new List<TValue>(ArgBounds.Select(ab => GenerateRandomValueWithinBounds(rng, ab)));
        }
        protected abstract TValue GenerateRandomValueWithinBounds(Random rng, TStatDeltaValueBounds bounds);

        public override IModEffect GetModEffect(Mod<IReadOnlyList<TValue>> modInstance)
        {
            var modArgValues = modInstance.StrongArg;

            var deltaList = new List<StatTemplateAndDelta<TValue>>();
            for (int i = 0; i < ArgBounds.Count; i++)
            {
                var thisSubArg = ArgBounds[i];
                var thisTemplate = thisSubArg.StatTemplate;
                var thisDeltaType = thisSubArg.DeltaType;
                var thisValue = modArgValues[i];

                deltaList.Add(new StatTemplateAndDelta<TValue>(thisTemplate, new StatDelta<TValue>(thisDeltaType, thisValue)));
            }

            return new StatDeltaModEffect<TValue>(deltaList);
        }
    }
    public abstract class StatDeltaValueBounds<TValue> where TValue : struct
    {
        protected StatDeltaValueBounds(IStatTemplate<TValue> statTemplate, StatDeltaType<TValue> deltaType, TValue minValue, TValue maxValue, TValue precision)
        {
            StatTemplate = statTemplate;
            DeltaType = deltaType;
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
            :base(statTemplate, deltaType, minValue, maxValue, precision)
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
    public sealed class StatDeltaModTemplate_Int : StatDeltaModTemplate<int, StatDeltaValueBounds_Int>
    {
        public StatDeltaModTemplate_Int(IReadOnlyList<StatDeltaValueBounds_Int> argBounds) : base(argBounds) { }

        protected override int GenerateRandomValueWithinBounds(Random rng, StatDeltaValueBounds_Int bounds)
        {
            // System.Random uses exclusive upper bound.
            int raw = rng.Next(bounds.MinValue, bounds.MaxValue + 1);

            return StatUtilities.LegalizeIntValue(raw, bounds.MinValue, bounds.MaxValue, bounds.Precision);
        }
    }
    public sealed class StatDeltaModTemplate_Float : StatDeltaModTemplate<float, StatDeltaValueBounds_Float>
    {
        public StatDeltaModTemplate_Float(IReadOnlyList<StatDeltaValueBounds_Float> argBounds) : base(argBounds) { }

        protected override float GenerateRandomValueWithinBounds(Random rng, StatDeltaValueBounds_Float bounds)
        {
            double randRoll = rng.NextDouble();

            // Linear interpolation with random double for t.
            float rawValue = (float)(bounds.MinValue + (randRoll * (bounds.MaxValue - bounds.MinValue)));

            return StatUtilities.LegalizeFloatValue(rawValue, bounds.MinValue, bounds.MaxValue, bounds.Precision, bounds.DecimalsOfPrecisionForRounding);
        }
    }
}
