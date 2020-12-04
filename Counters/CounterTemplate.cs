namespace KRpgLib.Counters
{
    public class CounterTemplate
    {
        public CounterTemplate(bool isVisibleToUsers, CounterDomain domain, TrackByOriginDecision trackByOrigin, StackingInfo stackingInfo, DurationInfo? durationInfo, EarlyExpirationInfo? earlyExpirationInfo, CuringInfo? curingInfo, LinkedCounterInfo? linkedCounterInfo)
        {
            IsVisibleToUsers = isVisibleToUsers;
            Domain = domain;
            TrackByOrigin = trackByOrigin;
            StackingInfo = stackingInfo;
            DurationInfo = durationInfo;
            EarlyExpirationInfo = earlyExpirationInfo;
            CuringInfo = curingInfo;
            LinkedCounterInfo = linkedCounterInfo;
        }

        // Required components.
        public bool IsVisibleToUsers { get; }
        public CounterDomain Domain { get; }
        public TrackByOriginDecision TrackByOrigin { get; }

        public StackingInfo StackingInfo { get; }

        // Optional components.
        public DurationInfo? DurationInfo { get; }
        public EarlyExpirationInfo? EarlyExpirationInfo { get; }
        public CuringInfo? CuringInfo { get; }
        public LinkedCounterInfo? LinkedCounterInfo { get; }
    }
    public enum TrackByOriginDecision
    {
        ONE_STACK_REGARDLESS_OF_ORIGIN = 0,
        EACH_ORIGIN_GETS_UNIQUE_STACK = 1,
    }
}
