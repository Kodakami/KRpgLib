using System.Collections.Generic;
using KRpgLib.Utility.KomponentObject;
namespace KRpgLib.Counters
{
    public interface ICounterTemplate : IKomponentObjectTemplate
    {
        Counter CreateCounter();
    }
    public interface ICounterTemplate<TCounter> : ICounterTemplate
        where TCounter : Counter
    {
        new TCounter CreateCounter();
    }
    public class Counter : KomponentObject<ICounterTemplate, CounterComponent>
    {
        new public ICounterTemplate Template { get; }

        public Counter(ICounterTemplate template, bool isVisibleToUsers, int maxInstanceCount, TrackByOriginDecision trackByOrigin, IEnumerable<CounterComponent> components)
            :base(template, components)
        {
            IsVisibleToUsers = isVisibleToUsers;
            MaxInstanceCount = maxInstanceCount;
            TrackByOrigin = trackByOrigin;
        }

        public bool IsVisibleToUsers { get; }
        public int MaxInstanceCount { get; }
        public TrackByOriginDecision TrackByOrigin { get; }

        public void OnCounterTick(CounterManager counterManager, CounterStack counterStack, int numberOfTicks)
            => PerformCallback(com => com.OnCounterTick(counterManager, counterStack, numberOfTicks));
        public void OnCounterStackCreated(CounterManager counterManager, CounterStack stack, CounterAdditionReason additionReason)
            => PerformCallback(com => com.OnCounterStackCreated(counterManager, stack, additionReason));
        public void OnCounterInstanceAdded(CounterManager counterManager, CounterStack counterStack, CounterAdditionReason additionReason)
            => PerformCallback(com => com.OnCounterInstanceAdded(counterManager, counterStack, additionReason));
        public void OnCounterInstanceRemoved(CounterManager counterManager, CounterStack counterStack, CounterRemovalReason removalReason)
            => PerformCallback(com => com.OnCounterInstanceRemoved(counterManager, counterStack, removalReason));
        public void OnCounterStackDestroyed(CounterManager counterManager, CounterStack stack, CounterRemovalReason removalReason)
            => PerformCallback(com => com.OnCounterStackDestroyed(counterManager, stack, removalReason));

        private void PerformCallback(System.Action<CounterComponent> callback)
        {
            void forEach(KeyValuePair<System.Type, List<CounterComponent>> kvp)
            {
                foreach (var com in kvp.Value)
                {
                    callback.Invoke(com);
                }
            }

            ForEachKomponent(forEach);
        }
    }
    public enum TrackByOriginDecision
    {
        ONE_STACK_REGARDLESS_OF_ORIGIN = 0,
        EACH_ORIGIN_GETS_UNIQUE_STACK = 1,
    }
}
