using System;

namespace KRpgLib.Utility.Gaccha
{
    public struct PonArgs<TCapsule>
    {
        public PonArgs(Predicate<TCapsule> predicateOrNull, int count, bool withReplacement)
        {
            PredicateOrNull = predicateOrNull;
            Count = count;
            WithReplacement = withReplacement;
        }

        /// <summary>
        /// Predicate used to determine which capsules in the pool are valid selections. If null, all capsules will be considered valid (x => true).
        /// </summary>
        public Predicate<TCapsule> PredicateOrNull { get; }
        /// <summary>
        /// The number of capsules to pon.
        /// </summary>
        public int Count { get; }
        /// <summary>
        /// If true, the capsule will be replaced into the pool after adding to the list (it can be ponned again with the same probability).
        /// </summary>
        public bool WithReplacement { get; }
    }
}
