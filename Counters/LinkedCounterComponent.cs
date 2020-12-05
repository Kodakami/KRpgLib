using System;
using System.Collections.Generic;
using System.Text;

namespace KRpgLib.Counters
{
    public class LinkedCounterComponent : CounterComponent
    {
        public List<LinkedCounterOnAddition> LinkedCountersOnAddition { get; }
        public List<LinkedCounterOnRemoval> LinkedCountersOnRemoval { get; }

        public LinkedCounterComponent(List<LinkedCounterOnAddition> onAddition, List<LinkedCounterOnRemoval> onRemoval)
        {
            LinkedCountersOnAddition = new List<LinkedCounterOnAddition>(onAddition);
            LinkedCountersOnRemoval = new List<LinkedCounterOnRemoval>(onRemoval);
        }
        /// <summary>
        /// This parent counter is being added to an object. Return a list of counters that are being added alongside it, or null.
        /// </summary>
        public virtual List<KeyValuePair<CounterTemplate, int>> GetCountersAddedDuringAddition(Counter counter, Random rng, CounterAdditionReason additionReason)
        {
            return GetCountersOnAddition_Internal(
                counter, rng,
                LinkedCounterAction.ADD_THIS_COUNTER,
                additionReason);
        }
        /// <summary>
        /// This parent counter is being added to an object. Return a list of counters that are being removed as a result of it, or null.
        /// </summary>
        public virtual List<KeyValuePair<CounterTemplate, int>> GetCountersRemovedDuringAddition(Counter counter, Random rng, CounterAdditionReason additionReason)
        {
            return GetCountersOnAddition_Internal(
                counter, rng,
                LinkedCounterAction.REMOVE_THIS_COUNTER,
                additionReason);
        }
        /// <summary>
        /// This parent counter is being removed from an object. Return a list of counters that are being added in its place, or null.
        /// </summary>
        public virtual List<KeyValuePair<CounterTemplate, int>> GetCountersAddedDuringRemoval(Counter counter, Random rng, CounterRemovalReason removalReason)
        {
            return GetCountersOnRemoval_Internal(
                counter, rng,
                LinkedCounterAction.ADD_THIS_COUNTER,
                removalReason);
        }
        /// <summary>
        /// This parent counter is being removed from an object. Return a list of counters that are being removed along with it, or null.
        /// </summary>
        public virtual List<KeyValuePair<CounterTemplate, int>> GetCountersRemovedDuringRemoval(Counter counter, Random rng, CounterRemovalReason removalReason)
        {
            return GetCountersOnRemoval_Internal(
                counter, rng,
                LinkedCounterAction.REMOVE_THIS_COUNTER,
                removalReason);
        }

        private List<KeyValuePair<CounterTemplate, int>> GetCountersOnAddition_Internal(Counter parent, Random rng, LinkedCounterAction linkedCounterAction, CounterAdditionReason counterAdditionReason)
        {
            var toBeRolled =
                    LinkedCountersOnAddition.FindAll(
                        lc => lc.LinkedCounterAction == linkedCounterAction
                        && GetAdditionPredicate(counterAdditionReason).Invoke(lc));

            // Roll to see if each one is applied and return the resulting list.
            return toBeRolled.FindAll(lc => TryRollForTrigger(rng, lc.ChanceToTrigger)).ConvertAll(lc => new KeyValuePair<CounterTemplate, int>(lc.Counter, lc.InstancesToAdd));
        }
        private List<KeyValuePair<CounterTemplate, int>> GetCountersOnRemoval_Internal(Counter parent, Random rng, LinkedCounterAction linkedCounterAction, CounterRemovalReason counterRemovalReason)
        {
            var toBeRolled =
                    LinkedCountersOnRemoval.FindAll(
                        lc => lc.LinkedCounterAction == linkedCounterAction
                        && GetRemovalPredicate(counterRemovalReason).Invoke(lc));

            // Roll to see if each one is applied and return the resulting list.
            return toBeRolled.FindAll(lc => TryRollForTrigger(rng, lc.ChanceToTrigger)).ConvertAll(lc => new KeyValuePair<CounterTemplate, int>(lc.Counter, lc.InstancesToAdd));
        }
        private bool TryRollForTrigger(Random rng, double chanceToTrigger)
        {
            return rng.NextDouble() < chanceToTrigger;
        }
        private Predicate<LinkedCounterOnAddition> GetAdditionPredicate(CounterAdditionReason additionReason)
        {
            switch (additionReason)
            {
                case CounterAdditionReason.DIRECTLY_APPLIED:
                    return lc => lc.TriggersWhenParentIsAppliedDirectly;
                case CounterAdditionReason.ADDED_BY_OTHER_COUNTER:
                    return lc => lc.TriggersWhenParentIsALinkedCounter;

                // Default includes CounterAdditionReason.SILENT_AND_ABSOLUTE.
                default:
                    return _ => false;
            }
        }
        private Predicate<LinkedCounterOnRemoval> GetRemovalPredicate(CounterRemovalReason removalReason)
        {
            switch (removalReason)
            {
                case CounterRemovalReason.EXPIRED_NATURALLY:
                    return lc => lc.TriggersWhenParentExpiresNaturally;
                case CounterRemovalReason.EXPIRED_EARLY:
                    return lc => lc.TriggersWhenParentExpiresEarly;
                case CounterRemovalReason.CURED_BY_USER_EFFECT:
                    return lc => lc.TriggersWhenParentIsRemovedByCuring;
                case CounterRemovalReason.CANCELLED_BY_USER:
                    return lc => lc.TriggersWhenParentIsCancelled;
                case CounterRemovalReason.OVERWRITTEN_BY_NEWER_COUNTER:
                    return lc => lc.TriggersWhenParentIsOverwrittenByNewerInstance;
                case CounterRemovalReason.REMOVED_BY_OTHER_COUNTER:
                    return lc => lc.TriggersWhenParentIsALinkedCounter;

                // Default includes CounterRemovalReason.SILENT_AND_ABSOLUTE.
                default:
                    return _ => false;
            }
        }
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
        CURED_BY_USER_EFFECT = 2,
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
        DIRECTLY_APPLIED = 0,
        /// <summary>
        /// The counter was added by another counter as the latter was being added or removed.
        /// </summary>
        ADDED_BY_OTHER_COUNTER = 1,
    }
}
