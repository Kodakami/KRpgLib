using System;
using System.Collections.Generic;
using KRpgLib.Utility;

namespace KRpgLib.Stats
{
    public sealed class StatProviderCollection<TValue> : IStatProvider<TValue> where TValue : struct
    {
        private readonly List<IStatProvider<TValue>> _providers;
        private readonly DeltaCollectionCacheHelper _statDeltaCollectionCache;

        public StatProviderCollection()
        {
            _providers = new List<IStatProvider<TValue>>();
            _statDeltaCollectionCache = new DeltaCollectionCacheHelper(_providers);
        }
        public void AddProvider(IStatProvider<TValue> provider)
        {
            if (provider == null || _providers.Contains(provider))
            {
                return;
            }

            _providers.Add(provider);
            _statDeltaCollectionCache.SetDirty_FromExternal();
        }
        public void RemoveProvider(IStatProvider<TValue> provider)
        {
            if (provider == null || !_providers.Contains(provider))
            {
                return;
            }

            _providers.Remove(provider);
            _statDeltaCollectionCache.SetDirty_FromExternal();
        }
        public bool HasProvider(IStatProvider<TValue> statProvider) => _providers.Contains(statProvider);
        public StatDeltaCollection<TValue> GetStatDeltaCollection() => _statDeltaCollectionCache.GetCacheCopy();

        private class DeltaCollectionCacheHelper : ParentedCachedValueController<StatDeltaCollection<TValue>, List<IStatProvider<TValue>>>
        {
            public DeltaCollectionCacheHelper(List<IStatProvider<TValue>> parent) : base(parent) { }

            protected override StatDeltaCollection<TValue> CalculateNewCache()
            {
                return new StatDeltaCollection<TValue>(Parent.ConvertAll(p => p.GetStatDeltaCollection()));
            }
            protected override StatDeltaCollection<TValue> CreateCacheCopy(StatDeltaCollection<TValue> cache)
            {
                // Safe to pass read-only collection by reference.
                return cache;
            }
        }
    }
}
