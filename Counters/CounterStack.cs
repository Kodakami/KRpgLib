using System;
using System.Collections.Generic;

namespace KRpgLib.Counters
{
    public sealed class CounterStack
    {
        public Counter Counter { get; }
        public object Origin { get; }

        private int _instanceCount;

        public ICounterTemplate Template => Counter.Template;

        public int TicksCounted { get; private set; }
        public int InstanceCount { get; private set; }
        public int MaxInstanceCount => Counter.MaxInstanceCount;
        public bool IsAtMaxInstanceCount => _instanceCount == Counter.MaxInstanceCount;

        public bool IsVisibleToUsers => Counter.IsVisibleToUsers;
        public TrackByOriginDecision TrackByOrigin => Counter.TrackByOrigin;

        public CounterStack(Counter counter, object origin)
        {
            Counter = counter ?? throw new ArgumentNullException(nameof(counter));
            Origin = origin;
            _instanceCount = 0;
            TicksCounted = 0;
        }

        /// <summary>
        /// Returns the number successfully added.
        /// </summary>
        public int AddInstances(CounterManager counterManager, int count, CounterAdditionReason additionReason)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            int added = InstanceCount + count > MaxInstanceCount ? MaxInstanceCount - InstanceCount : count;

            InstanceCount += added;

            Counter.OnCounterInstanceAdded(counterManager, this, additionReason);

            return added;
        }

        /// <summary>
        /// Returns the number successfully removed.
        /// </summary>
        public int RemoveInstances(CounterManager counterManager, int count, CounterRemovalReason removalReason)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            int removed = InstanceCount - count < 0 ? InstanceCount : count;

            InstanceCount -= removed;

            Counter.OnCounterInstanceRemoved(counterManager, this, removalReason);

            return removed;
        }

        /// <summary>
        /// Returns the number successfully removed.
        /// </summary>
        public int RemoveAllInstances(CounterManager counterManager, CounterRemovalReason removalReason)
        {
            int removed = InstanceCount;
            _instanceCount = 0;

            Counter.OnCounterInstanceRemoved(counterManager, this, removalReason);

            return removed;
        }

        public void AddTicksCounted(int tickCount) => TicksCounted += tickCount;
        public void RemoveTicksCounted(int tickCount) => TicksCounted -= tickCount;
        public void ResetTicksCounted() => TicksCounted = 0;

        public TComponent GetComponent<TComponent>() where TComponent : CounterComponent => Counter.GetComponent<TComponent>();
    }
}
