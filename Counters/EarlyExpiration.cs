using System;
using System.Collections.Generic;
using System.Text;

namespace KRpgLib.Counters
{
    public class EarlyExpiration : CounterComponent
    {
        public int MinimumDurationBeforeAttemptingEarlyExpiration { get; }
        public double ChanceToExpireEarly { get; }
        public double IncreasedChanceToExpireEarlyPerTick { get; }
        public double? MaxChanceToExpireEarly { get; }
        public OnExpiredDecision OnEarlyExpire { get; }

        public EarlyExpiration(int durationBeforeAttemptingEarlyExpiration, double chanceToExpireEarly, double increasedChanceToExpireEarlyPerTick, double? maxChanceToExpireEarly, OnExpiredDecision onEarlyExpire)
        {
            MinimumDurationBeforeAttemptingEarlyExpiration = durationBeforeAttemptingEarlyExpiration;
            ChanceToExpireEarly = chanceToExpireEarly;
            IncreasedChanceToExpireEarlyPerTick = increasedChanceToExpireEarlyPerTick;
            MaxChanceToExpireEarly = maxChanceToExpireEarly;
            OnEarlyExpire = onEarlyExpire;
        }

        public virtual void EarlyExpireStep(CounterManager counterManager, CounterStack counter)
        {
            // Adjustment for minimum duration and increased chance per roll.
            double adjustedChance = 0;
            if (counter.TicksCounted > MinimumDurationBeforeAttemptingEarlyExpiration)
            {
                adjustedChance = ChanceToExpireEarly;
                adjustedChance += IncreasedChanceToExpireEarlyPerTick * (counter.TicksCounted - MinimumDurationBeforeAttemptingEarlyExpiration);

                if (MaxChanceToExpireEarly.HasValue)
                {
                    adjustedChance = Math.Max(adjustedChance, MaxChanceToExpireEarly.Value);
                }
            }

            // Roll for it!
            bool successfulExpire = Utility.Environment.Rng.NextDouble() < adjustedChance;

            if (successfulExpire)
            {
                if (OnEarlyExpire == OnExpiredDecision.REMOVE_ONE_INSTANCE)
                {
                    counter.RemoveInstances(counterManager, 1, CounterRemovalReason.EXPIRED_EARLY);
                    counter.ResetTicksCounted();
                }
                else
                {
                    counter.RemoveAllInstances(counterManager, CounterRemovalReason.EXPIRED_EARLY);
                }
            }
        }
    }
}
