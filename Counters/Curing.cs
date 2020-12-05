using System;
using System.Collections.Generic;
using System.Text;

namespace KRpgLib.Counters
{
    public class Curing : CounterComponent
    {
        /// <summary>
        /// Number of ticks before this counter can be cured (removed by user effects). Null indicates counter is not curable.
        /// </summary>
        public int? DurationBeforeCurable { get; }

        public Curing(int? durationBeforeCurable)
        {
            DurationBeforeCurable = durationBeforeCurable;
        }

        /// <summary>
        /// Return true if cured, false if not.
        /// </summary>
        public virtual bool TryCure(Counter counter)
        {
            return DurationBeforeCurable < counter.UpdateTicks;
        }
    }
}
