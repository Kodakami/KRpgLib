namespace KRpgLib.Counters
{
    public struct DurationInfo
    {
        // Duration expressed in number of counter ticks (Number of seconds * Number of ticks per second).
        public int MaxDuration { get; }
        public OnExpiredDecision OnNaturalExpire { get; }

        public DurationInfo(int maxDuration, OnExpiredDecision onDurationExpired)
        {
            MaxDuration = maxDuration;
            OnNaturalExpire = onDurationExpired;
        }
    }
    public enum OnExpiredDecision
    {
        REMOVE_ALL_INSTANCES_IN_STACK = 0,
        REMOVE_ONE_INSTANCE = 1,
    }
}
