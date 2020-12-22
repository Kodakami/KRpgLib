using System;
using KRpgLib.Stats;

namespace KRpgLib.Affixes.ModTemplates
{
    // Later, make a compound version of this that has multiple rolled values in one mod (like minimum and maximum damage values per hit).
    public abstract class MTStatDelta<TValue> : IModTemplate<TValue> where TValue : struct
    {
        private readonly RollLegalizer _rollLegalizer;

        public IStatTemplate<TValue> AffectedStatTemplate { get; }
        public StatDeltaType<TValue> StatDeltaType { get; }
        public TValue MinimumRoll => _rollLegalizer.Min;
        public TValue MaximumRoll => _rollLegalizer.Max;
        public TValue? RollPrecision => _rollLegalizer.Precision;

        protected MTStatDelta(IStatTemplate<TValue> affectedStatTemplate, StatDeltaType<TValue> statDeltaType, RollLegalizer rollLegalizer)
        {
            AffectedStatTemplate = affectedStatTemplate;
            StatDeltaType = statDeltaType;
            _rollLegalizer = rollLegalizer;
        }
        public TValue GetNewRolledResult() => _rollLegalizer.GetRandomLegalValue();

        protected abstract class RollLegalizer
        {
            public TValue Min { get; }
            public TValue Max { get; }
            public TValue? Precision { get; }

            protected RollLegalizer(TValue min, TValue max, TValue? precision)
            {
                Min = min;
                Max = max;
                Precision = precision;
            }
            public abstract TValue GetRandomLegalValue();
        }
    }
    public class MTStatDelta_Int : MTStatDelta<int>
    {
        public MTStatDelta_Int(IStatTemplate<int> affectedStatTemplate, StatDeltaType<int> statDeltaType, int minRoll, int maxRoll, int? rollPrecision) :
            base(affectedStatTemplate, statDeltaType, new RollLegalizer_Int(minRoll, maxRoll, rollPrecision))
        { }
        private sealed class RollLegalizer_Int : RollLegalizer
        {
            public RollLegalizer_Int(int min, int max, int? precision)
                : base(
                     min,
                     max >= min ? max : throw new ArgumentException("Maximum value of stat delta must be greater than or equal to its minimum value."),
                     precision.HasValue ? precision >= 1 ? precision : null : null)
            { }
            public override int GetRandomLegalValue()
            {
                // Constructor ensures that Precision always has value >= 1 or null.

                int appliedPrecision = Precision ?? 1;

                int lowerBound = Min / appliedPrecision;
                int upperBound = Max / appliedPrecision;

                int randomValue = Utility.Environment.Rng.Next(lowerBound, upperBound + 1);

                return randomValue * appliedPrecision;
            }
        }
    }
    public class MTStatDelta_Float : MTStatDelta<float>
    {
        public MTStatDelta_Float(
            IStatTemplate<float> affectedStatTemplate,
            StatDeltaType<float> statDeltaType,
            float minRoll, float maxRoll, float? rollPrecision)
            : base(affectedStatTemplate, statDeltaType, new RollLegalizer_Float(minRoll, maxRoll, rollPrecision))
        { }
        private sealed class RollLegalizer_Float : RollLegalizer
        {
            public RollLegalizer_Float(float min, float max, float? precision)
                : base(
                     min,
                     max >= min ? max : throw new ArgumentException("Maximum value of stat delta must be greater than or equal to its minimum value."),
                     precision.HasValue ? precision > 0 ? precision : null : null)
            { }
            public override float GetRandomLegalValue()
            {
                // Thanks Nathan.

                float range = Max - Min;

                // Constructor ensures that Precision is always > 0 or null.
                if (Precision.HasValue)
                {
                    int appliedRange = (int)(range / Precision.Value);

                    int randomValue = Utility.Environment.Rng.Next(0, appliedRange + 1);

                    return (float)((randomValue * Precision.Value) + Min);
                }

                float randomFloat = (float)Utility.Environment.Rng.NextDouble();

                return (randomFloat * range) + Min;
            }
        }
    }
}
