namespace KRpgLib.Stats.Compound
{
    /// <summary>
    /// An encapsulated expression tree used for determining the value of a compound stat when given a stat set. This may be constructed directly or created using an AlgoBuilder.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class CompoundStatAlgorithm<TValue> where TValue : struct
    {
        // The root tree node.
        private readonly ValueExpression<TValue> _expression;

        /// <summary>
        /// Create a new compound stat given the root node of an expresion tree.
        /// </summary>
        /// <param name="expression">a tree of expression objects that, when evaluated, results in a value of the stat backing type</param>
        public CompoundStatAlgorithm(ValueExpression<TValue> expression)
        {
            _expression = expression ?? throw new System.ArgumentNullException(nameof(expression));
        }
        /// <summary>
        /// Evaluates the algorithm in the context of the provided stat set.
        /// </summary>
        /// <param name="statSet">a stat set to provide stat values to the algorithm</param>
        /// <returns>the result of the algorithm in the context of the provided stat set (raw compound stat value)</returns>
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
