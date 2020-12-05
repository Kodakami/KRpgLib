using System;
using System.Collections.Generic;

namespace KRpgLib.Counters
{
    public sealed class Counter
    {
        private int _instanceCount;

        public CounterTemplate Template { get; }
        public object Origin { get; }

        public int TicksCounted { get; private set; }
        public int InstanceCount
        {
            get
            {
                return _instanceCount;
            }
            private set
            {
                _instanceCount = Math.Min(value, Math.Max(Template.MaxInstanceCount, 0));
            }
        }
        public int MaxInstanceCount => Template.MaxInstanceCount;
        public bool IsAtMaxInstanceCount => _instanceCount == Template.MaxInstanceCount;

        public Counter(CounterTemplate template, object origin)
        {
            Template = template ?? throw new ArgumentNullException(nameof(template));
            Origin = origin;
            _instanceCount = 0;
            TicksCounted = 0;
        }

        public void AddInstances(int count) => InstanceCount += count;
        public void RemoveInstances(int count) => InstanceCount -= count;
        public void RemoveAllInstances() => _instanceCount = 0;

        public void AddTicksCounted(int tickCount) => TicksCounted += tickCount;
        public void RemoveTicksCounted(int tickCount) => TicksCounted -= tickCount;
        public void ResetTicksCounted() => TicksCounted = 0;

        public TComponent GetComponent<TComponent>() where TComponent : CounterComponent => Template.GetComponent<TComponent>();
    }
}
