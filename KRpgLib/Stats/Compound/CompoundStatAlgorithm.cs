using System.Collections.Generic;

namespace KRpgLib.Stats.Compound
{
    public sealed class CompoundStatAlgorithm<TValue> where TValue : struct
    {
        private readonly List<IAlgorithmStep<TValue>> _steps;
        private readonly IExpression<TValue> _initExp;

        public CompoundStatAlgorithm(params IAlgorithmStep<TValue>[] algorithmSteps)
        {
            foreach (var step in algorithmSteps)
            {
                if (step == null)
                {
                    throw new System.ArgumentNullException(nameof(step), "No algorithm steps may be null when creating a new compound stat algorithm.");
                }
            }

            _steps = new List<IAlgorithmStep<TValue>>(algorithmSteps);
        }
        public CompoundStatAlgorithm(IExpression<TValue> valueInitializer, params IAlgorithmStep<TValue>[] algorithmSteps)
            :this(algorithmSteps)
        {
            _initExp = valueInitializer ?? throw new System.ArgumentNullException(nameof(valueInitializer));
        }
        public TValue CalculateValue(IStatSet<TValue> statSet)
        {
            if (statSet == null)
            {
                throw new System.ArgumentNullException(nameof(statSet));
            }

            // If initExp is not null, evaluate and set input value. If null, use default value of backing type.
            TValue rawValue = (_initExp?.Evaluate(statSet)) ?? default;

            foreach (var step in _steps)
            {
                rawValue = step.Apply(statSet, rawValue);
            }
            return rawValue;
        }
    }
}
