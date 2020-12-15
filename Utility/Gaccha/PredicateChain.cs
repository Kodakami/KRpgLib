using System;
using System.Collections;
using System.Collections.Generic;

namespace KRpgLib.Utility.Gaccha
{
    /// <summary>
    /// Predicates used to locate a valid capsule for some purpose. If the first predicate fails, the next will take over until the end of the chain, at which point the gaccha should attempt to return any capsule (predicate becomes "x => true").
    /// </summary>
    public class PredicateChain<TCapsule> : IEnumerable<Predicate<TCapsule>>
    {
        private readonly List<Predicate<TCapsule>> _chain;

        public PredicateChain(IEnumerable<Predicate<TCapsule>> predicates)
        {
            // Null predicates are acceptable.

            _chain = new List<Predicate<TCapsule>>(predicates ?? throw new ArgumentNullException(nameof(predicates)));
        }
        public PredicateChain(params Predicate<TCapsule>[] predicates)
        {
            // Null predicates are acceptable.

            _chain = new List<Predicate<TCapsule>>(predicates ?? throw new ArgumentNullException(nameof(predicates)));
        }

        public IEnumerator<Predicate<TCapsule>> GetEnumerator()
        {
            return ((IEnumerable<Predicate<TCapsule>>)_chain).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_chain).GetEnumerator();
        }
    }
}
