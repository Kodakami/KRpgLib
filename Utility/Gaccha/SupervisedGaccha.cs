using System;
using System.Collections.Generic;

namespace KRpgLib.Utility.Gaccha
{
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
