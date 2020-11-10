using System.Collections.Generic;

namespace KRpgLib.Stats.Compound
{
    public interface IExpression<TValue> where TValue : struct
    {
        TValue Evaluate(IStatSet<TValue> forStatSet);
    }
    public class Literal<TValue> : IExpression<TValue> where TValue : struct
    {
        private readonly TValue _literal;
        public Literal(TValue literal)
        {
            _literal = literal;
        }
        public TValue Evaluate(IStatSet<TValue> _)
        {
            return _literal;
        }
    }
    public class StatLiteral<TValue> : IExpression<TValue> where TValue : struct
    {
        protected readonly IStatTemplate<TValue> _template;
        private readonly bool _useLegalizedValue;
        public StatLiteral(IStatTemplate<TValue> template, bool useLegalizedValue)
        {
            _template = template;
            _useLegalizedValue = useLegalizedValue;
        }
        public TValue Evaluate(IStatSet<TValue> forStatSet)
        {
            TValue rawValue = forStatSet.GetStatValue(_template);

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
            _unaryFunc = unaryFunc;
        }
        public TValue Evaluate(TValue input)
        {
            return _unaryFunc(input);
        }
    }
    public class BinaryOperationType<TValue> where TValue : struct
    {
        private readonly System.Func<TValue, TValue, TValue> _binaryFunc;
        public BinaryOperationType(System.Func<TValue, TValue, TValue> binaryMathFunc)
        {
            _binaryFunc = binaryMathFunc;
        }
        public TValue Evaluate(TValue left, TValue right)
        {
            return _binaryFunc(left, right);
        }
    }
    public class Operation_Unary<TValue> : IExpression<TValue> where TValue : struct
    {
        private readonly IExpression<TValue> _expression;
        private readonly UnaryOperationType<TValue> _operationType;

        public Operation_Unary(UnaryOperationType<TValue> operationType, IExpression<TValue> expression)
        {
            _operationType = operationType;
            _expression = expression;
        }
        public TValue Evaluate(IStatSet<TValue> forStatSet)
        {
            TValue expResult = _expression.Evaluate(forStatSet);
            return _operationType.Evaluate(expResult);
        }
    }
    public class Operation_Binary<TValue> : IExpression<TValue> where TValue : struct
    {
        private readonly IExpression<TValue> _lh, _rh;
        private readonly BinaryOperationType<TValue> _operationType;

        public Operation_Binary(BinaryOperationType<TValue> operationType, IExpression<TValue> leftHandExpression, IExpression<TValue> rightHandExpression)
        {
            _operationType = operationType;
            _lh = leftHandExpression;
            _rh = rightHandExpression;
        }
        public TValue Evaluate(IStatSet<TValue> forStatSet)
        {
            TValue exp1Result = _lh.Evaluate(forStatSet);
            TValue exp2Result = _rh.Evaluate(forStatSet);

            return _operationType.Evaluate(exp1Result, exp2Result);
        }
    }
    public class Comparison<TValue> where TValue : struct
    {
        private readonly IExpression<TValue> _lh, _rh;
        private readonly ComparisonType<TValue> _type;

        public Comparison(IExpression<TValue> leftHandExpression, ComparisonType<TValue> comparisonType, IExpression<TValue> rightHandExpression)
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

    // a.k.a. Statement.
    public interface IAlgorithmStep<TValue> where TValue : struct
    {
        TValue Apply(IStatSet<TValue> statSet, TValue currentValue);
    }
    public class Step_DoNothing<TValue> : IAlgorithmStep<TValue> where TValue : struct
    {
        public TValue Apply(IStatSet<TValue> statSet, TValue currentValue)
        {
            return currentValue;
        }
    }
    public class Step_UnaryOperation<TValue> : IAlgorithmStep<TValue> where TValue : struct
    {
        private readonly UnaryOperationType<TValue> _operation;
        public Step_UnaryOperation(UnaryOperationType<TValue> operation)
        {
            _operation = operation;
        }
        public TValue Apply(IStatSet<TValue> statSet, TValue inputValue)
        {
            return _operation.Evaluate(inputValue);
        }
    }
    public class Step_BinaryOperation<TValue> : IAlgorithmStep<TValue> where TValue : struct
    {
        private readonly BinaryOperationType<TValue> _op;
        private readonly IExpression<TValue> _rh;

        public Step_BinaryOperation(BinaryOperationType<TValue> operationType, IExpression<TValue> rightHandExpression)
        {
            _op = operationType;
            _rh = rightHandExpression;
        }
        public TValue Apply(IStatSet<TValue> statSet, TValue inputValue)
        {
            TValue rightHandExpResult = _rh.Evaluate(statSet);
            return _op.Evaluate(inputValue, rightHandExpResult);
        }
    }
    // TODO: Make boolean expressions inside comparisons.
    // TODO: Make block statements their own expression type.
    // TODO: Make Legalize a block.
    public class Step_Conditional<TValue> : IAlgorithmStep<TValue> where TValue : struct
    {
        private readonly Comparison<TValue> _comp;
        private readonly List<IAlgorithmStep<TValue>> _t, _f;
        public Step_Conditional(Comparison<TValue> comparison, List<IAlgorithmStep<TValue>> trueCaseBlock, List<IAlgorithmStep<TValue>> falseCaseBlock)
        {
            _comp = comparison;
            _t = trueCaseBlock;
            _f = falseCaseBlock;
        }
        public Step_Conditional(Comparison<TValue> comparison, IAlgorithmStep<TValue> trueCase, List<IAlgorithmStep<TValue>> falseCaseBlock)
            : this(comparison,
                 trueCase != null ? new List<IAlgorithmStep<TValue>>() { trueCase } : null,
                 falseCaseBlock)
        { }
        public Step_Conditional(Comparison<TValue> comparison, List<IAlgorithmStep<TValue>> trueCaseBlock, IAlgorithmStep<TValue> falseCase)
            : this(comparison,
                 trueCaseBlock,
                 falseCase != null ? new List<IAlgorithmStep<TValue>>() { falseCase } : null)
        { }
        public Step_Conditional(Comparison<TValue> comparison, IAlgorithmStep<TValue> trueCase, IAlgorithmStep<TValue> falseCase)
            :this(comparison,
                 trueCase != null ? new List<IAlgorithmStep<TValue>>() { trueCase } : null,
                 falseCase != null ? new List<IAlgorithmStep<TValue>>() { falseCase } : null)
        { }
        public TValue Apply(IStatSet<TValue> statSet, TValue inputValue)
        {
            return _comp.Evaluate(statSet) ?
                ApplyStatementBlock(statSet, inputValue, _t) :
                ApplyStatementBlock(statSet, inputValue, _f);
        }
        private TValue ApplyStatementBlock(IStatSet<TValue> statSet, TValue inputValue, List<IAlgorithmStep<TValue>> statementBlock)
        {
            if (statementBlock != null)
            {
                foreach (var statement in statementBlock)
                {
                    if (statement != null)
                    {
                        inputValue = statement.Apply(statSet, inputValue);
                    }
                }
            }
            return inputValue;
        }
    }
}
