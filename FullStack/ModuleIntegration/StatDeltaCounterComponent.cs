using KRpgLib.Stats;

namespace KRpgLib.Counters
{
    public class StatDeltaCounterComponent : CounterComponent, IStatProvider
    {
        protected DeltaCollection StatDeltaCollection { get; }
        public StatDeltaCounterComponent(DeltaCollection statDeltaCollection)
        {
            StatDeltaCollection = statDeltaCollection ?? throw new System.ArgumentNullException(nameof(statDeltaCollection));
        }
        public DeltaCollection GetDeltaCollection()
        {
            return StatDeltaCollection;
        }
    }
}
