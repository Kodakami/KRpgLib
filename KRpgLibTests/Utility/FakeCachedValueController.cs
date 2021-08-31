using System;
using System.Collections.Generic;
using KRpgLib.Utility;

namespace KRpgLibTests.Utility
{
    public class FakeCachedValueController : CachedValueController<int>
    {
        private readonly List<int> _ints = new List<int>();

        public event Action OnCacheUpdated;

        public void Add(int x)
        {
            _ints.Add(x);

            SetDirty();
        }
        protected override void OnEndOfUpdate()
        {
            OnCacheUpdated?.Invoke();
        }
        protected override int CalculateNewCache()
        {
            int total = 0;
            foreach (var x in _ints)
            {
                total += x;
            }
            return total;
        }

        protected override int CreateCacheCopy(int cache)
        {
            return cache;
        }
    }
}
