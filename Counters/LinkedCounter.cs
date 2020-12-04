namespace KRpgLib.Counters
{
    public abstract class LinkedCounter
    {
        protected LinkedCounter(
            LinkedCounterAction linkedCounterAction, CounterTemplate counter, int instancesToAdd, double chanceToTrigger, bool triggersWhenParentIsALinkedCounter)
        {
            LinkedCounterAction = linkedCounterAction;

            Counter = counter;
            InstancesToAdd = instancesToAdd;
            ChanceToTrigger = chanceToTrigger;

            TriggersWhenParentIsALinkedCounter = triggersWhenParentIsALinkedCounter;
        }
        public LinkedCounterAction LinkedCounterAction { get; }

        public CounterTemplate Counter { get; }
        public int InstancesToAdd { get; }
        public double ChanceToTrigger { get; }

        public bool TriggersWhenParentIsALinkedCounter { get; }
    }
    public sealed class LinkedCounterOnAddition : LinkedCounter
    {
        public LinkedCounterOnAddition(
            LinkedCounterAction linkedCounterAction,
            CounterTemplate counter,
            int instancesToAdd,
            double chanceToTrigger,
            bool triggersWhenParentIsALinkedCounter,

            bool triggersWhenParentIsAppliedDirectly)
            :base(linkedCounterAction, counter, instancesToAdd, chanceToTrigger, triggersWhenParentIsALinkedCounter)
        {
            TriggersWhenParentIsAppliedDirectly = triggersWhenParentIsAppliedDirectly;
        }

        public bool TriggersWhenParentIsAppliedDirectly { get; }
    }
    public sealed class LinkedCounterOnRemoval : LinkedCounter
    {
        public LinkedCounterOnRemoval(
            LinkedCounterAction linkedCounterAction,
            CounterTemplate counter,
            int instancesToAdd,
            double chanceToTrigger,
            bool triggersWhenParentIsALinkedCounter,
            
            bool triggersWhenParentExpiresNaturally,
            bool triggersWhenParentExpiresEarly,
            bool triggersWhenParentIsRemovedByCuring,
            bool triggersWhenParentIsCancelled,
            bool triggersWhenParentIsOverwrittenByNewerInstance)
            :base(linkedCounterAction, counter, instancesToAdd, chanceToTrigger, triggersWhenParentIsALinkedCounter)
        {
            TriggersWhenParentExpiresNaturally = triggersWhenParentExpiresNaturally;
            TriggersWhenParentExpiresEarly = triggersWhenParentExpiresEarly;
            TriggersWhenParentIsRemovedByCuring = triggersWhenParentIsRemovedByCuring;
            TriggersWhenParentIsCancelled = triggersWhenParentIsCancelled;
            TriggersWhenParentIsOverwrittenByNewerInstance = triggersWhenParentIsOverwrittenByNewerInstance;
        }

        public bool TriggersWhenParentExpiresNaturally { get; }
        public bool TriggersWhenParentExpiresEarly { get; }
        public bool TriggersWhenParentIsRemovedByCuring { get; }
        public bool TriggersWhenParentIsCancelled { get; }
        public bool TriggersWhenParentIsOverwrittenByNewerInstance { get; }
    }
    public enum ParentAction
    {
        PARENT_IS_ADDED,
        PARENT_IS_REMOVED,
    }
    public enum LinkedCounterAction
    {
        ADD_THIS_COUNTER,
        REMOVE_THIS_COUNTER,
    }
}
