﻿using System.Collections.Generic;

namespace KRpgLib.Stats.Compound
{
    /// <summary>
    /// Base class for expression objects.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    /// <typeparam name="TReturn">return type for Evaluate(). "bool" for logic expressions, "TValue" for value expressions.</typeparam>
    public abstract class Expression<TValue, TReturn> where TValue : struct //where TReturn : anything
    {
        public TReturn Evaluate(IStatSet<TValue> forStatSet)
        {
            if (forStatSet == null)
            {
                throw new System.ArgumentNullException(nameof(forStatSet));
            }
            return Evaluate_Internal(forStatSet);
        }
        protected abstract TReturn Evaluate_Internal(IStatSet<TValue> safeStatSet);
    }
    /// <summary>
    /// Abstract expression object representing a value of the stat backing type (usually a number).
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public abstract class ValueExpression<TValue> : Expression<TValue, TValue> where TValue : struct { }
    /// <summary>
    /// Abstract expression object representing a logical (boolean) value.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public abstract class LogicExpression<TValue> : Expression<TValue, bool> where TValue : struct { }
    /// <summary>
    /// Expression object representing a static value of the stat backing type (usually a number).
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class Literal<TValue> : ValueExpression<TValue> where TValue : struct
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
    /// <summary>
    /// Expression object representing the current value of a stat. May be a raw or legalized value.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class StatLiteral<TValue> : ValueExpression<TValue> where TValue : struct
    {
        private readonly IStat<TValue> _stat;
        private readonly bool _useLegalizedValue;
        public StatLiteral(IStat<TValue> stat, bool useLegalizedValue)
        {
            _stat = stat ?? throw new System.ArgumentNullException(nameof(stat));
            _useLegalizedValue = useLegalizedValue;
        }
        protected override TValue Evaluate_Internal(IStatSet<TValue> safeStatSet)
        {
            TValue raw = safeStatSet.GetStatValue(_stat);
            return _useLegalizedValue ? _stat.GetLegalizedValue(raw) : raw;
        }
    }
    /// <summary>
    /// Delegate function for operations on a value that take only one input.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    /// <param name="input">input value</param>
    /// <returns>the new value after performing the operation</returns>
    public delegate TValue UnaryFunc<TValue>(TValue input) where TValue : struct;
    /// <summary>
    /// Encapsulated unary operation function.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class UnaryOperationType<TValue> where TValue : struct
    {
        private readonly UnaryFunc<TValue> _unaryFunc;
        public UnaryOperationType(UnaryFunc<TValue> unaryFunc)
        {
            _unaryFunc = unaryFunc ?? throw new System.ArgumentNullException(nameof(unaryFunc));
        }
        public TValue Evaluate(TValue input)
        {
            return _unaryFunc(input);
        }
    }
    /// <summary>
    /// Delegate function for operations on a value that take exactly two ordered inputs of the same type.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    /// <param name="left">left-hand value</param>
    /// /// <param name="right">right-hand value</param>
    /// <returns>the new value after performing the operation</returns>
    public delegate TValue BinaryFunc<TValue>(TValue left, TValue right) where TValue : struct;
    /// <summary>
    /// Encapsulated binary operation function.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class BinaryOperationType<TValue> where TValue : struct
    {
        private readonly BinaryFunc<TValue> _binaryFunc;
        public BinaryOperationType(BinaryFunc<TValue> binaryFunc)
        {
            _binaryFunc = binaryFunc ?? throw new System.ArgumentNullException(nameof(binaryFunc));
        }
        public TValue Evaluate(TValue left, TValue right)
        {
            return _binaryFunc(left, right);
        }
    }
    /// <summary>
    /// Delegate function for operations on a value that take an arbitrarily-large list of unordered inputs of the same type.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    /// <param name="valueList">list of input values</param>
    /// <returns>the new value after performing the operation</returns>
    public delegate TValue MultiaryFunc<TValue>(List<TValue> valueList) where TValue : struct;
    /// <summary>
    /// Encapsulated multiary operation function.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class MultiaryOperationType<TValue> where TValue : struct
    {
        private readonly MultiaryFunc<TValue> _multiaryFunc;

        public MultiaryOperationType(MultiaryFunc<TValue> multiaryFunc)
        {
            _multiaryFunc = multiaryFunc ?? throw new System.ArgumentNullException(nameof(multiaryFunc));
        }
        public TValue Evaluate(List<TValue> safeValues)
        {
            return _multiaryFunc(safeValues);
        }
    }
    /// <summary>
    /// Delegate function for expressions that compare two values of the same type.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    /// <param name="leftHand">left-hand value</param>
    /// <param name="rightHand">right-hand value</param>
    /// <returns>true if the comparison is true, otherwise false</returns>
    public delegate bool ComparisonFunc<TValue>(TValue leftHand, TValue rightHand);
    /// <summary>
    /// Encapsulated comparison expression function.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class ComparisonType<TValue> where TValue : struct
    {
        private readonly ComparisonFunc<TValue> _func;
        public ComparisonType(ComparisonFunc<TValue> comparisonFunc)
        {
            _func = comparisonFunc ?? throw new System.ArgumentNullException(nameof(comparisonFunc));
        }
        public bool Evaluate(TValue value1, TValue value2)
        {
            return _func(value1, value2);
        }
    }

    /// <summary>
    /// Expression object representing a value operation taking only one value as input.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class ValueOperation_Unary<TValue> : ValueExpression<TValue> where TValue : struct
    {
        private readonly UnaryOperationType<TValue> _operationType;
        private readonly ValueExpression<TValue> _input;

        public ValueOperation_Unary(UnaryOperationType<TValue> operationType, ValueExpression<TValue> input)
        {
            _operationType = operationType ?? throw new System.ArgumentNullException(nameof(operationType));
            _input = input ?? throw new System.ArgumentNullException(nameof(input));
        }
        protected override TValue Evaluate_Internal(IStatSet<TValue> safeStatSet)
        {
            return _operationType.Evaluate(_input.Evaluate(safeStatSet));
        }
    }
    /// <summary>
    /// Expression object representing a value operation taking exactly two ordered values as input.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class ValueOperation_Binary<TValue> : ValueExpression<TValue> where TValue : struct
    {
        private readonly BinaryOperationType<TValue> _operationType;
        private readonly ValueExpression<TValue> _left, _right;
        public ValueOperation_Binary(BinaryOperationType<TValue> operationType, ValueExpression<TValue> left, ValueExpression<TValue> right)
        {
            _operationType = operationType ?? throw new System.ArgumentNullException(nameof(operationType));
            _left = left ?? throw new System.ArgumentNullException(nameof(left));
            _right = right ?? throw new System.ArgumentNullException(nameof(right));
        }
        protected override TValue Evaluate_Internal(IStatSet<TValue> safeStatSet)
        {
            return _operationType.Evaluate(_left.Evaluate(safeStatSet), _right.Evaluate(safeStatSet));
        }
    }
    /// <summary>
    /// Expression object representing a value operation taking an arbitrarily-large list of unordered values as input.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class ValueOperation_Multiary<TValue> : ValueExpression<TValue> where TValue : struct
    {
        private readonly MultiaryOperationType<TValue> _operationType;
        private readonly List<ValueExpression<TValue>> _valueExpressions;

        public ValueOperation_Multiary(MultiaryOperationType<TValue> operationType, List<ValueExpression<TValue>> valueExpressions)
        {
            _operationType = operationType ?? throw new System.ArgumentNullException(nameof(operationType));
            if (valueExpressions == null)
            {
                throw new System.ArgumentNullException(nameof(valueExpressions));
            }
            if (valueExpressions.Count == 0)
            {
                throw new System.ArgumentException("Argument may not be an empty array.", nameof(valueExpressions));
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
        public ValueOperation_Multiary(MultiaryOperationType<TValue> operationType, params ValueExpression<TValue>[] valueExpressions)
            : this(operationType, new List<ValueExpression<TValue>>(valueExpressions)) { }

        protected override TValue Evaluate_Internal(IStatSet<TValue> safeStatSet)
        {
            // Avoiding a call to System.Linq's "Select" method, I guess.

            List<TValue> expResults = new List<TValue>();
            foreach (var expression in _valueExpressions)
            {
                expResults.Add(expression.Evaluate(safeStatSet));
            }
            return _operationType.Evaluate(expResults);
        }
    }
    /// <summary>
    /// Expression object representing a logic operation taking only one value as input.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public abstract class LogicOperation_Unary<TValue> : LogicExpression<TValue> where TValue : struct
    {
        protected readonly LogicExpression<TValue> _input;
        protected LogicOperation_Unary(LogicExpression<TValue> input)
        {
            _input = input ?? throw new System.ArgumentNullException(nameof(input));
        }
    }
    /// <summary>
    /// Expression object representing a logic operation taking exactly two ordered values as input.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public abstract class LogicOperation_Binary<TValue> : LogicExpression<TValue> where TValue : struct
    {
        protected readonly LogicExpression<TValue> _left, _right;
        protected LogicOperation_Binary(LogicExpression<TValue> left, LogicExpression<TValue> right)
        {
            _left = left ?? throw new System.ArgumentNullException(nameof(left));
            _right = right ?? throw new System.ArgumentNullException(nameof(right));
        }
    }
    /// <summary>
    /// Expression object representing a logic operation taking an arbitrarily-large list of unordered values as input.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public abstract class LogicOperation_Multiary<TValue> : LogicExpression<TValue> where TValue : struct
    {
        protected readonly List<LogicExpression<TValue>> _logicExpressions;
        protected LogicOperation_Multiary(List<LogicExpression<TValue>> logicExpressions)
        {
            if (logicExpressions == null)
            {
                throw new System.ArgumentNullException(nameof(logicExpressions));
            }
            if (logicExpressions.Count == 0)
            {
                throw new System.ArgumentException("Argument may not be an empty array.", nameof(logicExpressions));
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
        protected LogicOperation_Multiary(params LogicExpression<TValue>[] logicExpressions)
            : this(new List<LogicExpression<TValue>>(logicExpressions)) { }
    }

    /// <summary>
    /// Expression object representing a logical comparison with two ordered values as input.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class Comparison<TValue> : LogicExpression<TValue> where TValue : struct
    {
        private readonly ComparisonType<TValue> _type;
        private readonly ValueExpression<TValue> _lh, _rh;

        public Comparison(ComparisonType<TValue> comparisonType, ValueExpression<TValue> leftHandExpression, ValueExpression<TValue> rightHandExpression)
        {
            _lh = leftHandExpression ?? throw new System.ArgumentNullException(nameof(leftHandExpression));
            _type = comparisonType ?? throw new System.ArgumentNullException(nameof(comparisonType));
            _rh = rightHandExpression ?? throw new System.ArgumentNullException(nameof(rightHandExpression));
        }

        protected override bool Evaluate_Internal(IStatSet<TValue> safeStatSet)
        {
            TValue exp1Result = _lh.Evaluate(safeStatSet);
            TValue exp2Result = _rh.Evaluate(safeStatSet);

            return _type.Evaluate(exp1Result, exp2Result);
        }
    }
    /// <summary>
    /// Expression object representing a conditional expression. Inputs are a logical (boolean) value as a condition, a value if true, and an alternative value if false.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class ConditionalExpression<TValue> : ValueExpression<TValue> where TValue : struct
    {
        private readonly LogicExpression<TValue> _condition;
        private readonly ValueExpression<TValue> _consequent, _alternative;
        public ConditionalExpression(LogicExpression<TValue> condition, ValueExpression<TValue> consequentExpression, ValueExpression<TValue> alternativeExpression)
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