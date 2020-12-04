using KRpgLib.Stats;

namespace KRpgLib.Counters
{
    public struct StatDeltaInfo<TValue> : IStatProvider<TValue> where TValue : struct
    {
        public StatDeltaCollection<TValue> StatDeltaCollection { get; }
        public StatDeltaInfo(StatDeltaCollection<TValue> statDeltaCollection)
        {
            StatDeltaCollection = statDeltaCollection ?? throw new System.ArgumentNullException(nameof(statDeltaCollection));
        }

        StatDeltaCollection<TValue> IStatProvider<TValue>.GetStatDeltaCollection()
        {
            return StatDeltaCollection;
        }
    }
}
