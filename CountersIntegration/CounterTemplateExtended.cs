using KRpgLib.Mods;

namespace KRpgLib.Counters
{
    public class CounterTemplateExtended<TValue> : CounterTemplate where TValue : struct
    {
        public StatDeltaInfo<TValue>? StatDeltas { get; }
        public FlagInfo? Flags { get; }

        public CounterTemplateExtended(
            bool isVisibleToUsers,
            CounterDomain domain,
            TrackByOriginDecision trackByOrigin,
            StackingInfo stackingInfo,
            DurationInfo? durationInfo,
            EarlyExpirationInfo? earlyExpirationInfo,
            CuringInfo? curingInfo,
            LinkedCounterInfo? linkedCounterInfo,
            StatDeltaInfo<TValue>? statDeltaInfo,
            FlagInfo? flagInfo)
            :base(isVisibleToUsers, domain, trackByOrigin, stackingInfo, durationInfo, earlyExpirationInfo, curingInfo, linkedCounterInfo)
        {
            StatDeltas = statDeltaInfo;
            Flags = flagInfo;
        }
    }
}
