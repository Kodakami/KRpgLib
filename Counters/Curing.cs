using System;
using System.Collections.Generic;
using System.Text;

namespace KRpgLib.Counters
{
    public class Curing : CounterComponent
    {
        private readonly CureDomain _cureDomain;
        /// <summary>
        /// Number of ticks before this counter can be cured (removed by user effects).
        /// </summary>
        private readonly int _durationBeforeCurable;

        public Curing(CureDomain cureDomain, int durationBeforeCurable)
        {
            _cureDomain = cureDomain;
            _durationBeforeCurable = durationBeforeCurable;
        }

        /// <summary>
        /// The type of counter for the purposes of cure-type effects.
        /// </summary>
        public virtual CureDomain CureDomain => _cureDomain;
        /// <summary>
        /// Return true if cure will trigger, false if not.
        /// </summary>
        public virtual bool TryCure(CounterStack stack)
        {
            return _durationBeforeCurable < stack.TicksCounted;
        }
    }
    public enum CureDomain
    {
        /// <summary>
        /// The counter can not be considered beneficial, detrimental, or mixed. It is amoral.
        /// </summary>
        NONE = 0,
        /// <summary>
        /// The counter is primarily a buff, boost, or boon. It was most likely granted by a friendly source.
        /// </summary>
        BENEFICIAL = 1,
        /// <summary>
        /// The counter is primarily a debuff, hindrance, or punishment. It was most likely inflicted by an enemy source.
        /// </summary>
        DETRIMENTAL = 2,
        /// <summary>
        /// The counter is a somewhat a benefit and a detriment. It could be considered good, bad, or both, depending on context.
        /// </summary>
        MIXED = 3,
    }
}
