using System;
using System.Collections.Generic;
using KRpgLib.Utility;
using System.Linq;
using KRpgLib.Utility.KomponentObject;

namespace KRpgLib.Stats
{
    /// <summary>
    /// A read-only collection of changes to stat values. The values inside the instance cannot be changed after instantiation. Create a new DeltaCollection with the desired values and combine them via a combination constructor.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class DeltaCollection<TValue> where TValue : struct
    {
        // Internal fields.

        // Dictionary of dictionaries. [Key1: Stat]->[Key2: DeltaType]->[Value: TValue].
        private readonly Dictionary<IStat<TValue>, Dictionary<DeltaType<TValue>, TValue>> _superDict;
        private readonly SnapshotCacheHelper _snapshotCache;

        /// <summary>
        /// Count the total number of stat deltas represented by the collection. Does not utilize cached values - use with some caution.
        /// </summary>
        public int CountValues() => _superDict.Values.Sum(subd => subd.Count);

        // Ctors.

        // Called by the public constructors before their own ctor code.
        private DeltaCollection()
        {
            _snapshotCache = new SnapshotCacheHelper(this);
            _superDict = new Dictionary<IStat<TValue>, Dictionary<DeltaType<TValue>, TValue>>();
        }
        /// <summary>
        /// Create a new DeltaCollection with the combined values from multiple others.
        /// </summary>
        /// <param name="combineFrom">a collection of other DeltaCollections</param>
        public DeltaCollection(IEnumerable<DeltaCollection<TValue>> combineFrom)
            :this()
        {
            // If the collection of collections is not null.
            if (combineFrom != null)
            {
                // For each collection to combine.
                foreach (var otherCollection in combineFrom)
                {
                    // If calling code didn't pass a null collection for some reason.
                    if (otherCollection != null)
                    {
                        // For each (stat (deltatype, value)) pair in the other's super dict...
                        foreach (var otherSuperKvp in otherCollection._superDict)
                        {
                            // For each (deltatype, value) pair in the stat's sub dict...
                            foreach (var otherSubKvp in otherSuperKvp.Value)
                            {
                                // Add the value to this collection. (Use the delta type's combine method to combine the values)
                                Add_Internal(otherSuperKvp.Key, otherSubKvp.Key, otherSubKvp.Value);
                            }
                        }
                    }
                }
            }

            // Set the snapshot cache dirty.
            _snapshotCache.SetDirty_FromExternal();
        }
        /// <summary>
        /// Create a new DeltaCollection from a collection of individual stats and deltas. This is the manual option for creating a new instance.
        /// </summary>
        /// <param name="statDeltas">a collection of StatDeltas</param>
        public DeltaCollection(IEnumerable<StatDelta<TValue>> statDeltas)
            :this()
        {
            // If the collection of deltas is not null.
            if (statDeltas != null)
            {
                // For each delta.
                foreach (var stad in statDeltas)
                {
                    // If the delta value is not default(TValue),
                    if (!stad.DeltaValue.Equals(default(TValue)))
                    {
                        // Stat and delta type are both null-checked on creation of the instance.
                        Add_Internal(stad.Stat, stad.DeltaType, stad.DeltaValue);
                    }

                    // If the delta value is default, we'll silently ignore it (default delta value indicates no change).
                }
            }

            // Set the snapshot cache dirty.
            _snapshotCache.SetDirty_FromExternal();
        }
        /// <summary>
        /// Create a new DeltaCollection from a collection of individual stats and deltas. This is the manual option for creating a new instance.
        /// </summary>
        public DeltaCollection(params StatDelta<TValue>[] statDeltas)
            : this((IEnumerable<StatDelta<TValue>>)statDeltas) { }

        // Private methods.
        private void Add_Internal(IStat<TValue> stat, DeltaType<TValue> deltaType, TValue deltaValue)
        {
            // Try to get the sub dict for the stat. If it wasn't found,
            if (!_superDict.TryGetValue(stat, out Dictionary<DeltaType<TValue>, TValue> subDict))
            {
                // Create a new sub dict.
                subDict = new Dictionary<DeltaType<TValue>, TValue>();

                // Add the sub dict to the super dict.
                _superDict[stat] = subDict;
            }

            // Try to get the value for the delta type. If it wasn't found,
            if (!subDict.TryGetValue(deltaType, out TValue existingValue))
            {
                // Add the new value to the subdict.
                subDict[deltaType] = deltaValue;

                // And return.
                return;
            }

            // If an existing value was found, combine the existing and new values with delta type's combination method.
            subDict[deltaType] = deltaType.Combine(existingValue, deltaValue);
        }

        // Public methods.

        /// <summary>
        /// Get a snapshot of all the stat values in the collection. Uses a cache, so lookups after the first one will be painless.
        /// </summary>
        public StatSnapshot<TValue> GetSnapshot() => _snapshotCache.GetCacheCopy();
        public TValue GetDeltaValue(IStat<TValue> stat, DeltaType<TValue> deltaType)
        {
            // If the stat and delta type have an entry for the value (throwing ArgNullEx for any null args),
            if (_superDict.TryGetValue(
                stat ?? throw new ArgumentNullException(nameof(stat)),
                out Dictionary<DeltaType<TValue>, TValue> subDict)
                && subDict.TryGetValue(
                    deltaType ?? throw new ArgumentNullException(nameof(deltaType)),
                    out TValue value))
            {
                // Return the value.
                return value;
            }

            // Otherwise return default value (not the stat's default value, since this method returns the delta value).
            return default;
        }

        // Private helper classes.
        private sealed class SnapshotCacheHelper : CachedValueController<StatSnapshot<TValue>, DeltaCollection<TValue>>
        {
            public SnapshotCacheHelper(DeltaCollection<TValue> context) : base(context) { }

            protected override StatSnapshot<TValue> CalculateNewCache()
            {
                // Create a new dictionary to store calculated values.
                var dict = new Dictionary<IStat<TValue>, TValue>();

                // Grab a handle to the deltaTypes list by priority.
                var orderedDeltaTypes = StatEnvironment<TValue>.Instance.DeltaTypes.GetAllByPriority();

                // For each stat...
                foreach (var kvp in Context._superDict)
                {
                    // Start with the default value.
                    TValue statTotal = kvp.Key.DefaultValue;

                    // For each registered delta type (addition, multiplication)...
                    foreach (DeltaType<TValue> deltaType in orderedDeltaTypes)
                    {
                        // If there is a delta of that type...
                        if (kvp.Value.TryGetValue(deltaType, out TValue deltaValue))
                        {
                            // Get the combined value (combining baseline and delta).
                            TValue combinedValues = deltaType.Combine(deltaType.BaselineValue, deltaValue);

                            // Apply the value to the current total.
                            statTotal = deltaType.Apply(statTotal, combinedValues);
                        }
                    }

                    // Add the stat total to the dictionary.
                    dict.Add(kvp.Key, statTotal);
                }

                return StatSnapshot<TValue>.Create(dict);
            }

            protected override StatSnapshot<TValue> CreateCacheCopy(StatSnapshot<TValue> cache)
            {
                // Safe to return read-only sealed class by reference.
                return cache;
            }
        }
    }
}