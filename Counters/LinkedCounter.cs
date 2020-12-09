using System;

namespace KRpgLib.Counters
{
    [AllowMultipleComponentInstances]
    public abstract class LinkedCounter : CounterComponent
    {
        protected LinkedCounter(LinkedCounterAction action, Counter counter, int instanceCount, double chanceToTrigger, bool triggersWhenParentIsALinkedCounter)
        {
            Action = action;
            Counter = counter;
            InstanceCount = instanceCount;
            ChanceToTrigger = chanceToTrigger;

            TriggersWhenParentIsALinkedCounter = triggersWhenParentIsALinkedCounter;
        }

        public LinkedCounterAction Action { get; }
        public Counter Counter { get; }
        public int InstanceCount { get; }
        public double ChanceToTrigger { get; }

        public bool TriggersWhenParentIsALinkedCounter { get; }

        protected bool RollForTrigger()
        {
            return Utility.Environment.Rng.NextDouble() < ChanceToTrigger;
        }
        protected void PerformAction(CounterManager counterManager, object origin)
        {
            if (Action == LinkedCounterAction.ADD_THIS_COUNTER)
            {
                counterManager.AddCounters(Counter, origin, InstanceCount, CounterAdditionReason.ADDED_BY_OTHER_COUNTER);
            }
            else
            {
                counterManager.RemoveInstances(Counter, origin, InstanceCount, CounterRemovalReason.REMOVED_BY_OTHER_COUNTER);
            }
        }
    }
    public class LinkedCounterOnParentStackCreated : LinkedCounter
    {
        protected LinkedCounterOnParentStackCreated(
            LinkedCounterAction linkedCounterAction,
            Counter counter,
            int instanceCount,
            double chanceToTrigger,
            bool triggersWhenParentIsALinkedCounter,
            bool triggersWhenParentIsAppliedDirectly)
            :
            base(linkedCounterAction, counter, instanceCount, chanceToTrigger, triggersWhenParentIsALinkedCounter)
        {
            TriggersWhenParentIsAppliedDirectly = triggersWhenParentIsAppliedDirectly;
        }

        public bool TriggersWhenParentIsAppliedDirectly { get; }

        public override void OnCounterStackCreated(CounterManager counterManager, CounterStack counterStack, CounterAdditionReason additionReason)
        {
            if (((additionReason == CounterAdditionReason.SILENT_AND_ABSOLUTE)
                || (additionReason == CounterAdditionReason.DIRECTLY_APPLIED_BY_EFFECT && TriggersWhenParentIsAppliedDirectly)
                || (additionReason == CounterAdditionReason.ADDED_BY_OTHER_COUNTER && TriggersWhenParentIsALinkedCounter))
                && RollForTrigger())
            {
                PerformAction(counterManager, counterStack.Origin);
            }
        }
    }
    public class LinkedCounterOnParentInstanceAdded : LinkedCounter
    {
        protected LinkedCounterOnParentInstanceAdded(
            LinkedCounterAction linkedCounterAction,
            Counter counter,
            int instanceCount,
            double chanceToTrigger,
            bool triggersWhenParentIsALinkedCounter,
            bool triggersWhenParentIsAppliedDirectly)
            :
            base(linkedCounterAction, counter, instanceCount, chanceToTrigger, triggersWhenParentIsALinkedCounter)
        {
            TriggersWhenParentIsDirectlyApplied = triggersWhenParentIsAppliedDirectly;
        }

        public bool TriggersWhenParentIsDirectlyApplied { get; }

        public override void OnCounterInstanceAdded(CounterManager counterManager, CounterStack counterStack, CounterAdditionReason additionReason)
        {
            if (((additionReason == CounterAdditionReason.SILENT_AND_ABSOLUTE)
                || (additionReason == CounterAdditionReason.DIRECTLY_APPLIED_BY_EFFECT && TriggersWhenParentIsDirectlyApplied)
                || (additionReason == CounterAdditionReason.ADDED_BY_OTHER_COUNTER && TriggersWhenParentIsALinkedCounter))
                && RollForTrigger())
            {
                PerformAction(counterManager, counterStack.Origin);
            }
        }
    }
    public class LinkedCounterOnParentInstanceRemoved : LinkedCounter
    {
        protected LinkedCounterOnParentInstanceRemoved(
            LinkedCounterAction linkedCounterAction,
            Counter counter,
            int instanceCount,
            double chanceToTrigger,
            bool triggersWhenParentIsALinkedCounter,
            bool triggersWhenParentExpiresNaturally,
            bool triggersWhenParentExpiresEarly,
            bool triggersWhenParentIsRemovedByCuring,
            bool triggersWhenParentIsCancelled,
            bool triggersWhenParentIsOverwrittenByNewerInstance)
            :
            base(linkedCounterAction, counter, instanceCount, chanceToTrigger, triggersWhenParentIsALinkedCounter)
        {
            TriggersWhenParentExpiresNaturally = triggersWhenParentExpiresNaturally;
            TriggersWhenParentExpiresEarly = triggersWhenParentExpiresEarly;
            TriggersWhenParentIsDirectlyRemoved = triggersWhenParentIsRemovedByCuring;
            TriggersWhenParentIsCancelled = triggersWhenParentIsCancelled;
            TriggersWhenParentIsOverwrittenByNewerCounter = triggersWhenParentIsOverwrittenByNewerInstance;
        }

        public bool TriggersWhenParentExpiresNaturally { get; }
        public bool TriggersWhenParentExpiresEarly { get; }
        public bool TriggersWhenParentIsDirectlyRemoved { get; }
        public bool TriggersWhenParentIsCancelled { get; }
        public bool TriggersWhenParentIsOverwrittenByNewerCounter { get; }

        public override void OnCounterInstanceRemoved(CounterManager counterManager, CounterStack counterStack, CounterRemovalReason removalReason)
        {
            if ((removalReason == CounterRemovalReason.SILENT_AND_ABSOLUTE
                || (removalReason == CounterRemovalReason.CANCELLED_BY_USER && TriggersWhenParentIsCancelled)
                || (removalReason == CounterRemovalReason.DIRECTLY_REMOVED_BY_EFFECT && TriggersWhenParentIsDirectlyRemoved)
                || (removalReason == CounterRemovalReason.EXPIRED_EARLY && TriggersWhenParentExpiresEarly)
                || (removalReason == CounterRemovalReason.EXPIRED_NATURALLY && TriggersWhenParentExpiresNaturally)
                || (removalReason == CounterRemovalReason.OVERWRITTEN_BY_NEWER_COUNTER && TriggersWhenParentIsOverwrittenByNewerCounter)
                || (removalReason == CounterRemovalReason.REMOVED_BY_OTHER_COUNTER && TriggersWhenParentIsALinkedCounter))
                && RollForTrigger())
            {
                PerformAction(counterManager, counterStack.Origin);
            }
        }
    }
    public class LinkedCounterOnParentStackDestroyed : LinkedCounter
    {
        protected LinkedCounterOnParentStackDestroyed(
            LinkedCounterAction linkedCounterAction,
            Counter counter,
            int instanceCount,
            double chanceToTrigger,
            bool triggersWhenParentIsALinkedCounter,
            bool triggersWhenParentExpiresNaturally,
            bool triggersWhenParentExpiresEarly,
            bool triggersWhenParentIsRemovedByCuring,
            bool triggersWhenParentIsCancelled,
            bool triggersWhenParentIsOverwrittenByNewerInstance)
            :
            base(linkedCounterAction, counter, instanceCount, chanceToTrigger, triggersWhenParentIsALinkedCounter)
        {
            TriggersWhenParentExpiresNaturally = triggersWhenParentExpiresNaturally;
            TriggersWhenParentExpiresEarly = triggersWhenParentExpiresEarly;
            TriggersWhenParentIsDirectlyRemoved = triggersWhenParentIsRemovedByCuring;
            TriggersWhenParentIsCancelled = triggersWhenParentIsCancelled;
            TriggersWhenParentIsOverwrittenByNewerCounter = triggersWhenParentIsOverwrittenByNewerInstance;
        }

        public bool TriggersWhenParentExpiresNaturally { get; }
        public bool TriggersWhenParentExpiresEarly { get; }
        public bool TriggersWhenParentIsDirectlyRemoved { get; }
        public bool TriggersWhenParentIsCancelled { get; }
        public bool TriggersWhenParentIsOverwrittenByNewerCounter { get; }

        public override void OnCounterStackDestroyed(CounterManager counterManager, CounterStack counterStack, CounterRemovalReason removalReason)
        {
            if ((removalReason == CounterRemovalReason.SILENT_AND_ABSOLUTE
                || (removalReason == CounterRemovalReason.CANCELLED_BY_USER && TriggersWhenParentIsCancelled)
                || (removalReason == CounterRemovalReason.DIRECTLY_REMOVED_BY_EFFECT && TriggersWhenParentIsDirectlyRemoved)
                || (removalReason == CounterRemovalReason.EXPIRED_EARLY && TriggersWhenParentExpiresEarly)
                || (removalReason == CounterRemovalReason.EXPIRED_NATURALLY && TriggersWhenParentExpiresNaturally)
                || (removalReason == CounterRemovalReason.OVERWRITTEN_BY_NEWER_COUNTER && TriggersWhenParentIsOverwrittenByNewerCounter)
                || (removalReason == CounterRemovalReason.REMOVED_BY_OTHER_COUNTER && TriggersWhenParentIsALinkedCounter))
                && RollForTrigger())
            {
                PerformAction(counterManager, counterStack.Origin);
            }
        }
    }
    public enum LinkedCounterAction
    {
        ADD_THIS_COUNTER,
        REMOVE_THIS_COUNTER,
    }
    /// <summary>
    /// A reason for removing a counter. Used by linked counters to determine whether to trigger.
    /// </summary>
    public enum CounterRemovalReason
    {
        /// <summary>
        /// Removed by game mechanics. When this is the reason, counters should be removed silently, with as few side effects as possible, and without question.
        /// </summary>
        SILENT_AND_ABSOLUTE = -1,
        /// <summary>
        /// The counter lasted its full maximum duration.
        /// </summary>
        EXPIRED_NATURALLY = 0,
        /// <summary>
        /// The counter successfully rolled to expire earlier than its full maximum duration.
        /// </summary>
        EXPIRED_EARLY = 1,
        /// <summary>
        /// The counter was removed by an effect such as a curing spell or item. This is typically indicative that a counter was detrimental.
        /// </summary>
        DIRECTLY_REMOVED_BY_EFFECT = 2,
        /// <summary>
        /// The counter was cancelled by a user, such as when interrupting their own casting animation. This is typically indicative that a counter was beneficial.
        /// </summary>
        CANCELLED_BY_USER = 3,
        /// <summary>
        /// The counter was overwritten by a newer instance of itself (from a different origin).
        /// </summary>
        OVERWRITTEN_BY_NEWER_COUNTER = 4,
        /// <summary>
        /// The counter was removed by another counter as the latter was being added or removed.
        /// </summary>
        REMOVED_BY_OTHER_COUNTER = 5,
    }
    /// <summary>
    /// A reason for adding a counter. Used by linked counters to determine whether to trigger.
    /// </summary>
    public enum CounterAdditionReason
    {
        /// <summary>
        /// Added by game mechanics. When this is the reason, counters should be added silently, with as few side effects as possible, and without question.
        /// </summary>
        SILENT_AND_ABSOLUTE = -1,
        /// <summary>
        /// The counter was directly applied by a user effect such as a skill or piece of equipment.
        /// </summary>
        DIRECTLY_APPLIED_BY_EFFECT = 0,
        /// <summary>
        /// The counter was added by another counter as the latter was being added or removed.
        /// </summary>
        ADDED_BY_OTHER_COUNTER = 1,
    }
}
