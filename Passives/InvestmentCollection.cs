using System;
using System.Collections.Generic;
using KRpgLib.Flags;
using KRpgLib.Stats;
using KRpgLib.Utility;

namespace KRpgLib.Investments
{
    public abstract class InvestmentCollection<TInvestmentTemplate, TInvestment, TInvestmentValue>
        where TInvestmentTemplate : IInvestmentTemplate<TInvestmentValue>
        where TInvestment : AbstractInvestment<TInvestmentTemplate, TInvestmentValue>
        where TInvestmentValue : struct, IInvestmentValue
    {
        private readonly List<TInvestment> _investments = new List<TInvestment>();

        protected InvestmentValueCacheHelper InvestmentValueCache { get; }
        protected InvestmentCollection()
        {
            InvestmentValueCache = CreateCacheHelper();
        }
        protected InvestmentCollection(IEnumerable<TInvestment> investments)
            :this()
        {
            foreach (var investment in investments ?? throw new ArgumentNullException(nameof(investments)))
            {
                Add(investment);
            }
        }

        public bool HasInvestment(TInvestmentTemplate template)
        {
            return _investments.Exists(p => p.Template.Equals(template));
        }
        protected void Add(TInvestment investment)
        {
            if (investment != null)
            {
                _investments.Add(investment);

                InvestmentValueCache.SetDirty_FromExternal();
            }
        }
        protected void Remove(TInvestment investment)
        {
            if (investment != null)
            {
                _investments.Remove(investment);

                InvestmentValueCache.SetDirty_FromExternal();
            }
        }
        protected abstract InvestmentValueCacheHelper CreateCacheHelper();

        protected abstract class InvestmentValueCacheHelper : ParentedCachedValueController<
            TInvestmentValue,
            InvestmentCollection<TInvestmentTemplate, TInvestment, TInvestmentValue>>
        {
            protected InvestmentValueCacheHelper(InvestmentCollection<TInvestmentTemplate, TInvestment, TInvestmentValue> parent) : base(parent) { }
        }
    }
    public abstract class WriteableInvestmentCollection<TInvestmentTemplate, TInvestment, TInvestmentValue>
        : InvestmentCollection<TInvestmentTemplate, TInvestment, TInvestmentValue>
        where TInvestmentTemplate : IInvestmentTemplate<TInvestmentValue>
        where TInvestment : AbstractInvestment<TInvestmentTemplate, TInvestmentValue>
        where TInvestmentValue : struct, IInvestmentValue
    {
        new public void Add(TInvestment investment) => base.Add(investment);
        new public void Remove(TInvestment investment) => base.Remove(investment);
    }
}
