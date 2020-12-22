using KRpgLib.Stats;

namespace KRpgLib.Counters
{
    public class StatDeltaComponent<TValue> : CounterComponent, IStatProvider<TValue> where TValue : struct
    {
        protected StatDeltaCollection<TValue> StatDeltaCollection { get; }
        public StatDeltaComponent(StatDeltaCollection<TValue> statDeltaCollection)
        {
            StatDeltaCollection = statDeltaCollection ?? throw new System.ArgumentNullException(nameof(statDeltaCollection));
        }
        public StatDeltaCollection<TValue> GetStatDeltaCollection()
        {
            return StatDeltaCollection;
        }
    }
}
