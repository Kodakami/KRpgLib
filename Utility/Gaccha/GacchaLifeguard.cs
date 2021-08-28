using System;
using System.Collections.Generic;
using KRpgLib.Utility.ValueCurves;

namespace KRpgLib.Utility.Gaccha
{
    public class GacchaLifeguard<TCapsule>
    {
        public PredicateChain<TCapsule> PredicateChain { get; }
        public ValueCurve<double> ProbabilityCurve { get; }

        public int PonsObserved { get; private set; }
        public double CurrentProbability => ProbabilityCurve.GetY(PonsObserved);

        public GacchaLifeguard(PredicateChain<TCapsule> predicateChain, ValueCurve<double> probabilityCurve)
        {
            PredicateChain = predicateChain ?? throw new ArgumentNullException(nameof(predicateChain));

            // ValueCurve is a struct, so an empty probability curve will result in every value being 0 (never triggering).
            ProbabilityCurve = probabilityCurve;
        }
        public void ObservePons(int count) => SetPonsObserved(PonsObserved + count);    // Negative values are silently accepted.
        public void ResetPonsObserved() => SetPonsObserved(0);
        protected void SetPonsObserved(int value)
        {
            PonsObserved = Math.Max(value, 0);
        }

        public bool RollForProc()
        {
            return Environment.Rng.NextDouble() < CurrentProbability;
        }
    }
    public class GacchaLifeguard<TCapsule, TPredicateChain> : GacchaLifeguard<TCapsule>
        where TPredicateChain : PredicateChain<TCapsule>
    {
        new public TPredicateChain PredicateChain => (TPredicateChain)base.PredicateChain;
        public GacchaLifeguard(TPredicateChain predicateChain, ValueCurve<double> probabilityCurve)
            :base(predicateChain, probabilityCurve) { }
    }
    public class GacchaLifeguardChain<TCapsule>
    {
        private readonly List<GacchaLifeguard<TCapsule>> _chain;

        public ResetOption OnProc { get; }

        // Regular ctor.
        public GacchaLifeguardChain(IEnumerable<GacchaLifeguard<TCapsule>> chain, ResetOption onProc)
        {
            _chain = new List<GacchaLifeguard<TCapsule>>(chain ?? throw new ArgumentNullException(nameof(chain)));
            if (_chain.Contains(null))
            {
                throw new ArgumentException("Collection may not contain null items.", nameof(chain));
            }

            OnProc = onProc;
        }

        public IEnumerable<int> GetPonsObserved() => _chain.ConvertAll(lg => lg.PonsObserved);
        public IEnumerable<double> GetCurrentProbabilities() => _chain.ConvertAll(lg => lg.CurrentProbability);
        public void ObservePons(int count)
        {
            _chain.ForEach(lg => lg.ObservePons(count));
        }

        public void ResetPonsObserved() => _chain.ForEach(lg => lg.ResetPonsObserved());
        protected bool RollSequentiallyForProc(out GacchaLifeguard<TCapsule> proc)
        {
            foreach (var lg in _chain)
            {
                if (lg.RollForProc())
                {
                    proc = lg;
                    return true;
                }
            }
            proc = null;
            return false;
        }
        public bool RollForProcAndReturnCapsuleIfSuccessful(Gaccha<TCapsule> gaccha, out TCapsule capsule)
        {
            capsule = default;

            if (gaccha == null)
            {
                return false;
            }

            if (RollSequentiallyForProc(out GacchaLifeguard<TCapsule> proc)
                && gaccha.TryPonSingle(proc.PredicateChain, withReplacement: true, out capsule))
            {
                // Trigger resets if applicable.
                if (OnProc == ResetOption.RESET_ALL_LIFEGUARDS_ON_PROC)
                {
                    ResetPonsObserved();
                }
                else if (OnProc == ResetOption.RESET_LIFEGUARD_ON_PROC)
                {
                    proc.ResetPonsObserved();
                }

                return true;
            }

            return false;
        }
        public enum ResetOption
        {
            RESET_ALL_LIFEGUARDS_ON_PROC = 0,
            RESET_LIFEGUARD_ON_PROC = 1,
            DO_NOT_RESET_ON_PROC = 2,
        }
    }
    public class GacchaLifeguardChain<TCapsule, TLifeguard> : GacchaLifeguardChain<TCapsule>
        where TLifeguard : GacchaLifeguard<TCapsule>
    {
        public GacchaLifeguardChain(IEnumerable<TLifeguard> chain, ResetOption onProc) : base(chain, onProc) { }
    }
}
