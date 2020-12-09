using System;
using System.Collections.Generic;
using System.Text;

namespace KRpgLib.Counters
{
    public class Duration : CounterComponent
    {
        /// <summary>
        /// Duration expressed in number of counter ticks (Number of seconds * Number of ticks per second).
        /// </summary>
        public int MaxDuration { get; }

        private readonly OnExpiredDecision _onExpiredNaturally;

        public Duration(int maxDuration, OnExpiredDecision onExpiredNaturally)
        {
            MaxDuration = maxDuration;
            _onExpiredNaturally = onExpiredNaturally;
        }

        public override void OnCounterTick(CounterManager counterManager, CounterStack counterStack, int numberOfTicks)
        {
            if (counterStack.TicksCounted > MaxDuration)
            {
                // If all instances get removed on natural expiration,
                if (_onExpiredNaturally == OnExpiredDecision.REMOVE_ONE_INSTANCE)
                {
                    counterStack.RemoveInstances(counterManager, 1, CounterRemovalReason.EXPIRED_NATURALLY);
                    counterStack.ResetTicksCounted();
                }
                else
                {
                    counterStack.RemoveAllInstances(counterManager, CounterRemovalReason.EXPIRED_NATURALLY);
                }
            }
        }
    }
    public enum OnExpiredDecision
    {
        REMOVE_ALL_INSTANCES_IN_STACK = 0,
        REMOVE_ONE_INSTANCE = 1,
    }
}
