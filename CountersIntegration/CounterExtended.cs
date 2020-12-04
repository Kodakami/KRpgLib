using KRpgLib.Mods;
using KRpgLib.Stats;
using KRpgLib.Flags;
using System.Collections.Generic;

namespace KRpgLib.Counters
{
    public class CounterExtended<TValue> : Counter, IStatProvider<TValue>, IFlagProvider where TValue : struct
    {
        public CounterTemplateExtended<TValue> TemplateExtended => (CounterTemplateExtended<TValue>)Template;

        protected StatDeltaHelper StatDeltas { get; }
        protected FlagHelper Flags { get; }

        protected CounterExtended(
            CounterTemplateExtended<TValue> template,
            object origin,
            CuringHelper curing,
            DurationHelper duration,
            EarlyExpirationHelper earlyExpiration,
            LinkedCounterHelper linkedCounters,
            StatDeltaHelper statDeltas,
            FlagHelper flags)
            :base(
                 template, origin, curing, duration, earlyExpiration, linkedCounters)
        {
            StatDeltas = statDeltas;
            Flags = flags;
        }

        public StatDeltaCollection<TValue> GetStatDeltaCollection() => StatDeltas?.GetStatDeltas(this) ?? new StatDeltaCollection<TValue>();
        public List<Flag> GetAllFlags() => Flags?.GetFlags(this) ?? new List<Flag>();

        protected class StatDeltaHelper
        {
            public StatDeltaCollection<TValue> GetStatDeltas(CounterExtended<TValue> counter)
            {
                var info = counter.TemplateExtended.StatDeltas;
                if (info.HasValue)
                {
                    return info.Value.StatDeltaCollection;
                }
                return null;    // Handled in outer code.
            }
        }
        protected class FlagHelper
        {
            public List<Flag> GetFlags(CounterExtended<TValue> counter)
            {
                var info = counter.TemplateExtended.Flags;
                if (info.HasValue)
                {
                    return info.Value.Flags;
                }
                return null;    // Handled in outer code.
            }
        }

        public static CounterExtended<TValue> Create(CounterTemplateExtended<TValue> counterTemplate, object origin)
        {
            var counter = Counter.Create(counterTemplate, origin);
            // TODO: Make component system for counters.
            /*
             *  CounterComponent<TInfo> where TInfo : CounterComponentInfo
             *  
             *  CounterTemplate.GetInfo<T>() where T : CounterComponentInfo
             *  
             *  TComponent Counter.GetComponent<TComponent, TInfo>()
             *      where TComponent : CounterComponent<TInfo>
             *      where TInfo : CounterComponentInfo
             *  
             *  
             *  
             */
        }
    }
}
