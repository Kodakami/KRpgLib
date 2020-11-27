using System.Collections.Generic;

namespace KRpgLib.Stats
{
    // Manages the value of a single stat in a stat manager. Tracks stat providers for itself but does not subscribe to stat provider events.
    internal class StatController<TValue> where TValue : struct
    {
        public IStatTemplate<TValue> StatTemplate { get; }

        /// <summary>
        /// True if the stat deltas provided have been changed and internal stat value needs to be recalculated. Starts as true to ensure cache update on next value request.
        /// </summary>
        public bool IsDirty { get; private set; } = true;
        public void SetDirty() => IsDirty = true;

        private TValue _cachedValueRaw;

        private readonly List<IStatProvider<TValue>> _providers = new List<IStatProvider<TValue>>();

        public StatController(IStatTemplate<TValue> statTemplate)
        {
            StatTemplate = statTemplate;

            // Cache will update before returning first value.
        }
        public void AddStatProvider(IStatProvider<TValue> provider)
        {
            _providers.Add(provider);

            IsDirty = true;
        }
        public void RemoveStatProvider(IStatProvider<TValue> provider)
        {
            _providers.Remove(provider);

            IsDirty = true;
        }
        /// <summary>
        /// Returns true if there are any stat providers in the internal list. Does not return true if deltas amount to default value (it is intended that the controller is still maintained in that case). If this returns false, the controller can be discarded as it represents a stat with nothing to manage.
        /// </summary>
        public bool HasDeltas() => _providers.Count > 0;
        private List<StatDelta<TValue>> GetAllDeltas()
        {
            List<StatDelta<TValue>> allDeltas = new List<StatDelta<TValue>>();
            foreach (var provider in _providers)
            {
                allDeltas.AddRange(provider.GetDeltasForStat(StatTemplate));
            }
            return allDeltas;
        }
        public TValue GetValueRaw()
        {
            if (_providers.Count == 0)
            {
                return StatTemplate.DefaultValue;
            }

            if (IsDirty)
            {
                UpdateCache();
            }

            return _cachedValueRaw;
        }

        /// <summary>
        /// Call this from client code only when needed. Is called internally when the value is requested while dirty.
        /// </summary>
        public void UpdateCache()
        {
            // Shortcut for empty provider list.
            if (_providers.Count == 0)
            {
                _cachedValueRaw = StatTemplate.DefaultValue;
            }

            // Get all deltas for stat template.
            var allDeltas = GetAllDeltas();

            // Calculate new stat value from deltas and update cached value.
            _cachedValueRaw = StatUtilities.ApplyStatDeltasByType(StatTemplate, allDeltas);

            // Reset dirty flag.
            IsDirty = false;
        }
    }
}
