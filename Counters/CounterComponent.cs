using KRpgLib.Utility.KomponentObject;

namespace KRpgLib.Counters
{
    public abstract class CounterComponent : Komponent
    {
        public virtual void OnCounterTick(CounterManager counterManager, CounterStack counterStack, int numberOfTicks) { }
        public virtual void OnCounterStackCreated(CounterManager counterManager, CounterStack counterStack, CounterAdditionReason additionReason) { }
        public virtual void OnCounterInstanceAdded(CounterManager counterManager, CounterStack counterStack, CounterAdditionReason additionReason) { }
        public virtual void OnCounterInstanceRemoved(CounterManager counterManager, CounterStack counterStack, CounterRemovalReason removalReason) { }
        public virtual void OnCounterStackDestroyed(CounterManager counterManager, CounterStack counterStack, CounterRemovalReason removalReason) { }
    }
}
