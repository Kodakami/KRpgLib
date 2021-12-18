using System;
using System.Collections.Generic;
using KRpgLib.Utility;

namespace KRpgLib.Stats
{
    /// <summary>
    /// A collection of stat providers (static and dynamic) that also maintains a cached delta collection.
    /// </summary>
    public sealed class StatProviderCollection : IStatProvider
    {
        private readonly List<IStatProvider> _providers;

        private readonly DeltaCollectionCacheHelper _deltaCollectionCache;

        public StatProviderCollection()
        {
            _providers = new List<IStatProvider>();

            _deltaCollectionCache = new DeltaCollectionCacheHelper(_providers);
        }
        public bool AddProvider(IStatProvider provider)
        {
            if (provider != null && !_providers.Contains(provider))
            {
                _providers.Add(provider);
                _deltaCollectionCache.SetDirty_FromExternal();
                return true;
            }
            return false;
        }
        public bool AddProvider(IDynamicStatProvider provider)
        {
            if (AddProvider((IStatProvider)provider))
            {
                provider.OnStatsChanged += _deltaCollectionCache.SetDirty_FromExternal;
                return true;
            }
            return false;
        }
        public bool RemoveProvider(IStatProvider provider)
        {
            if (provider != null && _providers.Contains(provider))
            {
                _providers.Remove(provider);
                _deltaCollectionCache.SetDirty_FromExternal();
                return true;
            }
            return false;
        }
        public bool RemoveProvider(IDynamicStatProvider provider)
        {
            if (RemoveProvider((IStatProvider)provider))
            {
                provider.OnStatsChanged -= _deltaCollectionCache.SetDirty_FromExternal;
                return true;
            }
            return false;
        }
        public bool HasProvider(IStatProvider statProvider) => _providers.Contains(statProvider);
        public DeltaCollection GetDeltaCollection() => _deltaCollectionCache.GetCacheCopy();

        private class DeltaCollectionCacheHelper : CachedValueController<DeltaCollection, List<IStatProvider>>
        {
            public DeltaCollectionCacheHelper(List<IStatProvider> context) : base(context) { }

            protected override DeltaCollection CalculateNewCache()
            {
                return new DeltaCollection(Context.ConvertAll(dsp => dsp.GetDeltaCollection()));
            }
            protected override DeltaCollection CreateCacheCopy(DeltaCollection cache)
            {
                // Safe to pass read-only collection by reference.
                return cache;
            }
        }
    }
}
