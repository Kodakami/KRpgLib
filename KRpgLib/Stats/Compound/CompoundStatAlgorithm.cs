using System.Collections.Generic;

namespace KRpgLib.Stats.Compound
{
    public sealed class CompoundStatAlgorithm
    {
        private readonly List<IAlgorithmStep> _steps;
        private readonly IExpression _initExp;

        public CompoundStatAlgorithm(params IAlgorithmStep[] algorithmSteps)
        {
            foreach (var step in algorithmSteps)
            {
                if (step == null)
                {
                    throw new System.ArgumentNullException(nameof(step), "No algorithm steps may be null when creating a new compound stat algorithm.");
                }
            }

            _steps = new List<IAlgorithmStep>(algorithmSteps);
        }
        public CompoundStatAlgorithm(IExpression valueInitializer, params IAlgorithmStep[] algorithmSteps)
            :this(algorithmSteps)
        {
            _initExp = valueInitializer ?? throw new System.ArgumentNullException(nameof(valueInitializer));
        }
        public float CalculateValue(IStatSet statSet)
        {
            if (statSet == null)
            {
                throw new System.ArgumentNullException(nameof(statSet));
            }

            float rawValue = _initExp == null ? 0 : _initExp.Evaluate(statSet);
            foreach (var step in _steps)
            {
                rawValue = step.Apply(statSet, rawValue);
            }
            return rawValue;
        }
    }
}
