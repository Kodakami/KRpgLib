using System;
using System.Collections.Generic;

namespace KRpgLib.Investments
{
    public abstract class AbstractInvestment<TInvestmentTemplate, TInvestmentValue>
        where TInvestmentTemplate : IInvestmentTemplate<TInvestmentValue>
        where TInvestmentValue : IInvestmentValue
    {
        public TInvestmentTemplate Template { get; }
        public abstract TInvestmentValue Value { get; }

        protected AbstractInvestment(TInvestmentTemplate template)
        {
            Template = template;
        }
    }
    public abstract class StaticInvestment<TInvestmentTemplate, TInvestmentValue> : AbstractInvestment<TInvestmentTemplate, TInvestmentValue>
        where TInvestmentTemplate : IInvestmentTemplate<TInvestmentValue>
        where TInvestmentValue : IInvestmentValue
    {
        protected TInvestmentValue _staticValue;

        public override TInvestmentValue Value => _staticValue;

        protected StaticInvestment(TInvestmentTemplate template, TInvestmentValue value)
            :base(template)
        {
            _staticValue = value;
        }
    }
    public abstract class TieredInvestment<TInvestmentTemplate, TInvestmentValue> : AbstractInvestment<TInvestmentTemplate, TInvestmentValue>
        where TInvestmentTemplate : IInvestmentTemplate<TInvestmentValue>
        where TInvestmentValue : IInvestmentValue
    {
        private readonly List<TInvestmentValue> _valueList;
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
        public override TInvestmentValue Value => _valueList[_tier];

        protected TieredInvestment(TInvestmentTemplate template, IEnumerable<TInvestmentValue> allValues)
            :base(template)
        {
            _valueList = new List<TInvestmentValue>(allValues ?? throw new ArgumentNullException(nameof(allValues)));
        }
    }
}
