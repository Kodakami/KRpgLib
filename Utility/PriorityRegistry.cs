using System;
using System.Collections.Generic;

namespace KRpgLib.Utility
{
    public class PriorityRegistry<T>
    {
        private readonly Dictionary<T, int> _registry = new Dictionary<T, int>();
        private List<T> _orderedCache;
        private bool _isDirty;

        public PriorityRegistry()
        {
            _isDirty = true;
        }
        public void RegisterItem(T item, int priority)
        {
            if (_registry.TryGetValue(item, out int existingPriority))
            {
                throw new ArgumentException($"Item is already in the priority registry (priority = {existingPriority}).");
            }

            _registry.Add(item, priority);
            _isDirty = true;
        }
        public void UnregisterItem(T item)
        {
            _registry.Remove(item);

            // This is faster than setting the dirty flag.
            _orderedCache.Remove(item);
        }
        public List<T> GetAllByPriority()
        {
            if (_isDirty)
            {
                UpdateCache();
            }

            return new List<T>(_orderedCache);
        }
        private void UpdateCache()
        {
            var newList = new List<T>();

            var temp = new List<KeyValuePair<T, int>>(_registry);
            temp.Sort((kvp1, kvp2) => kvp1.Value - kvp2.Value);

            foreach (var item in temp)
            {
                newList.Add(item.Key);
            }
            
            _orderedCache = newList;
            
            _isDirty = false;
        }
    }
}
