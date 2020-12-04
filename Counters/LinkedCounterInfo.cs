using System.Collections.Generic;

namespace KRpgLib.Counters
{
    public struct LinkedCounterInfo
    {
        public List<LinkedCounterOnAddition> LinkedCountersOnAddition { get; }
        public List<LinkedCounterOnRemoval> LinkedCountersOnRemoval { get; }

        public LinkedCounterInfo(List<LinkedCounterOnAddition> onAddition, List<LinkedCounterOnRemoval> onRemoval)
        {
            LinkedCountersOnAddition = new List<LinkedCounterOnAddition>(onAddition);
            LinkedCountersOnRemoval = new List<LinkedCounterOnRemoval>(onRemoval);
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
