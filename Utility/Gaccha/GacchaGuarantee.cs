using System;
using System.Collections.Generic;

namespace KRpgLib.Utility.Gaccha
{
    public class GacchaGuarantee<TCapsule>
    {
        public PredicateChain<TCapsule> PredicateChain { get; }
        public int Quantity { get; }

        public GacchaGuarantee(PredicateChain<TCapsule> predicateChain, int quantity)
        {
            PredicateChain = predicateChain ?? throw new ArgumentNullException(nameof(predicateChain));
            Quantity = quantity > 0 ? quantity : 0;
        }
        public GacchaGuarantee(IEnumerable<Predicate<TCapsule>> predicates, int quantity)
            : this(new PredicateChain<TCapsule>(predicates), quantity) { }
        public GacchaGuarantee(Predicate<TCapsule> predicate, int quantity)
            : this(new PredicateChain<TCapsule>(predicate), quantity) { }
    }
    public class GacchaGuarantee<TCapsule, TPredicateChain> : GacchaGuarantee<TCapsule>
        where TPredicateChain : PredicateChain<TCapsule>
    {
        new public TPredicateChain PredicateChain => (TPredicateChain)base.PredicateChain;
        public GacchaGuarantee(TPredicateChain predicateChain, int quantity) : base(predicateChain, quantity) { }
    }
}
