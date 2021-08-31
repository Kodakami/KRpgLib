using System;
using System.Collections.Generic;
using System.Linq;

namespace KRpgLib.Utility
{
    /// <summary>
    /// A utility class that maintains a collection of items with priority values. A higher priority value will make the item appear later in the list. The order of items with a shared priority value is undefined.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PriorityRegistry<T>
    {
        private readonly Dictionary<T, int> _registry;
        protected readonly OrderedListCacheHelper _cache;

        public PriorityRegistry()
        {
            _registry = new Dictionary<T, int>();
            _cache = new OrderedListCacheHelper(_registry);
        }
        public void RegisterItem(T item, int priority)
        {
            if (_registry.TryGetValue(item, out int existingPriority))
            {
                throw new ArgumentException($"Item is already in the priority registry (priority = {existingPriority}).");
            }

            // Null is an accepted value.
            _registry.Add(item, priority);

            SetDirty();
        }
        public void UnregisterItem(T item)
        {
            if (_registry.Remove(item))
            {
                SetDirty();
            }
        }
        protected void SetDirty() => _cache.SetDirty_FromExternal();
        public IReadOnlyList<T> GetAllByPriority() => _cache.GetCacheCopy();

        protected sealed class OrderedListCacheHelper : CachedValueController<List<T>, Dictionary<T, int>>
        {
            public OrderedListCacheHelper(Dictionary<T, int> context) : base(context) { }
            protected override List<T> CalculateNewCache()
            {
                return Context.OrderBy(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();
            }
            protected override List<T> CreateCacheCopy(List<T> cache)
            {
                return new List<T>(cache);
            }
        }
    }
}
