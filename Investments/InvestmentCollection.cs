using System;
using System.Collections;
using System.Collections.Generic;
using KRpgLib.Flags;
using KRpgLib.Stats;
using KRpgLib.Utility;

namespace KRpgLib.Investments
{
    // A collection of investments whose combined value can also be cached and retrieved.
    // Really useful for something like a Path of Exile passive tree, which provides hundreds of possible stat and flag changes.
    // In that case, caching the combined stat and flag changes greatly improves the time it takes to recalculate player stats.
    public abstract class InvestmentCollection<TInvestment, TValue>
        : IReadOnlyInvestmentCollection<TInvestment, TValue>
            where TInvestment : AbstractInvestment<TValue>
            // where TValue is anything
    {
        // The internal list of investments.
        private readonly List<TInvestment> _investments = new List<TInvestment>();

        // Helper object for caching combined investment value.
        protected InvestmentValueCacheHelper InvestmentValueCache { get; }

        // How many investments are in the collection.
        public int Count => _investments.Count;

        protected InvestmentCollection()
        {
            InvestmentValueCache = CreateCacheHelper() ?? throw new InvalidOperationException("Overrides of InvestmentCollection.CreateCacheHelper() may not return null.");
        }
        protected InvestmentCollection(IEnumerable<TInvestment> investments)
            :this()
        {
            foreach (var investment in investments ?? throw new ArgumentNullException(nameof(investments)))
            {
                Add(investment);
            }
        }
        protected InvestmentCollection(IEnumerable<InvestmentCollection<TInvestment, TValue>> investmentCollections)
        {
            foreach (var collection in investmentCollections)
            {
                foreach (var investment in collection)
                {
                    Add(investment);
                }
            }
        }

        // Is there an investment where predicate is true?
        public bool HasInvestmentWhere(Predicate<TInvestment> predicate)
        {
            return _investments.Exists(predicate);
        }

        // Get the cached combined value of all investments in the collection.
        public TValue GetCombinedInvestmentValue() => InvestmentValueCache.GetCacheCopy();

        // Add the investment to the set.
        public void Add(TInvestment investment)
        {
            if (investment != null)
            {
                _investments.Add(investment);

                InvestmentValueCache.SetDirty_FromExternal();
            }
        }

        // Remove the investment from the set.
        public void Remove(TInvestment investment)
        {
            if (investment != null && _investments.Remove(investment))
            {
                InvestmentValueCache.SetDirty_FromExternal();
            }
        }

        // Create a new cache helper of your particular implementation.
        protected abstract InvestmentValueCacheHelper CreateCacheHelper();

        public IEnumerator<TInvestment> GetEnumerator()
        {
            return ((IEnumerable<TInvestment>)_investments).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_investments).GetEnumerator();
        }

        protected abstract class InvestmentValueCacheHelper : CachedValueController<
            TValue,
            InvestmentCollection<TInvestment, TValue>>
        {
            protected InvestmentValueCacheHelper(InvestmentCollection<TInvestment, TValue> parent) : base(parent) { }
        }
    }
}
