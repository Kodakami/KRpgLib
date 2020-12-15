using System;
using System.Linq;
using System.Collections.Generic;

namespace KRpgLib.Utility.Gaccha
{
    public class GacchaPack<TCapsule>
    {
        public List<GacchaGuarantee<TCapsule>> Guarantees { get; }
        public int BasicPonCount { get; }

        public GacchaPack(IEnumerable<GacchaGuarantee<TCapsule>> guarantees = null, int basicPonCount = 0)
        {
            Guarantees = guarantees != null ? new List<GacchaGuarantee<TCapsule>>(guarantees) : new List<GacchaGuarantee<TCapsule>>();
            BasicPonCount = basicPonCount > 0 ? basicPonCount : 0;
        }

        public int GetTotalPonCount() => Guarantees.Sum(g => g.Quantity) + BasicPonCount;
        public bool GacchaHasEnoughCapsules(Gaccha<TCapsule> gaccha) => GetTotalPonCount() <= gaccha.GetTotalCapsuleCount();

        /// <summary>
        /// Try to pon capsules from the gaccha to fulfill provided requirements. Return true if the correct number of pons was returned, regardless of predicate chain success.
        /// </summary>
        public bool TryPonAll(Gaccha<TCapsule> gaccha, bool withReplacement, out IEnumerable<TCapsule> capsules)
        {
            if (gaccha == null)
            {
                throw new ArgumentNullException(nameof(gaccha));
            }

            var capsuleList = new List<TCapsule>();
            int adjustedBasicPonCount = BasicPonCount;

            if (gaccha.IsEmpty)
            {
                capsules = capsuleList;
                return false;
            }

            // Do guarantees.
            foreach (var guarantee in Guarantees)
            {
                // Drop return value - we're using count.
                if (gaccha.TryPonMany(guarantee.PredicateChain, guarantee.Quantity, withReplacement, out IEnumerable<TCapsule> foundForGuarantee))
                {
                    capsuleList.AddRange(foundForGuarantee);
                }

                // If there were unfulfilled guarantees, add that number to the number of basic pons.
                adjustedBasicPonCount += guarantee.Quantity - foundForGuarantee.Count();
            }

            // Do basic pons (drop return value - we're checking returned count directly).
            gaccha.TryPonMany((Predicate<TCapsule>)null, adjustedBasicPonCount, withReplacement, out IEnumerable<TCapsule> basicCapsules);
            capsuleList.AddRange(basicCapsules);

            capsules = capsuleList;
            return capsuleList.Count == GetTotalPonCount();
        }
    }
}
