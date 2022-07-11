using KRpgLib.Utility;
using System.Collections.Generic;

namespace KRpgLib.Stats
{
    /// <summary>
    /// An object that manages the stats for something. This may be owned by an RPG character, an enemy, etc.
    /// </summary>
    public class StatManager : IStatSet
    {
        // The actual collection of providers (static and dynamic) as well as the delta collection cache.
        private readonly StatProviderCollection _statProviders;

        private readonly CachedSnapshotHelper _cachedSnapshot;  //always starts dirty.

        /// <summary>
        /// Create a new stat manager. Use AddStatProvider() to pass in components that are stat providers such as class/race managers, affix managers, status effect managers, and passive ability managers.
        /// </summary>
        public StatManager()
        {
            _statProviders = new StatProviderCollection();
            _cachedSnapshot = new CachedSnapshotHelper(this);

            _statProviders.OnStatsChanged += UpdateCachedSnapshot;
        }
        ~StatManager()
        {
            _statProviders.OnStatsChanged -= UpdateCachedSnapshot;
        }

        /// <summary>
        /// Add a stat provider to the stat manager.
        /// </summary>
        /// <param name="provider">an IStatProvider</param>
        public bool AddStatProvider(IStatProvider provider)
        {
            // Null check.
            return AddStatProvider_Internal(provider ?? throw new System.ArgumentNullException(nameof(provider)));
        }
        /// <summary>
        /// Add a dynamic stat provider to the stat manager. Subscribes to the OnStatsChanged event.
        /// </summary>
        /// <param name="provider">an IDynamicStatProvider</param>
        public bool AddStatProvider(IDynamicStatProvider provider)
        {
            return AddStatProvider_Internal(provider ?? throw new System.ArgumentNullException(nameof(provider)));
        }

        /// <summary>
        /// Remove a stat provider from the stat manager (will no longer be queried for deltas when updating stat cache).
        /// </summary>
        /// <param name="statProvider">an IStatProvider</param>
        public void RemoveStatProvider(IStatProvider statProvider)
        {
            // Null check.
            RemoveStatProvider_Internal(statProvider ?? throw new System.ArgumentNullException(nameof(statProvider)));
        }
        /// <summary>
        /// Remove a dynamic stat provider from the stat manager (will no longer be queried for deltas when updating stat cache). Unsubscribes from the OnStatsChanged event.
        /// </summary>
        /// <param name="statProvider">an IDynamicStatProvider</param>
        public void RemoveStatProvider(IDynamicStatProvider statProvider)
        {
            // Null check.
            RemoveStatProvider_Internal(statProvider ?? throw new System.ArgumentNullException(nameof(statProvider)));
        }

        /// <summary>
        /// Get the current raw value of a single stat.
        /// </summary>
        /// <param name="stat">any stat</param>
        /// <returns>raw value of the stat</returns>
        public int GetStatValue(Stat stat) => _cachedSnapshot.GetCacheCopy().GetStatValue(stat);
        /// <summary>
        /// Get the current legalized value of a single stat.
        /// </summary>
        /// <param name="stat">any stat</param>
        /// <returns>legalized value of the stat</returns>
        public int GetStatValueLegalized(Stat stat) => _cachedSnapshot.GetCacheCopy().GetStatValueLegalized(stat);

        /// <summary>
        /// Get a snapshot of all current stat values.
        /// </summary>
        public StatSnapshot GetStatSnapshot() => _cachedSnapshot.GetCacheCopy();

        private bool AddStatProvider_Internal(IStatProvider safeProvider)
        {
            // Checks done in public accessor methods.

            return _statProviders.AddProvider(safeProvider);
        }
        private bool AddStatProvider_Internal(IDynamicStatProvider safeProvider)
        {
            return _statProviders.AddProvider(safeProvider);
        }
        private bool RemoveStatProvider_Internal(IStatProvider safeProvider)
        {
            // Checks done in public accessor methods.

            return _statProviders.RemoveProvider(safeProvider);
        }
        private bool RemoveStatProvider_Internal(IDynamicStatProvider safeProvider)
        {
            // Checks done in public accessor methods.

            return _statProviders.RemoveProvider(safeProvider);
        }

        private void UpdateCachedSnapshot()
        {
            _cachedSnapshot.SetDirty_FromExternal();
        }

        protected class CachedSnapshotHelper : CachedValueController<StatSnapshot, StatManager>
        {
            public CachedSnapshotHelper(StatManager context) : base(context) { }
            protected override StatSnapshot CalculateNewCache()
            {
                return Context._statProviders.GetDeltaCollection().GetSnapshot();
            }
            protected override StatSnapshot CreateCacheCopy(StatSnapshot cache)
            {
                // Stat snapshots are not modifiable after instantiation. Safe to pass by reference.
                return cache;
            }
        }
    }
}
