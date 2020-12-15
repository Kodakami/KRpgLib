using System;
using System.Collections.Generic;
using KRpgLib.Flags;
using KRpgLib.Stats;
using KRpgLib.Utility;

namespace KRpgLib.Investments
{
    public class InvestmentCollection<TValue> where TValue : struct
    {
        private readonly List<Investment<TValue>> _investments = new List<Investment<TValue>>();

        protected InvestmentValueCacheHelper InvestmentValueCache { get; }
        public InvestmentCollection()
        {
            InvestmentValueCache = new InvestmentValueCacheHelper(this);
        }
        public InvestmentCollection(IEnumerable<Investment<TValue>> investments)
            :this()
        {
            foreach (var investment in investments ?? throw new ArgumentNullException(nameof(investments)))
            {
                Add(investment);
            }
        }

        public bool HasInvestment(IInvestmentTemplate<TValue> template)
        {
            return _investments.Exists(p => p.Template == template);
        }
        protected void Add(Investment<TValue> investment)
        {
            if (investment != null)
            {
                _investments.Add(investment);

                InvestmentValueCache.SetDirty_FromExternal();
            }
        }
        protected void Remove(Investment<TValue> investment)
        {
            if (investment != null)
            {
                _investments.Remove(investment);

                InvestmentValueCache.SetDirty_FromExternal();
            }
        }

        protected class InvestmentValueCacheHelper : ParentedCachedValueController<InvestmentValue<TValue>, InvestmentCollection<TValue>>
        {
            public InvestmentValueCacheHelper(InvestmentCollection<TValue> parent) : base(parent) { }
            protected override InvestmentValue<TValue> CalculateNewCache()
            {
                var list = new List<InvestmentValue<TValue>>();
                foreach (var passive in Parent._investments)
                {
                    // Struct is safe to add directly.
                    list.Add(passive.Value);
                }

                // No need for null collection item checks. InvestmentValue throws null args in ctor.
                var totalStatDeltas = new StatDeltaCollection<TValue>(list.ConvertAll(iv => iv.GetStatDeltaCollection()));
                var totalFlags = new FlagCollection(list.ConvertAll(iv => iv.GetFlagCollection()));

                return new InvestmentValue<TValue>(totalFlags, totalStatDeltas);
            }

            protected override InvestmentValue<TValue> CreateCacheCopy(InvestmentValue<TValue> cache)
            {
                // Safe to pass struct directly.
                return cache;
            }
        }
    }
    public class WriteableInvestmentCollection<TValue> : InvestmentCollection<TValue> where TValue : struct
    {
        public WriteableInvestmentCollection() { }
        public WriteableInvestmentCollection(IEnumerable<Investment<TValue>> investments) : base(investments) { }

        new public void Add(Investment<TValue> investment) => base.Add(investment);
        new public void Remove(Investment<TValue> investment) => base.Remove(investment);
    }
}
