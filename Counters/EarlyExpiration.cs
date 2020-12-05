using System;
using System.Collections.Generic;
using System.Text;

namespace KRpgLib.Counters
{
    public class EarlyExpiration : CounterComponent
    {
        public int DurationBeforeAttemptingEarlyExpiration { get; }
        public double ChanceToExpireEarly { get; }
        public double IncreasedChanceToExpireEarlyPerTick { get; }
        public double? MaxChanceToExpireEarly { get; }
        public OnExpiredDecision OnEarlyExpire { get; }

        public EarlyExpiration(int durationBeforeAttemptingEarlyExpiration, double chanceToExpireEarly, double increasedChanceToExpireEarlyPerTick, double? maxChanceToExpireEarly, OnExpiredDecision onEarlyExpire)
        {
            DurationBeforeAttemptingEarlyExpiration = durationBeforeAttemptingEarlyExpiration;
            ChanceToExpireEarly = chanceToExpireEarly;
            IncreasedChanceToExpireEarlyPerTick = increasedChanceToExpireEarlyPerTick;
            MaxChanceToExpireEarly = maxChanceToExpireEarly;
            OnEarlyExpire = onEarlyExpire;
        }

        public virtual void EarlyExpireStep(Counter counter, Random rng)
        {
            // Adjustment for minimum duration and increased chance per roll.
            double adjustedChance = 0;
            if (counter.TicksCounted > DurationBeforeAttemptingEarlyExpiration)
            {
                adjustedChance = ChanceToExpireEarly;
                adjustedChance += IncreasedChanceToExpireEarlyPerTick * (counter.TicksCounted - DurationBeforeAttemptingEarlyExpiration);

                if (MaxChanceToExpireEarly.HasValue)
                {
                    adjustedChance = Math.Max(adjustedChance, MaxChanceToExpireEarly.Value);
                }
            }

            // Roll for it!
            bool successfulExpire = rng.NextDouble() < adjustedChance;

            if (successfulExpire)
            {
                if (OnEarlyExpire == OnExpiredDecision.REMOVE_ALL_INSTANCES_IN_STACK)
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
}
