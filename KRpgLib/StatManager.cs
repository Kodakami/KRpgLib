using System.Collections.Generic;
using KRpgLib.Utility;

namespace KRpgLib.Stats
{
    /// <summary>
    /// Base class for an object the manages the stats for something. This may be owned by an RPG character, an enemy, etc... Your implementation should take a series of components in the constructor and apply them as initial stat providers.
    /// </summary>
    public class StatManager<TValue> : IStatSet<TValue> where TValue : struct
    {
        private readonly StatProviderCollection<TValue> _statProviders;

        private readonly CachedSnapshotHelper _cachedSnapshot;  //always starts dirty.

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
                // Null list item check after the jump.
                AddStatProvider(provider);
            }
        }
        /// <summary>
        /// Remove a stat provider from the stat manager (will no longer be queried for stat deltas when updating stat cache).
        /// </summary>
        /// <param name="statProvider">an IStatProvider</param>
        public void RemoveStatProvider(IStatProvider<TValue> statProvider)
        {
            // Null check.
            RemoveStatProvider_Internal(statProvider ?? throw new System.ArgumentNullException(nameof(statProvider)));
        }
        /// <summary>
        /// Remove a list of stat providers from the stat manager (will no longer be queried for stat deltas when updating stat cache).
        /// </summary>
        /// <param name="statProviders">an IStatProvider list</param>
        public void RemoveStatProviders(IEnumerable<IStatProvider<TValue>> statProviders)
        {
            // Null check.
            if (statProviders == null)
            {
                throw new System.ArgumentNullException(nameof(statProviders));
            }

            foreach (var provider in statProviders)
            {
                // Null list item check after the jump.
                RemoveStatProvider(provider);
            }
        }

        /// <summary>
        /// Get the current raw value of a single stat.
        /// </summary>
        /// <param name="statTemplate">any stat template</param>
        /// <returns>raw value of the stat</returns>
        public TValue GetStatValue(IStatTemplate<TValue> statTemplate) => _cachedSnapshot.GetCacheCopy().GetStatValue(statTemplate);
        /// <summary>
        /// Get the current legalized value of a single stat.
        /// </summary>
        /// <param name="statTemplate">any stat template</param>
        /// <returns>legalized value of the stat</returns>
        public TValue GetStatValueLegalized(IStatTemplate<TValue> statTemplate) => _cachedSnapshot.GetCacheCopy().GetStatValueLegalized(statTemplate);

        /// <summary>
        /// Get a snapshot of all current stat values.
        /// </summary>
        public StatSnapshot<TValue> GetStatSnapshot() => _cachedSnapshot.GetCacheCopy();

        private void AddStatProvider_Internal(IStatProvider<TValue> safeStatProvider)
        {
            // Checks done in public accessor methods.

            _statProviders.AddProvider(safeStatProvider);
        }
        private void RemoveStatProvider_Internal(IStatProvider<TValue> safeStatProvider)
        {
            // Checks done in public accessor methods.

            _statProviders.RemoveProvider(safeStatProvider);
        }

        protected class CachedSnapshotHelper : ParentedCachedValueController<StatSnapshot<TValue>, StatManager<TValue>>
        {
            public CachedSnapshotHelper(StatManager<TValue> parent) : base(parent) { }
            protected override StatSnapshot<TValue> CalculateNewCache()
            {
                return StatSnapshot<TValue>.Create(Parent._statProviders.GetStatDeltaCollection());
            }
            protected override StatSnapshot<TValue> CreateCacheCopy(StatSnapshot<TValue> cache)
            {
                // Stat snapshots are not modifiable after instantiation. Safe to pass by reference.
                return cache;
            }
        }
    }
}
