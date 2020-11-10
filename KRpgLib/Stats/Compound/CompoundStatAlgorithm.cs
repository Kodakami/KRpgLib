using System.Collections.Generic;

namespace KRpgLib.Stats.Compound
{
    public sealed class CompoundStatAlgorithm<TValue> where TValue : struct
    {
        private readonly IExpression<TValue> _expression;

        public CompoundStatAlgorithm(IExpression<TValue> expression)
        {
            _expression = expression ?? throw new System.ArgumentNullException(nameof(expression));
        }
        public TValue CalculateValue(IStatSet<TValue> statSet)
        {
            if (statSet == null)
            {
                throw new System.ArgumentNullException(nameof(statSet));
            }

            return _expression.Evaluate(statSet);
        }
    }
}
