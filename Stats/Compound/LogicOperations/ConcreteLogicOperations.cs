using System.Collections.Generic;
using System.Linq;

namespace KRpgLib.Stats.Compound.LogicOperations
{
    /// <summary>
    /// An expression object representing a unary logical NOT operation.
    /// </summary>
    public sealed class LogicOperation_Not : LogicOperation_Unary
    {
        public LogicOperation_Not(LogicExpression input)
            : base(input) { }

        protected override bool Evaluate_Internal(IStatSet safeStatSet)
        {
            // NOT.
            return !_input.Evaluate(safeStatSet);
        }
    }
    /// <summary>
    /// An expression object representing a binary logical AND operation. Performs short-circuit calculations.
    /// </summary>
    public sealed class LogicOperation_And : LogicOperation_Binary
    {
        public LogicOperation_And(LogicExpression left, LogicExpression right)
            : base(left, right) { }

        protected override bool Evaluate_Internal(IStatSet safeStatSet)
        {
            // Short-circuit AND.
            return _left.Evaluate(safeStatSet) && _right.Evaluate(safeStatSet);
        }
    }
    /// <summary>
    /// An expression object representing a binary logical OR operation. Performs short-circuit calculations.
    /// </summary>
    public sealed class LogicOperation_Or : LogicOperation_Binary
    {
        public LogicOperation_Or(LogicExpression left, LogicExpression right)
            : base(left, right) { }

        protected override bool Evaluate_Internal(IStatSet safeStatSet)
        {
            // Short-circuit OR.
            return _left.Evaluate(safeStatSet) || _right.Evaluate(safeStatSet);
        }
    }
    /// <summary>
    /// An expression object representing a binary logical XOR operation.
    /// </summary>
    public sealed class LogicOperation_Xor : LogicOperation_Binary
    {
        public LogicOperation_Xor(LogicExpression left, LogicExpression right)
            : base(left, right) { }

        protected override bool Evaluate_Internal(IStatSet safeStatSet)
        {
            // XOR.
            return _left.Evaluate(safeStatSet) ^ _right.Evaluate(safeStatSet);
        }
    }
    /// <summary>
    /// An expression object representing a multiary logical ALL operation. Returns true if all values are true. Performs short-circuit calculations.
    /// </summary>
    public sealed class LogicOperation_All : LogicOperation_Multiary
    {
        public LogicOperation_All(IEnumerable<LogicExpression> logicExpressions)
            : base(logicExpressions) { }

        public LogicOperation_All(params LogicExpression[] logicExpressions)
            : base((IEnumerable<LogicExpression>)logicExpressions) { }

        protected override bool Evaluate_Internal(IStatSet safeStatSet)
        {
            // Multiary AND. All values are true.
            return _logicExpressions.All(exp => exp.Evaluate(safeStatSet));
        }
    }
    /// <summary>
    /// An expression object representing a multiary logical ANY operation. Returns true if any of the values are true. Performs short-circuit calculations.
    /// </summary>
    public sealed class LogicOperation_Any : LogicOperation_Multiary
    {
        public LogicOperation_Any(IEnumerable<LogicExpression> logicExpressions)
            : base(logicExpressions) { }

        public LogicOperation_Any(params LogicExpression[] logicExpressions)
            : base((IEnumerable<LogicExpression>)logicExpressions) { }

        protected override bool Evaluate_Internal(IStatSet safeStatSet)
        {
            // Multiary OR. At least one value is true.
            return _logicExpressions.Any(exp => exp.Evaluate(safeStatSet));
        }
    }
    /// <summary>
    /// An expression object representing a multiary logical ONE operation. Returns true if one and only one value is true. Performs short-circuit calculations.
    /// </summary>
    public sealed class LogicOperation_One : LogicOperation_Multiary
    {
        public LogicOperation_One(IEnumerable<LogicExpression> logicExpressions)
            : base(logicExpressions) { }

        public LogicOperation_One(params LogicExpression[] logicExpressions)
            : base((IEnumerable<LogicExpression>)logicExpressions) { }

        protected override bool Evaluate_Internal(IStatSet safeStatSet)
        {
            // My implementation of multiary XOR. One and only one is true.
            // Client code is free to introduce an odd-parity implementation if it wishes.

            bool oneIsTrue = false;
            foreach (var expression in _logicExpressions)
            {
                // If the expression evaluates to true..
                if (expression.Evaluate(safeStatSet))
                {
                    // If there is already a true expression, then return false since more than one is true.
                    if (oneIsTrue)
                    {
                        return false;
                    }
                    // Otherwise, flag that there is a true expression.
                    else
                    {
                        oneIsTrue = true;
                    }
                }
            }
            return oneIsTrue;
        }
    }
}
