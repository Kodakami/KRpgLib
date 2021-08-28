using System.Collections.Generic;
using KRpgLib.Utility.KomponentObject;
using KRpgLib.Utility.TemplateObject;
using System.Linq;

namespace KRpgLib.Counters
{
    public interface ICounterTemplate : ITemplate
    {
        Counter CreateCounter();
    }
    public interface ICounterTemplate<TCounter> : ICounterTemplate
        where TCounter : Counter
    {
        new TCounter CreateCounter();
    }
    public class Counter : KomponentObject<CounterComponent>, ITemplateObject<ICounterTemplate>
    {
        public ICounterTemplate Template { get; }

        public Counter(ICounterTemplate template, bool isVisibleToUsers, int maxInstanceCount, TrackByOriginDecision trackByOrigin, IEnumerable<CounterComponent> components)
            :base(components)
        {
            Template = template;
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
            // "this" is an IEnumerable<KeyValuePair<Type, IReadOnlyList<KomponentBase>>> by being a KomponentObject
            foreach (var kvp in this)
            {
                foreach (var com in kvp.Value)
                {
                    callback.Invoke(com);
                }
            }
        }
    }
    public enum TrackByOriginDecision
    {
        ONE_STACK_REGARDLESS_OF_ORIGIN = 0,
        EACH_ORIGIN_GETS_UNIQUE_STACK = 1,
    }
}
