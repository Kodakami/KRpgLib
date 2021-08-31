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
        public DeltaCollection<TValue> GetDeltaCollection() => _statDeltaCollectionCache.GetCacheCopy();

        private class DeltaCollectionCacheHelper : CachedValueController<DeltaCollection<TValue>, List<IStatProvider<TValue>>>
        {
            public DeltaCollectionCacheHelper(List<IStatProvider<TValue>> context) : base(context) { }

            protected override DeltaCollection<TValue> CalculateNewCache()
            {
                return new DeltaCollection<TValue>(Context.ConvertAll(p => p.GetDeltaCollection()));
            }
            protected override DeltaCollection<TValue> CreateCacheCopy(DeltaCollection<TValue> cache)
            {
                // Safe to pass read-only collection by reference.
                return cache;
            }
        }
    }
}
