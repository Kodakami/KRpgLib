using KRpgLib.Affixes;
using KRpgLib.Stats;
using System.Collections.Generic;

namespace KRpgLib.AffixesStatsIntegration
{
    public sealed class StatDeltaModEffect<TValue> : IModEffect where TValue : struct
    {
        public DeltaCollection<TValue> StatDeltaCollection { get; }

        public StatDeltaModEffect(DeltaCollection<TValue> statDeltaCollection)
        {
            StatDeltaCollection = statDeltaCollection;
        }

        public static implicit operator DeltaCollection<TValue>(StatDeltaModEffect<TValue> modEffect) => modEffect.StatDeltaCollection;
    }
}
