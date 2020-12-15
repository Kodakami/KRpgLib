using System;
using System.Linq;
using System.Collections.Generic;

namespace KRpgLib.Utility.Gaccha
{
    public class Gaccha<TCapsule>
    {
        protected readonly GacchaPool<TCapsule> Pool;

        public bool IsEmpty => Pool.IsEmpty;

        public Gaccha(GacchaPool<TCapsule> pool)
        {
            Pool = pool ?? throw new ArgumentNullException(nameof(pool));
        }
        public double GetProbability(TCapsule capsule)
        {
            int total = Pool.GetTotalCapsuleCount();
            if (total != 0)
            {
                int count = Pool.GetCapsuleCount(capsule);
                if (count != 0)
                {
                    return count / total;
                }
            }
            return 0;
        }
        public int GetTotalCapsuleCount() => Pool.GetTotalCapsuleCount();
        public double GetCombinedProbability(Predicate<TCapsule> predicateOrNull)
        {
            int total = Pool.GetTotalCapsuleCount();

            // If no capsules left,
            if (total == 0)
            {
                // 0% chance of selecting a capsule of any kind.
                return 0;
            }

            // In case you're looking for the probability of selecting any capsule when selecting any capsule:
            if (predicateOrNull == null)
            {
                // 100% chance of rain while raining.
                return 1;
            }

            int predicateTotal = Pool.GetCapsuleCount(predicateOrNull);

            // If no valid capsules for predicate,
            if (predicateTotal == 0)
            {
                // 0% chance of success.
                return 0;
            }

            // Safe division. Returned earlier if total was 0.
            return predicateTotal / total;
        }

        public void Add(GacchaResult<TCapsule> result) => Pool.Add(result.Capsule, result.Count);
        public void Remove(GacchaResult<TCapsule> result) => Pool.Remove(result.Capsule, result.Count);

        /*
         *      Below can be found a disgusting amount of copy-pasted methods, detailing two parallel branches of getting capsules from the pool.
         *      The ones that just find capsules and don't register them as needing to be removed,
         *      and the ones that pon capsules, with a flag for whether they should or should not be removed from the pool afterward.
         *      
         *      The two sets of methods are necessary (at least the virtual methods they call at the end),
         *      so that subclasses can record pons (and not regular searches), and potentially modify the results (of pons, but not searches).
         *      
         *      There is almost certainly a way to elegantly unite these two disparate houses, but if you can find it, you're a smarter coder than I am today.
         *      
         *      - Eric  12/14/20
         */

        public bool TryFindMany(PredicateChain<TCapsule> predicateChain, int count, out IEnumerable<TCapsule> capsules)
        {
            // Create the list of results.
            var capsuleList = new List<TCapsule>();

            // Create a handle for the selected capsule.
            TCapsule selected = default;
            int countRemaining = count;

            // For each predicate in the chain...
            foreach (var predicate in predicateChain)
            {
                // If all capsules have been found, we can stop looping.
                if (countRemaining == 0)
                {
                    break;
                }

                // Count the total number of capsules that apply to this predicate (if null, total capsule count).
                int totalNumberOfApplicableCapsules = Pool.GetCapsuleCount(predicate);

                // For the number of capsules left to find...
                for (int i = 0; i < countRemaining; i++)
                {
                    // If we found a capsule,
                    if (TryFind_Internal(predicate, totalNumberOfApplicableCapsules, out selected))
                    {
                        // Add it to the list.
                        capsuleList.Add(selected);
                    }
                    // Otherwise (we couldn't find an applicable capsule),
                    else
                    {
                        // No valid capsules left for predicate. Try the next predicate.
                        break;
                    }
                }
            }

            // It is possible that not all capsules have been found by this point. If so, it is not possible to fulfill the predicate chain with the current state of the gaccha.

            capsules = capsuleList;
            return capsules.Count() == count;
        }
        public bool TryPonMany(PredicateChain<TCapsule> predicateChain, int count, bool withReplacement, out IEnumerable<TCapsule> capsules)
        {
            // Create the list of results.
            var capsuleList = new List<TCapsule>();

            // Create a handle for the selected capsule.
            TCapsule selected = default;
            int countRemaining = count;

            // For each predicate in the chain...
            foreach (var predicate in predicateChain)
            {
                // If all capsules have been found, we can stop looping.
                if (countRemaining == 0)
                {
                    break;
                }

                // Count the total number of capsules that apply to this predicate (if null, total capsule count).
                int totalNumberOfApplicableCapsules = Pool.GetCapsuleCount(predicate);

                // For the number of capsules left to find...
                for (int i = 0; i < countRemaining; i++)
                {
                    // If we found a capsule,
                    if (TryPon_Internal(predicate, totalNumberOfApplicableCapsules, withReplacement, out selected))
                    {
                        // Add it to the list.
                        capsuleList.Add(selected);
                    }
                    // Otherwise (we couldn't find an applicable capsule),
                    else
                    {
                        // No valid capsules left for predicate. Try the next predicate.
                        break;
                    }
                }
            }

            // It is possible that not all capsules have been found by this point. If so, it is not possible to fulfill the predicate chain with the current state of the gaccha.

            capsules = capsuleList;
            return capsules.Count() == count;
        }
        /// <summary>
        /// Try and find a number of capsules in the gaccha which fulfill the predicate. The capsules are never removed. Returns true if all required capsules were found.
        /// </summary>
        public bool TryFindMany(Predicate<TCapsule> predicateOrNull, int count, out IEnumerable<TCapsule> capsules)
        {
            // Create the list of results.
            var capsuleList = new List<TCapsule>();

            // Create a handle for the selected capsule.
            TCapsule selected = default;

            // Count the total number of capsules that apply to this predicate (if not null).
            int totalNumberOfApplicableCapsules = Pool.GetCapsuleCount(predicateOrNull);

            // For the number of capsules we are finding...
            for (int i = 0; i < count; i++)
            {
                // If we found a capsule,
                if (TryFind_Internal(predicateOrNull, totalNumberOfApplicableCapsules, out selected))
                {
                    // Add it to the list.
                    capsuleList.Add(selected);
                }
                // Otherwise (we couldn't find an applicable capsule),
                else
                {
                    // No valid capsules left. Time to go.
                    break;
                }
            }

            capsules = capsuleList;
            return capsules.Count() == count;
        }
        /// <summary>
        /// Try and pon a number of capsules from the gaccha. Returns true if all required capsules were ponned.
        /// </summary>
        public bool TryPonMany(Predicate<TCapsule> predicateOrNull, int count, bool withReplacement, out IEnumerable<TCapsule> capsules)
        {
            // Create the list of results.
            var capsuleList = new List<TCapsule>();

            // Create a handle for the selected capsule.
            TCapsule selected = default;

            // Count the total number of items that apply to this predicate (if not null).
            int totalNumberOfApplicableCapsules = Pool.GetCapsuleCount(predicateOrNull);

            // For the number of items we are ponning...
            for (int i = 0; i < count; i++)
            {
                // If we could pon the capsule,
                if (TryPon_Internal(predicateOrNull, totalNumberOfApplicableCapsules, withReplacement, out selected))
                {
                    // Add it to the list.
                    capsuleList.Add(selected);
                }
                // Otherwise (we couldn't find an applicable capsule),
                else
                {
                    // No valid capsules left. Time to go.
                    break;
                }
            }

            capsules = capsuleList;
            return capsules.Count() == count;
        }
        public bool TryFindSingle(PredicateChain<TCapsule> predicateChain, out TCapsule capsule)
        {
            capsule = default;

            foreach (var predicate in predicateChain)
            {
                if (TryFindSingle(predicate, out capsule))
                {
                    return true;
                }

                // Try the next predicate in the chain.
            }

            return false;
        }
        public bool TryPonSingle(PredicateChain<TCapsule> predicateChain, bool withReplacement, out TCapsule capsule)
        {
            capsule = default;

            foreach (var predicate in predicateChain)
            {
                if (TryPonSingle(predicate, withReplacement, out capsule))
                {
                    return true;
                }

                // Try the next predicate in the chain.
            }

            return false;
        }
        /// <summary>
        /// Try and find a capsule in the gaccha which fulfills the predicate. Returns true if the capsule was found.
        /// </summary>
        public bool TryFindSingle(Predicate<TCapsule> predicateOrNull, out TCapsule capsule)
        {
            int totalNumberOfApplicableCapsules = Pool.GetCapsuleCount(predicateOrNull);

            return TryFind_Internal(predicateOrNull, totalNumberOfApplicableCapsules, out capsule);
        }
        /// <summary>
        /// Try and pon a capsule from the gaccha which fulfills the predicate. Returns true if the capsule was ponned.
        /// </summary>
        public bool TryPonSingle(Predicate<TCapsule> predicateOrNull, bool withReplacement, out TCapsule capsule)
        {
            int totalNumberOfApplicableCapsules = Pool.GetCapsuleCount(predicateOrNull);

            return TryPon_Internal(predicateOrNull, totalNumberOfApplicableCapsules, withReplacement, out capsule);
        }

        protected bool TryFind_Internal(Predicate<TCapsule> predicateOrNull, int totalNumberOfApplicableCapsules, out TCapsule capsule)
        {
            return Pool.TryGetCapsule(predicateOrNull, totalNumberOfApplicableCapsules, out capsule);
        }
        /// <summary>
        /// A gateway between TryPonMany/TryPonSingle and the internal private method for finding capsules. Override this to intercept/observe/record/modify the call in transit. Remember to call base method if applicable before returning.
        /// </summary>
        protected bool TryPon_Internal(Predicate<TCapsule> predicateOrNull, int totalNumberOfApplicableCapsules, bool withReplacement, out TCapsule capsule)
        {
            if (!Pool.TryGetCapsule(predicateOrNull, totalNumberOfApplicableCapsules, out capsule))
            {
                return false;
            }

            if (SupplyAlternatePonnedCapsuleIfApplicable(capsule, out TCapsule alternate))
            {
                capsule = alternate;
            }

            // If we are removing the capsule from the pool,
            if (!withReplacement)
            {
                // Do so.
                Pool.Remove(capsule, 1);
            }

            return true;
        }
        protected virtual bool SupplyAlternatePonnedCapsuleIfApplicable(TCapsule original, out TCapsule capsule)
        {
            capsule = original;
            return false;
        }
    }

    public class SupervisedGaccha<TCapsule> : Gaccha<TCapsule>
    {
        protected GacchaLifeguardChain<TCapsule> LifeguardChain { get; }

        public SupervisedGaccha(GacchaPool<TCapsule> other, GacchaLifeguardChain<TCapsule> lifeguardChain) : base(other)
        {
            LifeguardChain = lifeguardChain ?? throw new ArgumentNullException(nameof(lifeguardChain));
        }

        public IEnumerable<double> GetCurrentLifeguardProcProbabilities() => LifeguardChain.GetCurrentProbabilities();
        public void IncreasePonCountArtificially(int count) => LifeguardChain.ObservePons(count);
        public void ResetLifeguards() => LifeguardChain.ResetPonsObserved();

        protected override bool SupplyAlternatePonnedCapsuleIfApplicable(TCapsule original, out TCapsule capsule)
        {
            return LifeguardChain.RollForProcAndReturnCapsuleIfSuccessful(this, out capsule);
        }
    }
}
