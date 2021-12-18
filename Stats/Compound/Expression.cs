using System.Collections.Generic;
using System.Linq;

namespace KRpgLib.Stats.Compound
{
    /// <summary>
    /// Base class for expression objects.
    /// </summary>
    /// <typeparam name="TReturn">return type for Evaluate(). "bool" for logic expressions, "int" for value expressions.</typeparam>
    public abstract class Expression<TValue, TReturn>
    {
        public TReturn Evaluate(IStatSet forStatSet)
        {
            if (forStatSet == null)
            {
                throw new System.ArgumentNullException(nameof(forStatSet));
            }
            return Evaluate_Internal(forStatSet);
        }
        protected abstract TReturn Evaluate_Internal(IStatSet safeStatSet);
    }
    /// <summary>
    /// Abstract expression object representing an integer value.
    /// </summary>
    public abstract class ValueExpression : Expression<int, int> { }
    /// <summary>
    /// Abstract expression object representing a logical (boolean) value.
    /// </summary>
    public abstract class LogicExpression : Expression<int, bool> { }
    /// <summary>
    /// Expression object representing a literal integer value.
    /// </summary>
    public sealed class Literal : ValueExpression
    {
        private readonly int _literal;
        public Literal(int literal)
        {
            _literal = literal;
        }
        protected override int Evaluate_Internal(IStatSet safeStatSet)
        {
            return _literal;
        }
    }
    /// <summary>
    /// Expression object representing the current value of a stat. May be a raw or legalized value.
    /// </summary>
    public sealed class StatLiteral : ValueExpression
    {
        private readonly Stat _stat;
        private readonly bool _useLegalizedValue;
        public StatLiteral(Stat stat, bool useLegalizedValue)
        {
            _stat = stat ?? throw new System.ArgumentNullException(nameof(stat));
            _useLegalizedValue = useLegalizedValue;
        }
        protected override int Evaluate_Internal(IStatSet safeStatSet)
        {
            int raw = safeStatSet.GetStatValue(_stat);
            return _useLegalizedValue ? _stat.GetLegalizedValue(raw) : raw;
        }
    }
    /// <summary>
    /// Delegate function for operations on a value that take only one input.
    /// </summary>
    /// <param name="input">input value</param>
    /// <returns>the new value after performing the operation</returns>
    public delegate int UnaryFunc(int input);
    /// <summary>
    /// Encapsulated unary operation function.
    /// </summary>
    public sealed class UnaryOperationType
    {
        private readonly UnaryFunc _unaryFunc;
        public UnaryOperationType(UnaryFunc unaryFunc)
        {
            _unaryFunc = unaryFunc ?? throw new System.ArgumentNullException(nameof(unaryFunc));
        }
        public int Evaluate(int input)
        {
            return _unaryFunc(input);
        }
    }
    /// <summary>
    /// Delegate function for operations on a value that take exactly two ordered inputs of the same type.
    /// </summary>
    /// <param name="left">left-hand value</param>
    /// /// <param name="right">right-hand value</param>
    /// <returns>the new value after performing the operation</returns>
    public delegate int BinaryFunc(int left, int right);
    /// <summary>
    /// Encapsulated binary operation function.
    /// </summary>
    public sealed class BinaryOperationType
    {
        private readonly BinaryFunc _binaryFunc;
        public BinaryOperationType(BinaryFunc binaryFunc)
        {
            _binaryFunc = binaryFunc ?? throw new System.ArgumentNullException(nameof(binaryFunc));
        }
        public int Evaluate(int left, int right)
        {
            return _binaryFunc(left, right);
        }
    }
    /// <summary>
    /// Delegate function for operations on a value that take an arbitrarily-large collection of unordered inputs of the same type.
    /// </summary>
    /// <param name="values">collection of input values</param>
    /// <returns>the new value after performing the operation</returns>
    public delegate int MultiaryFunc(IEnumerable<int> values);
    /// <summary>
    /// Encapsulated multiary operation function.
    /// </summary>
    public sealed class MultiaryOperationType
    {
        private readonly MultiaryFunc _multiaryFunc;

        public MultiaryOperationType(MultiaryFunc multiaryFunc)
        {
            _multiaryFunc = multiaryFunc ?? throw new System.ArgumentNullException(nameof(multiaryFunc));
        }
        public int Evaluate(IEnumerable<int> values)
        {
            return _multiaryFunc(values);
        }
    }
    /// <summary>
    /// Delegate function for expressions that compare two values of the same type.
    /// </summary>
    /// <param name="leftHand">left-hand value</param>
    /// <param name="rightHand">right-hand value</param>
    /// <returns>true if the comparison is true, otherwise false</returns>
    public delegate bool ComparisonFunc(int leftHand, int rightHand);
    /// <summary>
    /// Encapsulated comparison expression function.
    /// </summary>
    public sealed class ComparisonType
    {
        private readonly ComparisonFunc _func;
        public ComparisonType(ComparisonFunc comparisonFunc)
        {
            _func = comparisonFunc ?? throw new System.ArgumentNullException(nameof(comparisonFunc));
        }
        public bool Evaluate(int leftHand, int rightHand)
        {
            return _func(leftHand, rightHand);
        }
    }

    /// <summary>
    /// Expression object representing a value operation taking only one value as input.
    /// </summary>
    public sealed class ValueOperation_Unary : ValueExpression
    {
        private readonly UnaryOperationType _operationType;
        private readonly ValueExpression _input;

        public ValueOperation_Unary(UnaryOperationType operationType, ValueExpression input)
        {
            _operationType = operationType ?? throw new System.ArgumentNullException(nameof(operationType));
            _input = input ?? throw new System.ArgumentNullException(nameof(input));
        }
        protected override int Evaluate_Internal(IStatSet safeStatSet)
        {
            return _operationType.Evaluate(_input.Evaluate(safeStatSet));
        }
    }
    /// <summary>
    /// Expression object representing a value operation taking exactly two ordered values as input.
    /// </summary>
    public sealed class ValueOperation_Binary : ValueExpression
    {
        private readonly BinaryOperationType _operationType;
        private readonly ValueExpression _left, _right;
        public ValueOperation_Binary(BinaryOperationType operationType, ValueExpression left, ValueExpression right)
        {
            _operationType = operationType ?? throw new System.ArgumentNullException(nameof(operationType));
            _left = left ?? throw new System.ArgumentNullException(nameof(left));
            _right = right ?? throw new System.ArgumentNullException(nameof(right));
        }
        protected override int Evaluate_Internal(IStatSet safeStatSet)
        {
            return _operationType.Evaluate(_left.Evaluate(safeStatSet), _right.Evaluate(safeStatSet));
        }
    }
    /// <summary>
    /// Expression object representing a value operation taking an arbitrarily-large collection of unordered values as input.
    /// </summary>
    public sealed class ValueOperation_Multiary : ValueExpression
    {
        private readonly MultiaryOperationType _operationType;
        private readonly IEnumerable<ValueExpression> _valueExpressions;

        public ValueOperation_Multiary(MultiaryOperationType operationType, IEnumerable<ValueExpression> valueExpressions)
        {
            _operationType = operationType ?? throw new System.ArgumentNullException(nameof(operationType));
            if (valueExpressions == null)
            {
                throw new System.ArgumentNullException(nameof(valueExpressions));
            }
            foreach (var expression in valueExpressions)
            {
                if (expression == null)
                {
                    throw new System.ArgumentException("Argument array may not contain null items.", nameof(valueExpressions));
                }
            }
            _valueExpressions = valueExpressions;
        }
        public ValueOperation_Multiary(MultiaryOperationType operationType, params ValueExpression[] valueExpressions)
            : this(operationType, (IEnumerable<ValueExpression>)valueExpressions) { }

        protected override int Evaluate_Internal(IStatSet safeStatSet)
        {
            // Avoiding a call to System.Linq's "Select" method, I guess.

            return _operationType.Evaluate(_valueExpressions.Select(ve => ve.Evaluate(safeStatSet)));
        }
    }
    /// <summary>
    /// Expression object representing a logic operation taking only one value as input.
    /// </summary>
    public abstract class LogicOperation_Unary : LogicExpression
    {
        protected readonly LogicExpression _input;
        protected LogicOperation_Unary(LogicExpression input)
        {
            _input = input ?? throw new System.ArgumentNullException(nameof(input));
        }
    }
    /// <summary>
    /// Expression object representing a logic operation taking exactly two ordered values as input.
    /// </summary>
    public abstract class LogicOperation_Binary : LogicExpression
    {
        protected readonly LogicExpression _left, _right;
        protected LogicOperation_Binary(LogicExpression left, LogicExpression right)
        {
            _left = left ?? throw new System.ArgumentNullException(nameof(left));
            _right = right ?? throw new System.ArgumentNullException(nameof(right));
        }
    }
    /// <summary>
    /// Expression object representing a logic operation taking an arbitrarily-large list of unordered values as input.
    /// </summary>
    public abstract class LogicOperation_Multiary : LogicExpression
    {
        protected readonly IEnumerable<LogicExpression> _logicExpressions;
        protected LogicOperation_Multiary(IEnumerable<LogicExpression> logicExpressions)
        {
            if (logicExpressions == null)
            {
                throw new System.ArgumentNullException(nameof(logicExpressions));
            }
            foreach (var expression in logicExpressions)
            {
                if (expression == null)
                {
                    throw new System.ArgumentException("Argument array may not contain null items.", nameof(logicExpressions));
                }
            }
            _logicExpressions = logicExpressions;
        }
        protected LogicOperation_Multiary(params LogicExpression[] logicExpressions)
            : this((IEnumerable<LogicExpression>)logicExpressions) { }
    }

    /// <summary>
    /// Expression object representing a logical comparison with two ordered values as input.
    /// </summary>
    public sealed class Comparison : LogicExpression
    {
        private readonly ComparisonType _type;
        private readonly ValueExpression _lh, _rh;

        public Comparison(ComparisonType comparisonType, ValueExpression leftHandExpression, ValueExpression rightHandExpression)
        {
            _lh = leftHandExpression ?? throw new System.ArgumentNullException(nameof(leftHandExpression));
            _type = comparisonType ?? throw new System.ArgumentNullException(nameof(comparisonType));
            _rh = rightHandExpression ?? throw new System.ArgumentNullException(nameof(rightHandExpression));
        }

        protected override bool Evaluate_Internal(IStatSet safeStatSet)
        {
            int leftHandResult = _lh.Evaluate(safeStatSet);
            int rightHandResult = _rh.Evaluate(safeStatSet);

            return _type.Evaluate(leftHandResult, rightHandResult);
        }
    }
    /// <summary>
    /// Expression object representing a conditional expression. Inputs are a logical (boolean) value as a condition, a value if true, and an alternative value if false.
    /// </summary>
    public sealed class ConditionalExpression : ValueExpression
    {
        private readonly LogicExpression _condition;
        private readonly ValueExpression _consequent, _alternative;
        public ConditionalExpression(LogicExpression condition, ValueExpression consequentExpression, ValueExpression alternativeExpression)
        {
            _condition = condition ?? throw new System.ArgumentNullException(nameof(condition));
            _consequent = consequentExpression ?? throw new System.ArgumentNullException(nameof(consequentExpression));
            _alternative = alternativeExpression ?? throw new System.ArgumentNullException(nameof(alternativeExpression));
        }
        protected override int Evaluate_Internal(IStatSet safeStatSet)
        {
            return _condition.Evaluate(safeStatSet) ? _consequent.Evaluate(safeStatSet) : _alternative.Evaluate(safeStatSet);
        }
    }
}
