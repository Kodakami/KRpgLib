using System;
using System.Collections.Generic;
using KRpgLib.Utility;

namespace KRpgLib.Flags
{
    public class FlagManager : IDynamicFlagProvider
    {
        protected List<FlagProviderController> _controllers = new List<FlagProviderController>();

        public event Action OnFlagsChanged;

        protected FlagTotalCacheHelper FlagTotalCache { get; }

        public FlagManager()
        {
            FlagTotalCache = new FlagTotalCacheHelper(this);
        }
        public FlagManager(List<IFlagProvider> initialProviders)
            :this()
        {
            foreach (var ip in initialProviders)
            {
                AddFlagProvider(ip);
            }
        }

        public IReadOnlyFlagCollection GetFlagCollection()
        {
            return FlagTotalCache.GetCacheCopy();
        }
        public void AddFlagProvider(IFlagProvider flagProvider)
        {
            AddFlagProvider_Internal(new FlagProviderController(flagProvider ?? throw new ArgumentNullException(nameof(flagProvider))));
        }
        public void AddFlagProvider(IDynamicFlagProvider dynamicFlagProvider)
        {
            AddFlagProvider_Internal(new FlagProviderController_Dynamic(dynamicFlagProvider ?? throw new ArgumentNullException(nameof(dynamicFlagProvider))));
            dynamicFlagProvider.OnFlagsChanged += SetDirty;
        }
        private void AddFlagProvider_Internal(FlagProviderController controller)
        {
            _controllers.Add(controller);
            SetDirty();
        }
        public void RemoveFlagProvider(IFlagProvider flagProvider)
        {
            if (flagProvider == null)
            {
                throw new ArgumentNullException(nameof(flagProvider));
            }

            var found = _controllers.Find(c => c.Provider == flagProvider);
            if (found != null)
            {
                RemoveFlagProvider_Internal(found);
            }
        }
        public void RemoveFlagProvider(IDynamicFlagProvider dynamicFlagProvider)
        {
            if (dynamicFlagProvider == null)
            {
                throw new ArgumentNullException(nameof(dynamicFlagProvider));
            }

            var found = _controllers.Find(c => c.Provider == dynamicFlagProvider);
            if (found != null)
            {
                found.Dispose();
                dynamicFlagProvider.OnFlagsChanged -= SetDirty;

                RemoveFlagProvider_Internal(found);
            }
        }
        private void RemoveFlagProvider_Internal(FlagProviderController controller)
        {
            _controllers.Remove(controller);
            SetDirty();
        }

        protected void SetDirty()
        {
            FlagTotalCache.SetDirty_FromExternal();
            OnFlagsChanged?.Invoke();
        }

        // Internal classes.
        protected sealed class FlagTotalCacheHelper : CachedValueController<FlagCollection, FlagManager>
        {
            public FlagTotalCacheHelper(FlagManager parent) : base(parent) { }
            protected override FlagCollection CalculateNewCache()
            {
                return new FlagCollection(Context._controllers.ConvertAll(c => c.GetFlags()));
            }

            protected override FlagCollection CreateCacheCopy(FlagCollection cache)
            {
                return new FlagCollection(cache);
            }
        }

        protected class FlagProviderController : System.IDisposable
        {
            public IFlagProvider Provider { get; }

            public FlagProviderController(IFlagProvider flagProvider)
            {
                Provider = flagProvider;
            }
            public virtual IReadOnlyFlagCollection GetFlags()
            {
                return Provider.GetFlagCollection();
            }
            public virtual void Dispose() { }
        }

        protected class FlagProviderController_Dynamic : FlagProviderController
        {
            new protected IDynamicFlagProvider Provider => (IDynamicFlagProvider)base.Provider;
            protected CacheHelper Cache { get; }

            public FlagProviderController_Dynamic(IDynamicFlagProvider flagProvider)
                :base(flagProvider)
            {
                Cache = new CacheHelper(this);

                flagProvider.OnFlagsChanged += Cache.SetDirty_FromExternal;
            }

            public override IReadOnlyFlagCollection GetFlags()
            {
                return Cache.GetCacheCopy();
            }
            public override void Dispose()
            {
                Provider.OnFlagsChanged -= Cache.SetDirty_FromExternal;
            }

            protected sealed class CacheHelper : CachedValueController<IReadOnlyFlagCollection, FlagProviderController_Dynamic>
            {
                public CacheHelper(FlagProviderController_Dynamic parent) : base(parent) { }
                protected override IReadOnlyFlagCollection CalculateNewCache()
                {
                    return Context.Provider.GetFlagCollection();
                }

                protected override IReadOnlyFlagCollection CreateCacheCopy(IReadOnlyFlagCollection cache)
                {
                    return new FlagCollection(cache);
                }
            }
        }
    }
}
