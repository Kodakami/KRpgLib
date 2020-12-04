namespace KRpgLib.Counters
{
    public struct EarlyExpirationInfo
    {
        public int DurationBeforeAttemptingEarlyExpiration { get; }
        public double ChanceToExpireEarly { get; }
        public double IncreasedChanceToExpireEarlyPerTick { get; }
        public double? MaxChanceToExpireEarly { get; }
        public OnExpiredDecision OnEarlyExpire { get; }

        public EarlyExpirationInfo(int durationBeforeAttemptingEarlyExpiration, double chanceToExpireEarly, double increasedChanceToExpireEarlyPerTick, double? maxChanceToExpireEarly, OnExpiredDecision onEarlyExpire)
        {
            DurationBeforeAttemptingEarlyExpiration = durationBeforeAttemptingEarlyExpiration;
            ChanceToExpireEarly = chanceToExpireEarly;
            IncreasedChanceToExpireEarlyPerTick = increasedChanceToExpireEarlyPerTick;
            MaxChanceToExpireEarly = maxChanceToExpireEarly;
            OnEarlyExpire = onEarlyExpire;
        }
    }
}
