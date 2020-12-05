using System.Collections.Generic;
namespace KRpgLib.Counters
{
    public class CounterTemplate
    {
        private readonly Dictionary<System.Type, CounterComponent> _componentDict = new Dictionary<System.Type, CounterComponent>();
        public CounterTemplate(bool isVisibleToUsers, Domain domain, int maxStackSize, TrackByOriginDecision trackByOrigin, IEnumerable<CounterComponent> components)
        {
            IsVisibleToUsers = isVisibleToUsers;
            Domain = domain;
            MaxInstanceCount = maxStackSize;
            TrackByOrigin = trackByOrigin;

            if (components != null)
            {
                foreach (var info in components)
                {
                    RegisterComponent(info);
                }
            }
        }

        // Required components.
        public bool IsVisibleToUsers { get; }
        public Domain Domain { get; }
        public int MaxInstanceCount { get; }
        public TrackByOriginDecision TrackByOrigin { get; }

        protected void RegisterComponent(CounterComponent info)
        {
            if (info == null)
            {
                throw new System.ArgumentNullException(nameof(info));
            }

            _componentDict[info.GetType()] = info;
        }
        protected void UnregisterComponent<TComponent>()
        {
            _componentDict.Remove(typeof(TComponent));
        }
        public TComponent GetComponent<TComponent>() where TComponent : CounterComponent
        {
            if (_componentDict.TryGetValue(typeof(TComponent), out CounterComponent found))
            {
                return (TComponent)found;
            }
            return null;
        }
    }
    public enum TrackByOriginDecision
    {
        ONE_STACK_REGARDLESS_OF_ORIGIN = 0,
        EACH_ORIGIN_GETS_UNIQUE_STACK = 1,
    }
}
