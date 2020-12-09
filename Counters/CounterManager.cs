using System;
using System.Collections.Generic;

namespace KRpgLib.Counters
{
    public class CounterManager
    {
        private readonly List<CounterStack> _counterStacks = new List<CounterStack>();

        protected virtual int MaxRecursionDepthForLinkedCounterTriggering => 7;

        /// <summary>
        /// Returns the number of instances added.
        /// </summary>
        public int AddCounters(Counter counter, object origin, int instanceCount, CounterAdditionReason additionReason)
        {
            var stack = FindOrCreateStack(counter, origin, additionReason);

            return stack.AddInstances(this, instanceCount, additionReason);
        }
        /// <summary>
        /// Returns the number of instances removed.
        /// </summary>
        public int RemoveInstances(Counter counter, object origin, int instanceCount, CounterRemovalReason removalReason)
        {
            var stack = FindExistingStackOrNull(counter, origin);
            if (stack != null)
            {
                int removedCount = stack.RemoveInstances(this, instanceCount, removalReason);

                if (stack.InstanceCount == 0)
                {
                    DestroyStack(stack, removalReason);
                }
                return removedCount;
            }
            return 0;
        }
        /// <summary>
        /// Returns the number of instances removed.
        /// </summary>
        public int RemoveStack(Counter counter, object origin, CounterRemovalReason removalReason)
        {
            var stack = FindExistingStackOrNull(counter, origin);
            if (stack != null)
            {
                int removedCount = stack.RemoveAllInstances(this, removalReason);
                DestroyStack(stack, removalReason);
                return removedCount;
            }
            return 0;
        }
        /// <summary>
        /// Returns the total number of instances removed.
        /// </summary>
        public int RemoveAllStacksWhere(Predicate<CounterStack> predicate, CounterRemovalReason removalReason)
        {
            int instancesRemoved = 0;
            foreach (var stack in FindAllExistingStacks(predicate))
            {
                instancesRemoved += stack.RemoveAllInstances(this, removalReason);

                if (stack.InstanceCount == 0)
                {
                    DestroyStack(stack, removalReason);
                }
            }
            return instancesRemoved;
        }
        protected virtual CounterStack FindOrCreateStack(
            Counter counter, object origin, CounterAdditionReason additionReason)
        {
            var stack = FindExistingStackOrNull(counter, origin);
            if (stack != null)
            {
                return stack;
            }

            if (counter.TrackByOrigin == TrackByOriginDecision.ONE_STACK_REGARDLESS_OF_ORIGIN)
            {
                var existingStackWithTemplate = _counterStacks.Find(c => c.Template == counter.Template);
                if (existingStackWithTemplate != null)
                {
                    // One stack exists and only one may exist, regardless of origin.
                    DestroyStack(existingStackWithTemplate, CounterRemovalReason.OVERWRITTEN_BY_NEWER_COUNTER);
                }
            }

            return CreateNewStack(counter, origin, additionReason);
        }
        protected virtual CounterStack FindExistingStackOrNull(Counter counter, object origin)
        {
            return _counterStacks.Find(c => c.Template == counter && c.Origin == origin);
        }
        protected virtual List<CounterStack> FindAllExistingStacks(Counter counter)
        {
            return FindAllExistingStacks(s => s.Template == counter.Template);
        }
        protected virtual List<CounterStack> FindAllExistingStacks(Predicate<CounterStack> predicate)
        {
            return _counterStacks.FindAll(predicate);
        }
        protected virtual CounterStack CreateNewStack(Counter counter, object origin, CounterAdditionReason additionReason)
        {
            var stack = new CounterStack(counter, origin);

            // Gain this counter stack.
            _counterStacks.Add(stack);

            // OnCounterStackCreated callback happens.
            stack.Counter.OnCounterStackCreated(this, stack, additionReason);

            return stack;
        }
        protected virtual void DestroyStack(CounterStack stack, CounterRemovalReason removalReason)
        {
            // Obliterate this counter stack.
            _counterStacks.Remove(stack);

            stack.Counter.OnCounterStackDestroyed(this, stack, removalReason);
        }
        protected virtual void DestroyStacks(IEnumerable<CounterStack> stacks, CounterRemovalReason removalReason)
        {
            foreach (var stack in stacks)
            {
                _counterStacks.Remove(stack);
                stack.Counter.OnCounterStackDestroyed(this, stack, removalReason);
            }
        }
    }
}
