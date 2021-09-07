using KRpgLib.Stats;

namespace KRpgLib.Counters
{
    public class StatDeltaCounterComponent<TValue> : CounterComponent, IStatProvider<TValue> where TValue : struct
    {
        protected DeltaCollection<TValue> StatDeltaCollection { get; }
        public StatDeltaCounterComponent(DeltaCollection<TValue> statDeltaCollection)
        {
            StatDeltaCollection = statDeltaCollection ?? throw new System.ArgumentNullException(nameof(statDeltaCollection));
        }
        public DeltaCollection<TValue> GetDeltaCollection()
        {
            return StatDeltaCollection;
        }
    }
}
