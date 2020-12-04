using System;
using System.Collections.Generic;

namespace KRpgLib.Counters
{
    public class Counter
    {
        public CounterTemplate Template { get; }
        public object Origin { get; }
        public int InstanceCount { get; private set; }
        public int UpdatesCounted { get; protected set; }

        /// <summary>
        /// May be null if system is not used. Check for null before referencing.
        /// </summary>
        protected CuringHelper Curing { get; }
        /// <summary>
        /// May be null if system is not used. Check for null before referencing.
        /// </summary>
        protected DurationHelper Duration { get; }
        /// <summary>
        /// May be null if system is not used. Check for null before referencing.
        /// </summary>
        protected EarlyExpirationHelper EarlyExpiration { get; }
        /// <summary>
        /// May be null if system is not used. Check for null before referencing.
        /// </summary>
        protected LinkedCounterHelper LinkedCounters { get; }

        protected Counter(
            CounterTemplate template,
            object origin,
            CuringHelper curing,
            DurationHelper duration,
            EarlyExpirationHelper earlyExpiration,
            LinkedCounterHelper linkedCounters)
        {
            Template = template ?? throw new ArgumentNullException(nameof(template));
            Origin = origin;
            InstanceCount = 0;
            UpdatesCounted = 0;

            // All optional components may be null. If they are, their accessor functions should return default results.
            Curing = curing;
            Duration = duration;
            EarlyExpiration = earlyExpiration;
            LinkedCounters = linkedCounters;
        }

        public void AddInstances(int count)
        {
            InstanceCount = Math.Max(InstanceCount + count, Template.StackingInfo.MaxStackSize);
        }
        public void RemoveInstances(int count)
        {
            InstanceCount = Math.Max(InstanceCount - count, 0);
        }
        public void RemoveAllInstances()
        {
            InstanceCount = 0;
        }
        public bool TryCure() => Curing?.TryCure(this) ?? false;
        public void NaturalExpireStep() => Duration?.NaturalExpireStep(this);
        public void EarlyExpireStep(Random rng) => EarlyExpiration?.EarlyExpireStep(this, rng);

        public List<KeyValuePair<CounterTemplate, int>> GetCountersAddedDuringAddition(Random rng, CounterAdditionReason additionReason)
        {
            return LinkedCounters?.GetCountersAddedDuringAddition(this, rng, additionReason) ?? new List<KeyValuePair<CounterTemplate, int>>();
        }
        public List<KeyValuePair<CounterTemplate, int>> GetCountersRemovedDuringAddition(Random rng, CounterAdditionReason additionReason)
        {
            return LinkedCounters?.GetCountersRemovedDuringAddition(this, rng, additionReason) ?? new List<KeyValuePair<CounterTemplate, int>>();
        }
        public List<KeyValuePair<CounterTemplate, int>> GetCountersAddedDuringRemoval(Random rng, CounterRemovalReason removalReason)
        {
            return LinkedCounters?.GetCountersAddedDuringRemoval(this, rng, removalReason) ?? new List<KeyValuePair<CounterTemplate, int>>();
        }
        public List<KeyValuePair<CounterTemplate, int>> GetCountersRemovedDuringRemoval(Random rng, CounterRemovalReason removalReason)
        {
            return LinkedCounters?.GetCountersRemovedDuringRemoval(this, rng, removalReason) ?? new List<KeyValuePair<CounterTemplate, int>>();
        }

        protected class CuringHelper
        {
            /// <summary>
            /// Return true if cured, false if not.
            /// </summary>
            public virtual bool TryCure(Counter counter)
            {
                if (counter.Template.CuringInfo.HasValue)
                {
                    var info = counter.Template.CuringInfo.Value;
                    if (info.DurationBeforeCurable < counter.UpdatesCounted)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        protected class DurationHelper
        {
            public virtual void NaturalExpireStep(Counter counter)
            {
                // If there is any duration info,
                if (counter.Template.DurationInfo.HasValue)
                {
                    var info = counter.Template.DurationInfo.Value;
                    // If the max duration has been exceeded as of this update,
                    if (counter.UpdatesCounted > info.MaxDuration)
                    {
                        // If all instances get removed on natural expiration,
                        if (info.OnNaturalExpire == OnExpiredDecision.REMOVE_ALL_INSTANCES_IN_STACK)
                        {
                            counter.RemoveAllInstances();
                        }
                        else
                        {
                            counter.RemoveInstances(1);
                            counter.UpdatesCounted = 0;
                        }
                    }
                }
            }
        }
        protected class EarlyExpirationHelper
        {
            /// <summary>
            /// Return true if stack is now empty, false if not.
            /// </summary>
            public virtual void EarlyExpireStep(Counter counter, Random rng)
            {
                if (counter.Template.EarlyExpirationInfo.HasValue)
                {
                    var info = counter.Template.EarlyExpirationInfo.Value;

                    // Adjustment for minimum duration and increased chance per roll.
                    double adjustedChance = 0;
                    if (counter.UpdatesCounted > info.DurationBeforeAttemptingEarlyExpiration)
                    {
                        adjustedChance = info.ChanceToExpireEarly;
                        adjustedChance += info.IncreasedChanceToExpireEarlyPerTick * (counter.UpdatesCounted - info.DurationBeforeAttemptingEarlyExpiration);

                        if (info.MaxChanceToExpireEarly.HasValue)
                        {
                            adjustedChance = Math.Max(adjustedChance, info.MaxChanceToExpireEarly.Value);
                        }
                    }

                    bool successfulExpire = rng.NextDouble() < adjustedChance;

                    if (successfulExpire)
                    {
                        if (info.OnEarlyExpire == OnExpiredDecision.REMOVE_ALL_INSTANCES_IN_STACK)
                        {
                            counter.RemoveAllInstances();
                        }
                        else
                        {
                            counter.RemoveInstances(1);
                        }
                    }
                }
            }
        }
        protected class LinkedCounterHelper
        {
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
                if (parent.Template.LinkedCounterInfo.HasValue)
                {
                    var info = parent.Template.LinkedCounterInfo.Value;

                    var toBeRolled =
                        info.LinkedCountersOnAddition.FindAll(
                            lc => lc.LinkedCounterAction == linkedCounterAction
                            && GetAdditionPredicate(counterAdditionReason).Invoke(lc));

                    // Roll to see if each one is applied and return the resulting list.
                    return toBeRolled.FindAll(lc => TryRollForTrigger(rng, lc.ChanceToTrigger)).ConvertAll(lc => new KeyValuePair<CounterTemplate, int>(lc.Counter, lc.InstancesToAdd));
                }
                return new List<KeyValuePair<CounterTemplate, int>>();
            }
            private List<KeyValuePair<CounterTemplate, int>> GetCountersOnRemoval_Internal(Counter parent, Random rng, LinkedCounterAction linkedCounterAction, CounterRemovalReason counterRemovalReason)
            {
                if (parent.Template.LinkedCounterInfo.HasValue)
                {
                    var info = parent.Template.LinkedCounterInfo.Value;

                    var toBeRolled =
                        info.LinkedCountersOnRemoval.FindAll(
                            lc => lc.LinkedCounterAction == linkedCounterAction
                            && GetRemovalPredicate(counterRemovalReason).Invoke(lc));

                    // Roll to see if each one is applied and return the resulting list.
                    return toBeRolled.FindAll(lc => TryRollForTrigger(rng, lc.ChanceToTrigger)).ConvertAll(lc => new KeyValuePair<CounterTemplate, int>(lc.Counter, lc.InstancesToAdd));
                }
                return new List<KeyValuePair<CounterTemplate, int>>();
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
        /// Create a counter from a counter template and origin object (origin may be null, template may not).
        /// </summary>
        /// <param name="template">the template this counter is an instance of</param>
        /// <param name="origin">the object which created, provided, inflicted, or is otherwise responsible for this counter</param>
        public static Counter Create(CounterTemplate template, object origin)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            return new Counter(
                template,
                origin,
                template.CuringInfo.HasValue ? new CuringHelper() : null,
                template.DurationInfo.HasValue ? new DurationHelper() : null,
                template.EarlyExpirationInfo.HasValue ? new EarlyExpirationHelper() : null,
                template.LinkedCounterInfo.HasValue ? new LinkedCounterHelper() : null);
        }
    }
}
