using System.Collections.Generic;

namespace KRpgLib.Stats.Compound
{
    public interface IExpression<TValue> where TValue : struct
    {
        TValue Evaluate(IStatSet<TValue> forStatSet);
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
        private readonly bool _useLegalizedValue;
        public StatLiteral(IStatTemplate<TValue> template, bool useLegalizedValue)
        {
            _template = template ?? throw new System.ArgumentNullException(nameof(template));
            _useLegalizedValue = useLegalizedValue;
        }
        protected override TValue Evaluate_Internal(IStatSet<TValue> safeStatSet)
        {
            TValue rawValue = safeStatSet.GetStatValue(_template);

            if (_useLegalizedValue)
            {
                return _template.GetLegalizedValue(rawValue);
            }

            return rawValue;
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
        private readonly IExpression<TValue> _expression;
        private readonly UnaryOperationType<TValue> _operationType;

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
    public class Operation_Binary<TValue> : AbstractExpression<TValue> where TValue : struct
    {
        private readonly IExpression<TValue> _lh, _rh;
        private readonly BinaryOperationType<TValue> _operationType;

        public Operation_Binary(IExpression<TValue> leftHandExpression, BinaryOperationType<TValue> operationType, IExpression<TValue> rightHandExpression)
        {
            _operationType = operationType ?? throw new System.ArgumentNullException(nameof(operationType));
            _lh = leftHandExpression ?? throw new System.ArgumentNullException(nameof(leftHandExpression));
            _rh = rightHandExpression ?? throw new System.ArgumentNullException(nameof(rightHandExpression));
        }
        protected override TValue Evaluate_Internal(IStatSet<TValue> safeStatSet)
        {
            TValue exp1Result = _lh.Evaluate(safeStatSet);
            TValue exp2Result = _rh.Evaluate(safeStatSet);

            return _operationType.Evaluate(exp1Result, exp2Result);
        }
    }
    public class Condition<TValue> where TValue : struct
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

    // TODO: Make boolean expressions inside comparisons.
    // TODO: Make Legalize a type of expression.
    public class ConditionalExpression<TValue> : AbstractExpression<TValue> where TValue : struct
    {
        private readonly Condition<TValue> _condition;
        private readonly IExpression<TValue> _consequent, _alternative;
        public ConditionalExpression(Condition<TValue> condition, IExpression<TValue> consequentExpression, IExpression<TValue> alternativeExpression)
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
