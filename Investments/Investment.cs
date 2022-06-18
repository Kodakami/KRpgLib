using System;
using System.Collections.Generic;

namespace KRpgLib.Investments
{
    // Shared code between the two most common types of investment: static and tiered.
    public abstract class AbstractInvestment<TValue>
    {
        // The value of the investment (such as an integer for a stat value).
        public abstract TValue Value { get; }
    }

    // An investment with an unchanging value, such as a flag that is provided to an actor.
    public abstract class StaticInvestment<TValue> : AbstractInvestment<TValue>
    {
        // The static value this investment provides.
        protected TValue _staticValue;

        public override TValue Value => _staticValue;

        protected StaticInvestment(TValue value)
        {
            _staticValue = value;
        }
    }
    
    // An investment which offers different values at different tiers. For example: putting different amounts of points into your Q ability gives different effects.
    public abstract class TieredInvestment<TValue> : AbstractInvestment<TValue>
    {
        // The list of values, in order, for each tier.
        private readonly List<TValue> _valueList;
        
        // The currently-selected tier.
        private int _tier;
        
        public int Tier
        {
            get
            {
                return _tier;
            }
            protected set
            {
                // Clamp between tier 0 and max tier possible.
                _tier = Math.Max(0, Math.Min(_valueList.Count - 1, value));
            }
        }

        // Return the value at the current tier.
        public override TValue Value => _valueList[_tier];

        protected TieredInvestment(IEnumerable<TValue> allValues)
        {
            _valueList = new List<TValue>(allValues ?? throw new ArgumentNullException(nameof(allValues)));
        }
    }
}
