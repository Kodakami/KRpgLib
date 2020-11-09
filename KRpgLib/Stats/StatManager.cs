using KRpgLib.Stats.Compound;
using System.Collections.Generic;

namespace KRpgLib.Stats
{
    /// <summary>
    /// Base class for an object the manages the stats for something. This may be owned by an RPG character, an enemy, etc... Your implementation should take a series of components in the constructor and apply them as initial stat providers.
    /// </summary>
    public class StatManager : AbstractStatSet
    {
        private readonly Dictionary<IStatTemplate, StatController> _controllerDict =
            new Dictionary<IStatTemplate, StatController>();

        private readonly List<IStatProvider> _statProviders = new List<IStatProvider>();

        private Dictionary<IStatTemplate, float> _cachedSnapshotValues;     //Assigned and converted to StatSet before returning value.
        private bool _isCachedSetDirty = true;

        //ctor
        public StatManager()
        {
            // Nothing. :D
            // Pass in other components that are stat providers such as class/race managers, arpg-style modifier managers, status effect managers, and passive ability managers.
        }
        public StatManager(IEnumerable<IStatProvider> initStatProviders)
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
        public void AddStatProvider(IStatProvider statProvider)
        {
            // Null check.
            if (statProvider != null)
            {
                AddStatProvider_Internal(statProvider);
            }
        }
        /// <summary>
        /// Add a list of stat providers to the stat manager.
        /// </summary>
        /// <param name="statProviders">an IStatProvider list</param>
        public void AddStatProviders(List<IStatProvider> statProviders)
        {
            // Null check.
            if (statProviders == null)
            {
                return;
            }

            foreach (var provider in statProviders)
            {
                if (provider != null)
                {
                    AddStatProvider_Internal(provider);
                }
            }
        }

        // Internal provider addition method.
        private void AddStatProvider_Internal(IStatProvider statProvider)
        {
            // Checks done in public accessor methods.

            // Subscribe to event.
            statProvider.OnStatDeltasChanged += SetDirty;

            // Get dirty (get all stats that have deltas from provider).
            var dirtyList = statProvider.GetStatsWithDeltas();

            // For each stat modified by provider...
            foreach (var dirtyStat in dirtyList)
            {
                // Get the controller (or make a new one).
                var controller = GetOrCreateController(dirtyStat);

                // And add this stat provider to its list (this will flag the controller as dirty).
                controller.AddStatProvider(statProvider);
            }

            // All clear.
            _statProviders.Add(statProvider);
        }
        /// <summary>
        /// Remove a stat provider from the stat manager (will no longer be queried for stat deltas when updating stat cache).
        /// </summary>
        /// <param name="statProvider">an IStatProvider</param>
        public void RemoveStatProvider(IStatProvider statProvider)
        {
            // Null check, not in list check.
            if (statProvider != null && _statProviders.Contains(statProvider))
            {
                // Removal process.
                RemoveStatProvider_Internal(statProvider);

                // Clean up controller dictionary.
                CleanUpUnusedControllers();
            }
        }
        /// <summary>
        /// Remove a list of stat providers from the stat manager (will no longer be queried for stat deltas when updating stat cache).
        /// </summary>
        /// <param name="statProviders">an IStatProvider list</param>
        public void RemoveStatProviders(List<IStatProvider> statProviders)
        {
            // Null check, empty list check.
            if (statProviders == null || statProviders.Count == 0)
            {
                return;
            }

            foreach (var provider in statProviders)
            {
                // Null check, not in list check.
                if (provider != null && _statProviders.Contains(provider))
                {
                    RemoveStatProvider_Internal(provider);
                }
            }

            // Clean up controller dictionary.
            CleanUpUnusedControllers();
        }

        // Internal provider removal method.
        private void RemoveStatProvider_Internal(IStatProvider statProvider)
        {
            // Checks in public accessor methods.

            // Unsubscribe from event.
            statProvider.OnStatDeltasChanged -= SetDirty;

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
        protected override float GetStatValue_Internal(IStatTemplate safeStatTemplate)
        {
            // Null checks unnecessary at this point.

            if (!_controllerDict.ContainsKey(safeStatTemplate))
            {
                return safeStatTemplate.DefaultValue;
            }

            return _controllerDict[safeStatTemplate].GetValueRaw();
        }
        protected override float GetCompoundStatValue_Internal(ICompoundStatTemplate safeCompoundStatTemplate)
        {
            // Null checks unnecessary at this point.

            return safeCompoundStatTemplate.Algorithm.CalculateValue(this);
        }

        /// <summary>
        /// Gets a snapshot of all current stat values. Will utilize cached values where possible.
        /// </summary>
        /// <returns>new StatSnapshot</returns>
        public StatSnapshot GetStatSnapshot()
        {
            if (_isCachedSetDirty)
            {
                UpdateCachedStatSet();
            }

            return new StatSnapshot(_cachedSnapshotValues);
        }

        private StatController GetOrCreateController(IStatTemplate statTemplate)
        {
            if (_controllerDict.ContainsKey(statTemplate))
            {
                return _controllerDict[statTemplate];
            }

            var newController = new StatController(statTemplate);
            _controllerDict.Add(statTemplate, newController);
            return newController;
        }

        // Mark a stat controller as in need of recalculation (or add the controller if a provider update added a stat). Stat controllers whose providers were all removed in this way will be removed from the dictionary on the next provider removal or hard update.
        private void SetDirty(IStatProvider provider, IStatTemplate template)
        {
            if (_controllerDict.ContainsKey(template))
            {
                _controllerDict[template].SetDirty();
            }
            else
            {
                var newController = new StatController(template);
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
            _cachedSnapshotValues = new Dictionary<IStatTemplate, float>();
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
