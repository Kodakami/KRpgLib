using System;
using System.Collections.Generic;

namespace KRpgLib.Counters
{
    public class CounterManager
    {
        private readonly List<Counter> _counters = new List<Counter>();

        protected virtual int MaxRecursionDepthForLinkedCounterTriggering => 7;

        public void AddCountersSilently(CounterTemplate counterTemplate, object origin, int numberOfInstances, Random rng)
        {
            AddCountersRecursive(counterTemplate, origin, rng, numberOfInstances, CounterAdditionReason.SILENT_AND_ABSOLUTE, 0);
        }
        public void AddCounters(CounterTemplate counterTemplate, object origin, int numberOfInstances, Random rng)
        {
            AddCountersRecursive(counterTemplate, origin, rng, numberOfInstances, CounterAdditionReason.DIRECTLY_APPLIED, 0);
        }
        public void RemoveCountersSilently(CounterTemplate counterTemplate, object origin, int numberOfInstances, Random rng)
        {
            RemoveCountersRecursive(counterTemplate, origin, rng, numberOfInstances, CounterRemovalReason.SILENT_AND_ABSOLUTE, 0);
        }
        public void TryCure(CounterTemplate counterTemplate, object origin, int numberOfInstances, Random rng)
        {
            RemoveCountersRecursive(counterTemplate, origin, rng, numberOfInstances, CounterRemovalReason.CURED_BY_USER_EFFECT, 0);
        }
        public void TryCancel(CounterTemplate counterTemplate, object origin, int numberOfInstances, Random rng)
        {
            RemoveCountersRecursive(counterTemplate, origin, rng, numberOfInstances, CounterRemovalReason.CANCELLED_BY_USER, 0);
        }
        protected void AddCountersRecursive(CounterTemplate template, object origin, Random rng, int instanceCount, CounterAdditionReason additionReason, int recursionDepth)
        {
            if (recursionDepth > MaxRecursionDepthForLinkedCounterTriggering)
            {
                return;
            }

            var counter = FindOrCreateCounterStack(
                template, origin,
                template.TrackByOrigin == TrackByOriginDecision.EACH_ORIGIN_GETS_UNIQUE_STACK,
                rng, additionReason, recursionDepth);

            counter.AddInstances(instanceCount);
        }
        protected void RemoveCountersRecursive(CounterTemplate template, object origin, Random rng, int instanceCount, CounterRemovalReason removalReason, int recursionDepth)
        {
            if (recursionDepth > MaxRecursionDepthForLinkedCounterTriggering)
            {
                return;
            }

            var counter = FindExistingCounterOrNull(template, origin, template.TrackByOrigin == TrackByOriginDecision.EACH_ORIGIN_GETS_UNIQUE_STACK);
            if (counter != null)
            {
                counter.RemoveInstances(instanceCount);
                if (counter.InstanceCount == 0)
                {
                    DestroyCounterStack(counter, rng, removalReason, recursionDepth);
                }
            }
        }
        protected virtual Counter FindOrCreateCounterStack(
            CounterTemplate template, object origin, bool mustBeOriginSpecific, Random rng, CounterAdditionReason additionReason, int recursionDepth)
        {
             return FindExistingCounterOrNull(template, origin, mustBeOriginSpecific) ??
                CreateNewCounterStack(template, origin, rng, additionReason, recursionDepth);
        }
        protected virtual Counter FindExistingCounterOrNull(CounterTemplate template, object origin, bool mustBeOriginSpecific)
        {
            Predicate<Counter> predicate;
            if (mustBeOriginSpecific)
            {
                predicate = c => c.Template == template && c.Origin == origin;
            }
            else
            {
                predicate = c => c.Template == template;
            }

            return _counters.Find(predicate);
        }
        protected virtual Counter CreateNewCounterStack(CounterTemplate template, object origin, Random rng, CounterAdditionReason additionReason, int recursionDepth)
        {
            var counter = new Counter(template, origin);

            // Gain this counter stack.
            _counters.Add(counter);

            OnNewCounterStackCreated(counter, rng, additionReason, recursionDepth);

            return counter;
        }
        protected virtual void OnNewCounterStackCreated(Counter counter, Random rng, CounterAdditionReason additionReason, int recursionDepth)
        {
            var linkedCounters = counter.GetComponent<LinkedCounterComponent>();
            if (linkedCounters != null)
            {
                // Removed on addition.
                foreach (var lc in linkedCounters.GetCountersRemovedDuringAddition(counter, rng, additionReason))
                {
                    RemoveCountersRecursive(lc.Key, counter.Origin, rng, lc.Value, CounterRemovalReason.REMOVED_BY_OTHER_COUNTER, recursionDepth + 1);
                }

                // Added on addition.
                foreach (var lc in linkedCounters.GetCountersAddedDuringAddition(counter, rng, additionReason))
                {
                    AddCountersRecursive(lc.Key, counter.Origin, rng, lc.Value, CounterAdditionReason.ADDED_BY_OTHER_COUNTER, recursionDepth + 1);
                }
            }
        }
        protected virtual void DestroyCounterStack(Counter counter, Random rng, CounterRemovalReason removalReason, int recursionDepth)
        {
            // Obliterate this counter stack.
            _counters.Remove(counter);

            OnCounterStackDestroyed(counter, rng, removalReason, recursionDepth);
        }
        protected virtual void OnCounterStackDestroyed(Counter counter, Random rng, CounterRemovalReason removalReason, int recursionDepth)
        {
            var linkedCounters = counter.GetComponent<LinkedCounterComponent>();
            if (linkedCounters != null)
            {
                // Removed on removal.
                foreach (var lc in linkedCounters.GetCountersRemovedDuringRemoval(counter, rng, removalReason))
                {
                    RemoveCountersRecursive(lc.Key, counter.Origin, rng, lc.Value, CounterRemovalReason.REMOVED_BY_OTHER_COUNTER, recursionDepth + 1);
                }

                // Added on removal.
                foreach (var lc in linkedCounters.GetCountersAddedDuringRemoval(counter, rng, removalReason))
                {
                    AddCountersRecursive(lc.Key, counter.Origin, rng, lc.Value, CounterAdditionReason.ADDED_BY_OTHER_COUNTER, recursionDepth + 1);
                }
            }
        }
    }
}
