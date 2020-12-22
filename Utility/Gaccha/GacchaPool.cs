using System;
using System.Linq;
using System.Collections.Generic;

namespace KRpgLib.Utility.Gaccha
{
    public class GacchaPool<TCapsule>
    {
        // Private fields
        private readonly Dictionary<TCapsule, int> _pool = new Dictionary<TCapsule, int>();

        // Protected components
        protected TotalCapsuleCountHelper TotalCapsuleCount { get; }

        // Public properties
        public bool IsEmpty => _pool.All(kvp => kvp.Value == 0);    // Better than "TotalCapsuleCount.GetCacheCopy() == 0;" due to short-circuit behavior.

        // Ctors.
        private GacchaPool()
        {
            TotalCapsuleCount = new TotalCapsuleCountHelper(this);
        }
        public GacchaPool(IDictionary<TCapsule, int> pool)
            : this()
        {
            _pool = new Dictionary<TCapsule, int>(pool ?? throw new ArgumentNullException(nameof(pool)));
        }
        public GacchaPool(GacchaPool<TCapsule> other) : this(other._pool) { }

        public GacchaPool(IEnumerable<GacchaPool<TCapsule>> gacchaPools)    // DictionarySmasher putting in work again. :)
            : this(DictionarySmasher<TCapsule, int>.Smash(
                valueSmasher: (_, totalCount) => totalCount.Sum(),          // Smash by adding totals.
                dictionaries: gacchaPools.Select(p => p._pool).ToList()     // Make a list of all the gaccha pools' ...pools.
                ))
        { }

        // Public methods.
        public int GetTotalCapsuleCount() => TotalCapsuleCount.GetCacheCopy();
        public int GetCapsuleCount(TCapsule capsule)
        {
            if (IsEmpty || !_pool.TryGetValue(capsule, out int count))
            {
                return 0;
            }

            return count;
        }
        public int GetCapsuleCount(Predicate<TCapsule> predicateOrNull)
        {
            int total = TotalCapsuleCount.GetCacheCopy();

            // If no capsules left,
            if (total == 0)
            {
                return 0;
            }

            // In case you're looking for the total capsule count,
            if (predicateOrNull == null)
            {
                return total;
            }

            return _pool.Sum(kvp => predicateOrNull(kvp.Key) ? kvp.Value : 0);
        }

        public void Add(TCapsule capsule, int count)
        {
            if (_pool.ContainsKey(capsule))
            {
                _pool[capsule] += count;
            }
            else
            {
                _pool.Add(capsule, count);
            }

            TotalCapsuleCount.SetDirty_FromExternal();
        }
        public void Remove(TCapsule capsule, int count)
        {
            if (_pool.ContainsKey(capsule))
            {
                if (count < _pool[capsule])
                {
                    _pool[capsule] -= count;
                }
                else
                {
                    _pool[capsule] = 0;
                }

                TotalCapsuleCount.SetDirty_FromExternal();
            }
        }

        public bool TryFindCapsule(Predicate<TCapsule> predicateOrNull, out TCapsule capsule)
        {
            int totalNumberOfApplicableCapsules = GetCapsuleCount(predicateOrNull);

            return TryFindCapsule(predicateOrNull, totalNumberOfApplicableCapsules, out capsule);
        }
        public bool TryFindCapsule(Predicate<TCapsule> predicateOrNull, int totalNumberOfApplicableCapsules, out TCapsule capsule)
        {
            capsule = default;

            // If there are no valid capsules,
            if (totalNumberOfApplicableCapsules == 0)
            {
                // Leave and notify that capsule was not provided.
                return false;
            }

            // Exclusive upper bound.
            int randomCapsuleIndex = Environment.Rng.Next(0, totalNumberOfApplicableCapsules);

            foreach (var kvp in _pool)       // Random retrieval from weighted list courtesy of The Internet.
            {
                // Null selector means all are valid.
                if (predicateOrNull != null && !predicateOrNull(kvp.Key))
                {
                    continue;
                }

                if (randomCapsuleIndex < kvp.Value)
                {
                    capsule = kvp.Key;
                    break;
                }

                randomCapsuleIndex -= kvp.Value;
            }

            return true;
        }

        // Protected helper classes.
        protected sealed class TotalCapsuleCountHelper : ParentedCachedValueController<int, GacchaPool<TCapsule>>
        {
            public TotalCapsuleCountHelper(GacchaPool<TCapsule> parent) : base(parent) { }

            protected override int CalculateNewCache()
            {
                return Parent._pool.Sum(kvp => kvp.Value);
            }
            protected override int CreateCacheCopy(int cache) => cache;
        }
    }
}
