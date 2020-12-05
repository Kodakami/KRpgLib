using System;
using System.Collections.Generic;
using System.Text;

namespace KRpgLib.Counters
{
    public class Duration : CounterComponent
    {
        // Duration expressed in number of counter ticks (Number of seconds * Number of ticks per second).
        public int MaxDuration { get; }
        public OnExpiredDecision OnNaturalExpire { get; }

        public Duration(int maxDuration, OnExpiredDecision onDurationExpired)
        {
            MaxDuration = maxDuration;
            OnNaturalExpire = onDurationExpired;
        }

        public virtual void NaturalExpireStep(Counter counter)
        {
            if (counter.TicksCounted > MaxDuration)
            {
                // If all instances get removed on natural expiration,
                if (OnNaturalExpire == OnExpiredDecision.REMOVE_ALL_INSTANCES_IN_STACK)
                {
                    counter.RemoveAllInstances();
                }
                else
                {
                    counter.RemoveInstances(1);
                    counter.ResetTicksCounted();
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
