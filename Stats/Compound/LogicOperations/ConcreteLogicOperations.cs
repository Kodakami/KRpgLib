using System.Collections.Generic;

namespace KRpgLib.Stats.Compound.LogicOperations
{
    /// <summary>
    /// An expression object representing a unary logical NOT operation.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class LogicOperation_Not<TValue> : LogicOperation_Unary<TValue> where TValue : struct
    {
        public LogicOperation_Not(LogicExpression<TValue> input)
            : base(input) { }

        protected override bool Evaluate_Internal(IStatSet<TValue> safeStatSet)
        {
            // NOT.
            return !_input.Evaluate(safeStatSet);
        }
    }
    /// <summary>
    /// An expression object representing a binary logical AND operation. Performs short-circuit calculations.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class LogicOperation_And<TValue> : LogicOperation_Binary<TValue> where TValue : struct
    {
        public LogicOperation_And(LogicExpression<TValue> left, LogicExpression<TValue> right)
            : base(left, right) { }

        protected override bool Evaluate_Internal(IStatSet<TValue> safeStatSet)
        {
            // Short-circuit AND.
            return _left.Evaluate(safeStatSet) && _right.Evaluate(safeStatSet);
        }
    }
    /// <summary>
    /// An expression object representing a binary logical OR operation. Performs short-circuit calculations.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class LogicOperation_Or<TValue> : LogicOperation_Binary<TValue> where TValue : struct
    {
        public LogicOperation_Or(LogicExpression<TValue> left, LogicExpression<TValue> right)
            : base(left, right) { }

        protected override bool Evaluate_Internal(IStatSet<TValue> safeStatSet)
        {
            // Short-circuit OR.
            return _left.Evaluate(safeStatSet) || _right.Evaluate(safeStatSet);
        }
    }
    /// <summary>
    /// An expression object representing a binary logical XOR operation.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class LogicOperation_Xor<TValue> : LogicOperation_Binary<TValue> where TValue : struct
    {
        public LogicOperation_Xor(LogicExpression<TValue> left, LogicExpression<TValue> right)
            : base(left, right) { }

        protected override bool Evaluate_Internal(IStatSet<TValue> safeStatSet)
        {
            // XOR.
            return _left.Evaluate(safeStatSet) ^ _right.Evaluate(safeStatSet);
        }
    }
    /// <summary>
    /// An expression object representing a multiary logical ALL operation. Returns true if all values are true. Performs short-circuit calculations.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class LogicOperation_All<TValue> : LogicOperation_Multiary<TValue> where TValue : struct
    {
        public LogicOperation_All(List<LogicExpression<TValue>> logicExpressions)
            : base(logicExpressions) { }

        public LogicOperation_All(params LogicExpression<TValue>[] logicExpressions)
            : base(logicExpressions) { }

        protected override bool Evaluate_Internal(IStatSet<TValue> safeStatSet)
        {
            // Multiary AND. All values are true.
            // System.Linq's "TrueForAll" method does the same thing with the same short-circuit behavior.

            foreach (var expression in _logicExpressions)
            {
                // If the expression evaluates to false, then not all are true. Return false.
                if (!expression.Evaluate(safeStatSet))
                {
                    return false;
                }
            }
            return true;
        }
    }
    /// <summary>
    /// An expression object representing a multiary logical ANY operation. Returns true if any of the values are true. Performs short-circuit calculations.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class LogicOperation_Any<TValue> : LogicOperation_Multiary<TValue> where TValue : struct
    {
        public LogicOperation_Any(List<LogicExpression<TValue>> logicExpressions)
            : base(logicExpressions) { }

        public LogicOperation_Any(params LogicExpression<TValue>[] logicExpressions)
            : base(logicExpressions) { }

        protected override bool Evaluate_Internal(IStatSet<TValue> safeStatSet)
        {
            // Multiary OR. At least one value is true.
            // System.Linq's "Any" method does the same thing with the same short-circuit behavior.

            foreach (var expression in _logicExpressions)
            {
                // If the expression evaluates to true, then at least one is true. Return true.
                if (expression.Evaluate(safeStatSet))
                {
                    return true;
                }
            }
            return false;
        }
    }
    /// <summary>
    /// An expression object representing a multiary logical ONE operation. Returns true if one and only one value is true. Performs short-circuit calculations.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class LogicOperation_One<TValue> : LogicOperation_Multiary<TValue> where TValue : struct
    {
        public LogicOperation_One(List<LogicExpression<TValue>> logicExpressions)
            : base(logicExpressions) { }

        public LogicOperation_One(params LogicExpression<TValue>[] logicExpressions)
            : base(logicExpressions) { }

        protected override bool Evaluate_Internal(IStatSet<TValue> safeStatSet)
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
