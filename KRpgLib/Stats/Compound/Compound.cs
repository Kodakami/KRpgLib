using System.Collections.Generic;

namespace KRpgLib.Stats.Compound
{
    public interface IExpression<TValue> where TValue : struct
    {
        TValue Evaluate(IStatSet<TValue> forStatSet);
    }
    public interface ILogicalExpression<TValue> where TValue : struct
    {
        bool Evaluate(IStatSet<TValue> forStatSet);
    }
    public abstract class AbstractExpression<TValue> : IExpression<TValue> where TValue : struct
    {
        public TValue Evaluate(IStatSet<TValue> forStatSet)
        {
            if (forStatSet == null)
            {
                throw new System.ArgumentNullException(nameof(forStatSet));
            }
            return Evaluate_Internal(forStatSet);
        }
        protected abstract TValue Evaluate_Internal(IStatSet<TValue> safeStatSet);
    }
    public class Literal<TValue> : AbstractExpression<TValue> where TValue : struct
    {
        private readonly TValue _literal;
        public Literal(TValue literal)
        {
            _literal = literal;
        }
        protected override TValue Evaluate_Internal(IStatSet<TValue> safeStatSet)
        {
            return _literal;
        }
    }
    public class StatLiteral<TValue> : AbstractExpression<TValue> where TValue : struct
    {
        protected readonly IStatTemplate<TValue> _template;
        protected readonly bool _useLegalizedValue;
        public StatLiteral(IStatTemplate<TValue> template, bool useLegalizedValue)
        {
            _template = template ?? throw new System.ArgumentNullException(nameof(template));
            _useLegalizedValue = useLegalizedValue;
        }
        protected override TValue Evaluate_Internal(IStatSet<TValue> safeStatSet)
        {
            TValue raw = safeStatSet.GetStatValue(_template);
            return _useLegalizedValue ? _template.GetLegalizedValue(raw) : raw;
        }
    }
    public class UnaryOperationType<TValue> where TValue : struct
    {
        private readonly System.Func<TValue, TValue> _unaryFunc;
        public UnaryOperationType(System.Func<TValue, TValue> unaryFunc)
        {
            _unaryFunc = unaryFunc ?? throw new System.ArgumentNullException(nameof(unaryFunc));
        }
        public TValue Evaluate(TValue input)
        {
            return _unaryFunc(input);
        }
    }
    public class BinaryOperationType<TValue> where TValue : struct
    {
        private readonly System.Func<TValue, TValue, TValue> _binaryFunc;
        public BinaryOperationType(System.Func<TValue, TValue, TValue> binaryFunc)
        {
            _binaryFunc = binaryFunc ?? throw new System.ArgumentNullException(nameof(binaryFunc));
        }
        public TValue Evaluate(TValue left, TValue right)
        {
            return _binaryFunc(left, right);
        }
    }
    public class Operation_Unary<TValue> : AbstractExpression<TValue> where TValue : struct
    {
        private readonly UnaryOperationType<TValue> _operationType;
        private readonly IExpression<TValue> _expression;

        public Operation_Unary(UnaryOperationType<TValue> operationType, IExpression<TValue> expression)
        {
            _operationType = operationType ?? throw new System.ArgumentNullException(nameof(operationType));
            _expression = expression ?? throw new System.ArgumentNullException(nameof(expression));
        }
        protected override TValue Evaluate_Internal(IStatSet<TValue> safeStatSet)
        {
            TValue expResult = _expression.Evaluate(safeStatSet);
            return _operationType.Evaluate(expResult);
        }
    }
    public abstract class AbstractOperation_Binary<TValue> : AbstractExpression<TValue> where TValue : struct
    {
        protected readonly BinaryOperationType<TValue> _operationType;

        protected AbstractOperation_Binary(BinaryOperationType<TValue> operationType)
        {
            _operationType = operationType ?? throw new System.ArgumentNullException(nameof(operationType));
        }
    }
    public class Operation_BinaryAssociative<TValue> : AbstractOperation_Binary<TValue> where TValue : struct
    {
        private readonly IExpression<TValue>[] _expressions;
        public Operation_BinaryAssociative(BinaryOperationType<TValue> operationType, params IExpression<TValue>[] expressionParams)
            :base(operationType)
        {
            if (expressionParams == null)
            {
                throw new System.ArgumentNullException(nameof(expressionParams));
            }
            if (expressionParams.Length == 0)
            {
                throw new System.ArgumentException("Argument array must not be empty.", nameof(expressionParams));
            }
            foreach (var expression in expressionParams)
            {
                if (expression == null)
                {
                    throw new System.ArgumentException("Argument array must not contain null items.", nameof(expressionParams));
                }
            }
            _expressions = expressionParams;
        }
        protected override TValue Evaluate_Internal(IStatSet<TValue> safeStatSet)
        {
            TValue total = default;
            foreach (var expression in _expressions)
            {
                total = _operationType.Evaluate(total, expression.Evaluate(safeStatSet));
            }
            return total;
        }
    }
    public class Operation_BinaryNonAssociative<TValue> : AbstractOperation_Binary<TValue> where TValue : struct
    {
        private readonly IExpression<TValue> _lh, _rh;
        public Operation_BinaryNonAssociative(BinaryOperationType<TValue> operationType, IExpression<TValue> leftHandExpression, IExpression<TValue> rightHandExpression)
            : base(operationType)
        {
            _lh = leftHandExpression ?? throw new System.ArgumentNullException(nameof(leftHandExpression));
            _rh = rightHandExpression ?? throw new System.ArgumentNullException(nameof(rightHandExpression));
        }
        protected override TValue Evaluate_Internal(IStatSet<TValue> safeStatSet)
        {
            return _operationType.Evaluate(_lh.Evaluate(safeStatSet), _rh.Evaluate(safeStatSet));
        }
    }
    public class Condition<TValue> : ILogicalExpression<TValue> where TValue : struct
    {
        private readonly IExpression<TValue> _lh, _rh;
        private readonly ComparisonType<TValue> _type;

        public Condition(IExpression<TValue> leftHandExpression, ComparisonType<TValue> comparisonType, IExpression<TValue> rightHandExpression)
        {
            _lh = leftHandExpression;
            _type = comparisonType;
            _rh = rightHandExpression;
        }
        public bool Evaluate(IStatSet<TValue> forStatSet)
        {
            TValue exp1Result = _lh.Evaluate(forStatSet);
            TValue exp2Result = _rh.Evaluate(forStatSet);

            return _type.Evaluate(exp1Result, exp2Result);
        }
    }
    public sealed class LogicalOperationType<TValue> where TValue : struct
    {
        public delegate bool FunctionDelegate(ILogicalExpression<TValue> left, ILogicalExpression<TValue> right, IStatSet<TValue> forStatSet);
        private readonly FunctionDelegate _func;
        public LogicalOperationType(FunctionDelegate func)
        {
            _func = func ?? throw new System.ArgumentNullException(nameof(func));
        }
        public bool Evaluate(ILogicalExpression<TValue> leftHand, ILogicalExpression<TValue> rightHand, IStatSet<TValue> forStatSet)
        {
            // Does not check for null, so you'd better have checked in calling code.
            return _func(leftHand, rightHand, forStatSet);
        }
    }
    public class LogicalOperation_Binary<TValue> : ILogicalExpression<TValue> where TValue : struct
    {
        private readonly LogicalOperationType<TValue> _op;
        private readonly ILogicalExpression<TValue> _lh, _rh;
        public LogicalOperation_Binary(ILogicalExpression<TValue> leftHandExpression, LogicalOperationType<TValue> operationType, ILogicalExpression<TValue> rightHandExpression)
        {
            _op = operationType ?? throw new System.ArgumentNullException(nameof(operationType));
            _lh = leftHandExpression ?? throw new System.ArgumentNullException(nameof(leftHandExpression));
            _rh = rightHandExpression ?? throw new System.ArgumentNullException(nameof(rightHandExpression));
        }

        public bool Evaluate(IStatSet<TValue> forStatSet)
        {
            return _op.Evaluate(_lh, _rh, forStatSet ?? throw new System.ArgumentNullException(nameof(forStatSet)));
        }
    }
    public class ComparisonType<TValue> where TValue : struct
    {
        private readonly System.Func<TValue, TValue, bool> _func;
        public ComparisonType(System.Func<TValue, TValue, bool> comparisonFunc)
        {
            _func = comparisonFunc;
        }
        public bool Evaluate(TValue value1, TValue value2)
        {
            return _func(value1, value2);
        }
    }

    // TODO: Make Legalize a type of expression.
    public class ConditionalExpression<TValue> : AbstractExpression<TValue> where TValue : struct
    {
        private readonly ILogicalExpression<TValue> _condition;
        private readonly IExpression<TValue> _consequent, _alternative;
        public ConditionalExpression(ILogicalExpression<TValue> condition, IExpression<TValue> consequentExpression, IExpression<TValue> alternativeExpression)
        {
            _condition = condition ?? throw new System.ArgumentNullException(nameof(condition));
            _consequent = consequentExpression ?? throw new System.ArgumentNullException(nameof(consequentExpression));
            _alternative = alternativeExpression ?? throw new System.ArgumentNullException(nameof(alternativeExpression));
        }
        protected override TValue Evaluate_Internal(IStatSet<TValue> safeStatSet)
        {
            return _condition.Evaluate(safeStatSet) ? _consequent.Evaluate(safeStatSet) : _alternative.Evaluate(safeStatSet);
        }
    }
}
