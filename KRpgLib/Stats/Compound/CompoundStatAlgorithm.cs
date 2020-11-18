namespace KRpgLib.Stats.Compound
{
    public sealed class CompoundStatAlgorithm<TValue> where TValue : struct
    {
        private readonly ValueExpression<TValue> _expression;

        public CompoundStatAlgorithm(ValueExpression<TValue> expression)
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
