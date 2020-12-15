using System;
using System.Collections.Generic;
namespace KRpgLib.Utility.Gaccha
{
    public class GacchaBanner<TCapsule>
    {
        protected Gaccha<TCapsule> Gaccha { get; }
        protected bool ReplaceCapsulesAfterPon { get; }     // "withReplacement"

        protected GacchaBanner(Gaccha<TCapsule> gaccha, bool replaceCapsulesAfterPon)
        {
            Gaccha = gaccha ?? throw new ArgumentNullException(nameof(gaccha));
            ReplaceCapsulesAfterPon = replaceCapsulesAfterPon;
        }
        public bool TrySinglePull(out TCapsule capsule)
        {
            return Gaccha.TryPonSingle((Predicate<TCapsule>)null, ReplaceCapsulesAfterPon, out capsule);
        }

        public static GacchaBanner<TCapsule> CreateBasicGacchaBanner(
            IDictionary<TCapsule, int> poolDictionary,
            bool replaceCapsulesAfterPon)
        {
            var pool = new GacchaPool<TCapsule>(poolDictionary);
            var basicGaccha = new Gaccha<TCapsule>(pool);

            return new GacchaBanner<TCapsule>(basicGaccha, replaceCapsulesAfterPon);
        }
        public static GacchaBanner<TCapsule> CreateSupervisedGacchaBanner(
            IDictionary<TCapsule, int> poolDictionary,
            IEnumerable<GacchaLifeguard<TCapsule>> lifeguards,
            GacchaLifeguardChain<TCapsule>.ResetOption onLifeguardProc,
            bool replaceCapsulesAfterPon)
        {
            var lifeguardChain = new GacchaLifeguardChain<TCapsule>(lifeguards, onLifeguardProc);
            var pool = new GacchaPool<TCapsule>(poolDictionary);
            var supervisedGaccha = new SupervisedGaccha<TCapsule>(pool, lifeguardChain);

            return new GacchaBanner<TCapsule>(supervisedGaccha, replaceCapsulesAfterPon);
        }
    }
}
