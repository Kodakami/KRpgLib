using System;
using System.Collections.Generic;
using System.Text;

namespace KRpgLib.Utility
{
    public abstract class CachedValueController<TCache>
    {
        private TCache _cache;
        protected bool IsDirty { get; private set; } = true;
        protected void SetDirty() => IsDirty = true;
        protected void UpdateCache()
        {
            _cache = CalculateNewCache();
            IsDirty = false;
        }
        protected abstract TCache CalculateNewCache();
        protected abstract TCache CreateCacheCopy(TCache cache);

        public TCache GetCacheCopy()
        {
            if (IsDirty)
            {
                UpdateCache();
            }
            return CreateCacheCopy(_cache);
        }
        public void ForceCacheUpdate() => UpdateCache();
    }
}
