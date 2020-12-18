using System;
using System.Collections;
using System.Collections.Generic;
using KRpgLib.Flags;
using KRpgLib.Stats;
using KRpgLib.Utility;

namespace KRpgLib.Investments
{
    public abstract class InvestmentCollection<TInvestmentTemplate, TInvestment, TInvestmentValue> : IEnumerable<TInvestment>
        where TInvestmentTemplate : IInvestmentTemplate<TInvestmentValue>
        where TInvestment : AbstractInvestment<TInvestmentTemplate, TInvestmentValue>
        where TInvestmentValue : struct, IInvestmentValue
    {
        private readonly List<TInvestment> _investments = new List<TInvestment>();

        protected InvestmentValueCacheHelper InvestmentValueCache { get; }
        protected InvestmentCollection()
        {
            InvestmentValueCache = CreateCacheHelper() ?? throw new InvalidOperationException("Overrides of InvestmentCollection.CreateCacheHelper() may not return null.");
        }
        protected InvestmentCollection(IEnumerable<TInvestment> investments)
            :this()
        {
            foreach (var investment in investments ?? throw new ArgumentNullException(nameof(investments)))
            {
                AddIfNotInSet(investment);
            }
        }
        protected InvestmentCollection(IEnumerable<InvestmentCollection<TInvestmentTemplate, TInvestment, TInvestmentValue>> investmentCollections)
        {
            foreach (var collection in investmentCollections)
            {
                foreach (var investment in collection)
                {
                    AddIfNotInSet(investment);
                }
            }
        }

        public bool HasInvestment(TInvestmentTemplate template)
        {
            return _investments.Exists(p => p.Template.Equals(template));
        }
        public bool HasInvestmentWhere(Predicate<TInvestment> predicate)
        {
            return _investments.Exists(predicate);
        }
        public TInvestmentValue GetCombinedInvestmentValue() => InvestmentValueCache.GetCacheCopy();

        protected void AddIfNotInSet(TInvestment investment)
        {
            if (investment != null)
            {
                _investments.Add(investment);

                InvestmentValueCache.SetDirty_FromExternal();
            }
        }
        protected void RemoveIfNotInSet(TInvestment investment)
        {
            if (investment != null && _investments.Remove(investment))
            {
                InvestmentValueCache.SetDirty_FromExternal();
            }
        }
        protected abstract InvestmentValueCacheHelper CreateCacheHelper();

        public IEnumerator<TInvestment> GetEnumerator()
        {
            return ((IEnumerable<TInvestment>)_investments).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_investments).GetEnumerator();
        }

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
        public void Add(TInvestment investment) => AddIfNotInSet(investment);
        public void Remove(TInvestment investment) => RemoveIfNotInSet(investment);
    }
}
