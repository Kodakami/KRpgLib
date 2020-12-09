using System.Collections.Generic;
namespace KRpgLib.Counters
{
    public interface ICounterTemplate
    {
        Counter CreateCounter(object origin);
    }
    public interface ICounterTemplate<TCounter> : ICounterTemplate where TCounter : Counter
    {
        new TCounter CreateCounter(object origin);
    }
    public class Counter
    {
        private readonly Dictionary<System.Type, List<CounterComponent>> _componentDict = new Dictionary<System.Type, List<CounterComponent>>();

        public ICounterTemplate Template { get; }

        public Counter(bool isVisibleToUsers, int maxInstanceCount, TrackByOriginDecision trackByOrigin, IEnumerable<CounterComponent> components)
        {
            IsVisibleToUsers = isVisibleToUsers;
            MaxInstanceCount = maxInstanceCount;
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
        public int MaxInstanceCount { get; }
        public TrackByOriginDecision TrackByOrigin { get; }

        protected void RegisterComponent<TComponent>(TComponent component) where TComponent : CounterComponent
        {
            /* 
             *  This method uses reflection to check attributes on the provided TComponent.
             *  It is expected to be used between 10 and 200 times (on startup) in the average program that uses it.
             *  This will add at most a handful of milliseconds to total startup time in exchange for ease of use and clarity.
             */

            // No null components.
            if (component == null)
            {
                throw new System.ArgumentNullException(nameof(component));
            }

            // Get the type of component.
            var typeArg = typeof(TComponent);

            // No components of abstract base type.
            if (typeArg.Equals(typeof(CounterComponent)))
            {
                throw new System.ArgumentException("Component may not be of base class CounterComponent.", nameof(component));
            }

            // This value may be changed by an attribute later on down.
            bool allowMultipleInstances = false;

            // For each of the custom attributes on the component type.
            foreach (var cAtt in typeArg.GetCustomAttributes(true))
            {
                // Grab a handle for the attribute's type.
                var attType = cAtt.GetType();

                // If it is a required component attribute,
                if (attType.Equals(typeof(RequireCounterComponentAttribute)))
                {
                    // Cast to known type.
                    RequireCounterComponentAttribute rcca = (RequireCounterComponentAttribute)cAtt;

                    // If there is no key in the component dictionary (no required component registered at this point),
                    if (!_componentDict.ContainsKey(rcca.RequiredType))
                    {
                        // Throw out.
                        throw new System.ArgumentException($"Counter has no registered components of required type: {rcca.RequiredType}.", nameof(component));
                    }
                }
                // Otherwise if multiple instances are not yet allowed AND it is an allow multiple instances attribute,
                else if (!allowMultipleInstances && attType.Equals(typeof(AllowMultipleComponentInstancesAttribute)))
                {
                    // Set the flag for that (and stop checking).
                    allowMultipleInstances = true;
                }
            }

            // If this type of component hasn't been registered yet (make a handle for the related list),
            if (!_componentDict.TryGetValue(typeArg, out List<CounterComponent> componentList))
            {
                // Make a new list.
                componentList = new List<CounterComponent>();

                // Add it as a new entry in the dictionary.
                _componentDict.Add(typeArg, componentList);
            }
            // Otherwise (it has been defined),
            else
            {
                // If multiple instances are not allowed and there is already an instance.
                if (!allowMultipleInstances && componentList.Count > 0)
                {
                    // Throw out.
                    // There may be a need to change this functionality in the future if it ends up being more practical to replace the old component instead.
                    throw new System.ArgumentException($"Counter already contains a component of type \"{typeArg}\", and only one instance of this type is allowed.", nameof(component));
                }
            }

            // Add the provided component to the list.
            componentList.Add(component);
        }
        protected void UnregisterComponent(CounterComponent component)
        {
            var key = component.GetType();

            _componentDict[key].Remove(component);

            if (_componentDict[key].Count == 0)
            {
                _componentDict.Remove(key);
            }
        }
        public TComponent GetComponent<TComponent>() where TComponent : CounterComponent
        {
            if (_componentDict.TryGetValue(typeof(TComponent), out List<CounterComponent> found))
            {
                return (TComponent)found[0];    // No need to check for empty list or null. Impossible internal state.
            }
            return null;
        }
        public List<TComponent> GetComponents<TComponent>() where TComponent : CounterComponent
        {
            if (_componentDict.TryGetValue(typeof(TComponent), out List<CounterComponent> found))
            {
                return found.ConvertAll(com => (TComponent)com);
            }
            return null;
        }
        public List<CounterComponent> GetAllComponents()
        {
            var list = new List<CounterComponent>();
            foreach (var kvp in _componentDict)
            {
                foreach (var com in kvp.Value)
                {
                    list.Add(com);
                }
            }
            return list;
        }

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
            foreach (var kvp in _componentDict)
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
