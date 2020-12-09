using System;
using System.Collections.Generic;
using System.Text;

namespace KRpgLib.Counters
{
    public abstract class CounterComponent
    {
        public virtual void OnCounterTick(CounterManager counterManager, CounterStack counterStack, int numberOfTicks) { }
        public virtual void OnCounterStackCreated(CounterManager counterManager, CounterStack counterStack, CounterAdditionReason additionReason) { }
        public virtual void OnCounterInstanceAdded(CounterManager counterManager, CounterStack counterStack, CounterAdditionReason additionReason) { }
        public virtual void OnCounterInstanceRemoved(CounterManager counterManager, CounterStack counterStack, CounterRemovalReason removalReason) { }
        public virtual void OnCounterStackDestroyed(CounterManager counterManager, CounterStack counterStack, CounterRemovalReason removalReason) { }
    }

    /// <summary>
    /// If a counter component has this, then a counter can have multiple instances of the component.
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class AllowMultipleComponentInstancesAttribute : Attribute { }

    /// <summary>
    /// If a counter component has this, then a counter must already have at least one registered instance of the provided component type before this component can be registered.
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class RequireCounterComponentAttribute : Attribute
    {
        public Type RequiredType { get; }

        public RequireCounterComponentAttribute(Type requiredType)
        {
            if (requiredType == null)
            {
                throw new ArgumentNullException(nameof(requiredType));
            }
            if (!requiredType.IsSubclassOf(typeof(CounterComponent)))
            {
                throw new ArgumentException("Argument must be a subclass of CounterComponent.", nameof(requiredType));
            }

            RequiredType = requiredType;
        }
    }
}
