using KRpgLib.Stats;
using System;

namespace KRpgLib.Affixes
{
    public sealed class StatDeltaValueBounds
    {
        public StatDeltaValueBounds(Stat stat, DeltaType deltaType, int minRollValue, int maxRollValue, int rollPrecision = 1)
        {
            Stat = stat ?? throw new ArgumentNullException(nameof(stat));
            DeltaType = deltaType ?? throw new ArgumentNullException(nameof(deltaType));
            MinRollValue = minRollValue;
            MaxRollValue = maxRollValue;
            RollPrecision = rollPrecision;
        }

        public Stat Stat { get; }
        public DeltaType DeltaType { get; }
        public int MinRollValue { get; }
        public int MaxRollValue { get; }
        public int RollPrecision { get; }
    }
}
