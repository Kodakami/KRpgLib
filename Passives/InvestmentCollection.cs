using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            // Pass in self to "CalculateTotalValue". I can't rely on my future self and others to realize that the derrived class is a collection of investments.
            InvestmentValueCache = new InvestmentValueCacheHelper(() => CalculateTotalValue(this), CreateCacheCopy);
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
        protected abstract TInvestmentValue CalculateTotalValue(IEnumerable<TInvestment> investments);
        protected abstract TInvestmentValue CreateCacheCopy(TInvestmentValue cache);

        public IEnumerator<TInvestment> GetEnumerator()
        {
            return ((IEnumerable<TInvestment>)_investments).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_investments).GetEnumerator();
        }

        protected sealed class InvestmentValueCacheHelper : CachedValueController<TInvestmentValue>
        {
            private readonly Func<TInvestmentValue> _calculateNewCacheFunc;
            private readonly Func<TInvestmentValue, TInvestmentValue> _createCachecopyFunc;

            public InvestmentValueCacheHelper(Func<TInvestmentValue> createNewCacheFunc, Func<TInvestmentValue, TInvestmentValue> copyCacheFunc)
            {
                // No null checks. Methods must exist.
                _calculateNewCacheFunc = createNewCacheFunc;
                _createCachecopyFunc = copyCacheFunc;
            }

            public void SetDirty_FromExternal() => SetDirty();

            protected override TInvestmentValue CalculateNewCache() => _calculateNewCacheFunc();
            protected override TInvestmentValue CreateCacheCopy(TInvestmentValue cache) => _createCachecopyFunc(cache);
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
