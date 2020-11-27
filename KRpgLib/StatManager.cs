using System.Collections.Generic;

namespace KRpgLib.Stats
{
    /// <summary>
    /// Base class for an object the manages the stats for something. This may be owned by an RPG character, an enemy, etc... Your implementation should take a series of components in the constructor and apply them as initial stat providers.
    /// </summary>
    public class StatManager<TValue> : AbstractStatSet<TValue> where TValue : struct
    {
        private readonly Dictionary<IStatTemplate<TValue>, StatController<TValue>> _controllerDict =
            new Dictionary<IStatTemplate<TValue>, StatController<TValue>>();

        private readonly List<IStatProvider<TValue>> _statProviders = new List<IStatProvider<TValue>>();

        private Dictionary<IStatTemplate<TValue>, TValue> _cachedSnapshotValues;     //Assigned and converted to StatSet before returning value.
        private bool _isCachedSetDirty = true;

        //ctor
        public StatManager()
        {
            // Nothing. :D
            // Pass in other components that are stat providers such as class/race managers, arpg-style modifier managers, status effect managers, and passive ability managers.
        }
        public StatManager(IEnumerable<IStatProvider<TValue>> initStatProviders)
        {
            // Initialize with stat providers.
            foreach (var provider in initStatProviders)
            {
                AddStatProvider(provider);
            }
        }

        /// <summary>
        /// Add a stat provider to the stat manager.
        /// </summary>
        /// <param name="statProvider">an IStatProvider</param>
        public void AddStatProvider(IStatProvider<TValue> statProvider)
        {
            // Null check.
            AddStatProvider_Internal(statProvider ?? throw new System.ArgumentNullException(nameof(statProvider)));
        }
        /// <summary>
        /// Add a list of stat providers to the stat manager.
        /// </summary>
        /// <param name="statProviders">an IStatProvider list</param>
        public void AddStatProviders(IEnumerable<IStatProvider<TValue>> statProviders)
        {
            // Null check.
            foreach (var provider in statProviders ?? throw new System.ArgumentNullException(nameof(statProviders)))
            {
                // Further null check after the jump.
                AddStatProvider(provider);
            }
        }
        public void AddDynamicStatProvider(IStatProvider_Dynamic<TValue> dynamicStatProvider)
        {
            AddStatProvider(dynamicStatProvider ?? throw new System.ArgumentNullException(nameof(dynamicStatProvider)));
            SubscribeToDynamicStatProvider(dynamicStatProvider);
        }
        public void AddDynamicStatProviders(IEnumerable<IStatProvider_Dynamic<TValue>> dynamicStatProviders)
        {
            // Null check.
            foreach (var dsp in dynamicStatProviders ?? throw new System.ArgumentNullException(nameof(dynamicStatProviders)))
            {
                // Further null check after the jump.
                AddDynamicStatProvider(dsp);
            }
        }

        // Internal provider addition method.
        private void AddStatProvider_Internal(IStatProvider<TValue> safeStatProvider)
        {
            // Checks done in public accessor methods.

            // Get dirty (get all stats that have deltas from provider).
            var dirtyList = safeStatProvider.GetStatsWithDeltas();

            // For each stat modified by provider...
            foreach (var dirtyStat in dirtyList)
            {
                // Get the controller (or make a new one).
                var controller = GetOrCreateController(dirtyStat);

                // And add this stat provider to its list (this will flag the controller as dirty).
                controller.AddStatProvider(safeStatProvider);
            }

            // All clear.
            _statProviders.Add(safeStatProvider);
        }
        private void SubscribeToDynamicStatProvider(IStatProvider_Dynamic<TValue> statProvider)
        {
            // Checks done in public accessor methods.

            // Subscribe to event.
            statProvider.OnStatDeltasChanged += SetDirty;
        }
        /// <summary>
        /// Remove a stat provider from the stat manager (will no longer be queried for stat deltas when updating stat cache).
        /// </summary>
        /// <param name="statProvider">an IStatProvider</param>
        public void RemoveStatProvider(IStatProvider<TValue> statProvider)
        {
            // Null check.
            if (_statProviders.Contains(statProvider ?? throw new System.ArgumentNullException(nameof(statProvider))))
            {
                // Removal process.
                RemoveStatProvider_Internal(statProvider);

                // Clean up controller dictionary.
                CleanUpUnusedControllers();
            }
            // If we got here, then the provider was removed by some other means. Strange.
        }
        /// <summary>
        /// Remove a list of stat providers from the stat manager (will no longer be queried for stat deltas when updating stat cache).
        /// </summary>
        /// <param name="statProviders">an IStatProvider list</param>
        public void RemoveStatProviders(List<IStatProvider<TValue>> statProviders)
        {
            // Null check.
            if (statProviders == null)
            {
                throw new System.ArgumentNullException(nameof(statProviders));
            }

            // Empty list check.
            if (statProviders.Count == 0)
            {
                return;
            }

            foreach (var provider in statProviders)
            {
                // Null check and not in list check are here (we don't call the other method) in order to call clean up less often.
                if (provider != null && _statProviders.Contains(provider))
                {
                    RemoveStatProvider_Internal(provider);
                }
            }

            // Clean up controller dictionary.
            CleanUpUnusedControllers();
        }
        public void RemoveDynamicStatProvider(IStatProvider_Dynamic<TValue> dynamicStatProvider)
        {
            RemoveStatProvider(dynamicStatProvider ?? throw new System.ArgumentNullException(nameof(dynamicStatProvider)));
            UnsubscribeFromDynamicStatProvider(dynamicStatProvider);
        }
        public void RemoveDynamicStatProviders(IEnumerable<IStatProvider_Dynamic<TValue>> dynamicStatProviders)
        {
            foreach (var dsp in dynamicStatProviders ?? throw new System.ArgumentNullException(nameof(dynamicStatProviders)))
            {
                RemoveDynamicStatProvider(dsp);
            }
        }

        // Internal provider removal method.
        private void RemoveStatProvider_Internal(IStatProvider<TValue> statProvider)
        {
            // Checks in public accessor methods.

            // Get dirty (get all stats that have deltas from provider).
            var dirtyList = statProvider.GetStatsWithDeltas();

            // For each stat modified by provider...
            foreach (var dirtyStat in dirtyList)
            {
                // Get the controller.
                var controller = GetOrCreateController(dirtyStat);

                // And remove this stat provider from its list (will set controller dirty).
                controller.RemoveStatProvider(statProvider);
            }

            // All clear.
        }
        private void UnsubscribeFromDynamicStatProvider(IStatProvider_Dynamic<TValue> statProvider)
        {
            // Checks done in public accessor methods.

            // Unsubscribe from event.
            statProvider.OnStatDeltasChanged -= SetDirty;
        }

        protected override TValue GetStatValue_Internal(IStatTemplate<TValue> safeStatTemplate)
        {
            // Null checks unnecessary at this point.

            if (!_controllerDict.ContainsKey(safeStatTemplate))
            {
                return safeStatTemplate.DefaultValue;
            }

            return _controllerDict[safeStatTemplate].GetValueRaw();
        }

        /// <summary>
        /// Gets a snapshot of all current stat values. Will utilize cached values where possible.
        /// </summary>
        /// <returns>new StatSnapshot</returns>
        public StatSnapshot<TValue> GetStatSnapshot()
        {
            if (_isCachedSetDirty)
            {
                UpdateCachedStatSet();
            }

            return new StatSnapshot<TValue>(_cachedSnapshotValues);
        }

        private StatController<TValue> GetOrCreateController(IStatTemplate<TValue> statTemplate)
        {
            // Null checks should be in calling code.

            if (_controllerDict.ContainsKey(statTemplate))
            {
                return _controllerDict[statTemplate];
            }

            var newController = new StatController<TValue>(statTemplate);
            _controllerDict.Add(statTemplate, newController);
            return newController;
        }

        // Mark a stat controller as in need of recalculation (or add the controller if a dynamic stat provider's update added a new stat). Stat controllers whose providers were all removed in this way will be removed from the dictionary on the next provider removal or hard update.
        private void SetDirty(IStatProvider<TValue> provider, IStatTemplate<TValue> template)
        {
            // Null checks should be in calling code.

            if (_controllerDict.ContainsKey(template))
            {
                _controllerDict[template].SetDirty();
            }
            else
            {
                var newController = new StatController<TValue>(template);
                newController.AddStatProvider(provider);
                _controllerDict.Add(template, newController);
            }

            SetCachedSetDirty();
        }
        private void SetCachedSetDirty()
        {
            _isCachedSetDirty = true;
        }

        /// <summary>
        /// Update all cached values. Unecessary for on-the-fly calculations, but useful before saving game state or before getting all stat values at once, etc...
        /// </summary>
        public void HardUpdate()
        {
            CleanUpUnusedControllers();

            UpdateDirtyControllers();

            UpdateCachedStatSet();
        }
        // Remove all stat controllers with no providers (called when removing stat controllers).
        private void CleanUpUnusedControllers()
        {
            // For each stat controller in the internal dictionary...
            foreach (var key in _controllerDict.Keys) // new list (safe to iterate)
            {
                // If the controller has no stat deltas (no stat providers)...
                if (!_controllerDict[key].HasDeltas())
                {
                    // Remove the controller from the dictionary.
                    _controllerDict.Remove(key);
                }
            }
        }
        private void UpdateDirtyControllers()
        {
            foreach (var kvp in _controllerDict)
            {
                var controller = kvp.Value;
                if (controller.IsDirty)
                {
                    controller.UpdateCache();
                }
            }
        }

        private void UpdateCachedStatSet()
        {
            _cachedSnapshotValues = new Dictionary<IStatTemplate<TValue>, TValue>();
            foreach (var kvp in _controllerDict)
            {
                var statTemplate = kvp.Key;
                var controller = kvp.Value;

                if (controller.HasDeltas())
                {
                    _cachedSnapshotValues.Add(statTemplate, controller.GetValueRaw());
                }
            }
        }
    }
}
