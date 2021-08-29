using System;
using System.Collections.Generic;
using KRpgLib.Utility;
using System.Linq;
using KRpgLib.Utility.KomponentObject;

namespace KRpgLib.Stats
{
    /// <summary>
    /// A read-only collection of changes to stat values. The values inside the instance cannot be changed after instantiation. Create a new StatDeltaCollection with the desired values and combine them via a combination constructor.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public sealed class StatDeltaCollection<TValue> where TValue : struct
    {
        // Internal fields.

        // Dictionary of dictionaries. [Key1: StatTemplate]->[Key2: StatDeltaType]->[Value: TValue].
        private readonly Dictionary<IStatTemplate<TValue>, Dictionary<StatDeltaType<TValue>, TValue>> _superDict;
        private readonly SnapshotCacheHelper _snapshotCache;

        /// <summary>
        /// Count the total number of stat deltas represented by the collection. Does not utilize cached values - use with some caution.
        /// </summary>
        public int CountValues() => _superDict.Values.Sum(subd => subd.Count);

        // Ctors.

        // Called by the public constructors before their own ctor code.
        private StatDeltaCollection()
        {
            _snapshotCache = new SnapshotCacheHelper(this);
            _superDict = new Dictionary<IStatTemplate<TValue>, Dictionary<StatDeltaType<TValue>, TValue>>();
        }
        /// <summary>
        /// Create a new StatDeltaCollection with the combined values from multiple others.
        /// </summary>
        /// <param name="combineFrom">a collection of other StatDeltaCollections</param>
        public StatDeltaCollection(IEnumerable<StatDeltaCollection<TValue>> combineFrom)
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
                        // For each (template, (deltatype, value)) pair in the other's super dict...
                        foreach (var otherSuperKvp in otherCollection._superDict)
                        {
                            // For each (deltatype, value) pair in the template's sub dict...
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
        /// Create a new StatDeltaCollection from a collection of individual stat templates and deltas. This is the manual option for creating a new instance.
        /// </summary>
        /// <param name="statTemplateAndDeltas"></param>
        public StatDeltaCollection(IEnumerable<StatTemplateAndDelta<TValue>> statTemplateAndDeltas)
            :this()
        {
            // If the collection of deltas is not null.
            if (statTemplateAndDeltas != null)
            {
                // For each delta.
                foreach (var stad in statTemplateAndDeltas)
                {
                    // If the delta value is not default(TValue),
                    if (!stad.DeltaValue.Equals(default(TValue)))
                    {
                        // Stat template and delta type are both null-checked on creation of the instance.
                        Add_Internal(stad.Template, stad.DeltaType, stad.DeltaValue);
                    }

                    // If the delta value is default, we'll silently ignore it (default delta value indicates no change).
                }
            }

            // Set the snapshot cache dirty.
            _snapshotCache.SetDirty_FromExternal();
        }
        public StatDeltaCollection(params StatTemplateAndDelta<TValue>[] statTemplateAndDeltas)
            : this((IEnumerable<StatTemplateAndDelta<TValue>>)statTemplateAndDeltas) { }

        // Private methods.
        private void Add_Internal(IStatTemplate<TValue> statTemplate, StatDeltaType<TValue> deltaType, TValue deltaValue)
        {
            // Try to get the sub dict for the stat. If it wasn't found,
            if (!_superDict.TryGetValue(statTemplate, out Dictionary<StatDeltaType<TValue>, TValue> subDict))
            {
                // Create a new sub dict.
                subDict = new Dictionary<StatDeltaType<TValue>, TValue>();

                // Add the sub dict to the super dict.
                _superDict[statTemplate] = subDict;
            }

            // Try to get the value for the delta type. If it wasn't found,
            if (!subDict.TryGetValue(deltaType, out TValue existingValue))
            {
                // Add the new value to the subdict.
                subDict[deltaType] = deltaValue;

                // And return.
                return;
            }

            // If an existing value was found, combine the existing and new values with stat delta type's combination method.
            subDict[deltaType] = deltaType.Combine(existingValue, deltaValue);
        }

        // Public methods.

        /// <summary>
        /// Get a snapshot of the stat values in the collection. Uses a cache, so lookups after the first one will be painless.
        /// </summary>
        public StatSnapshot<TValue> GetStatSnapshot() => _snapshotCache.GetCacheCopy();
        public TValue GetDeltaValue(IStatTemplate<TValue> statTemplate, StatDeltaType<TValue> deltaType)
        {
            // If the stat template and delta type have an entry for the value (throwing ArgNullEx for any null args),
            if (_superDict.TryGetValue(
                statTemplate ?? throw new ArgumentNullException(nameof(statTemplate)),
                out Dictionary<StatDeltaType<TValue>, TValue> subDict)
                && subDict.TryGetValue(
                    deltaType ?? throw new ArgumentNullException(nameof(deltaType)),
                    out TValue value))
            {
                // Return the value.
                return value;
            }

            // Otherwise return default value (not the stat template's default value, since this method returns the delta value).
            return default;
        }

        // Private helper classes.
        private sealed class SnapshotCacheHelper : CachedValueController<StatSnapshot<TValue>, StatDeltaCollection<TValue>>
        {
            public SnapshotCacheHelper(StatDeltaCollection<TValue> context) : base(context) { }

            protected override StatSnapshot<TValue> CalculateNewCache()
            {
                // Create a new dictionary to store calculated values.
                var dict = new Dictionary<IStatTemplate<TValue>, TValue>();

                // For each stat template...
                foreach (var kvp in Context._superDict)
                {
                    // Start with the default value.
                    TValue statValue = kvp.Key.DefaultValue;

                    // For each type of stat delta (addition, multiplication)...
                    foreach (StatDeltaType<TValue> deltaType in StatDeltaType<TValue>.GetAllByPriority())
                    {
                        // If there is a delta of that type...
                        if (kvp.Value.TryGetValue(deltaType, out TValue deltaValue))
                        {
                            // Get the combined value (combining baseline value and delta value).
                            TValue combinedValues = deltaType.Combine(deltaType.BaselineValue, deltaValue);

                            // Apply the value to the current stat value.
                            statValue = deltaType.Apply(statValue, combinedValues);
                        }
                    }

                    // Add the stat value to the dictionary.
                    dict.Add(kvp.Key, statValue);
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