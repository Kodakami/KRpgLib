using KRpgLib.Affixes;
using KRpgLib.Stats;
using System.Collections.Generic;

namespace KRpgLib.AffixesStatsIntegration
{
    public sealed class StatDeltaModEffect<TValue> : IModEffect where TValue : struct
    {
        public StatDeltaCollection<TValue> StatDeltaCollection { get; }

        public StatDeltaModEffect(StatDeltaCollection<TValue> statDeltaCollection)
        {
            StatDeltaCollection = statDeltaCollection;
        }

        public static implicit operator StatDeltaCollection<TValue>(StatDeltaModEffect<TValue> modEffect) => modEffect.StatDeltaCollection;
    }
}
