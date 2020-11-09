using System.Collections.Generic;

namespace KRpgLib.Stats.Compound
{
    public interface IExpression
    {
        float Evaluate(IStatSet forStatSet);
    }
    public class Literal : IExpression
    {
        private readonly float _literal;
        public Literal(float literal)
        {
            _literal = literal;
        }
        public float Evaluate(IStatSet _)
        {
            return _literal;
        }
    }
    public class StatLiteral : IExpression
    {
        protected readonly IStatTemplate _template;
        private readonly bool _useLegalizedValue;
        public StatLiteral(IStatTemplate template, bool useLegalizedValue)
        {
            _template = template;
            _useLegalizedValue = useLegalizedValue;
        }
        public float Evaluate(IStatSet forStatSet)
        {
            float rawValue = forStatSet.GetStatValue(_template);
            
            if (_useLegalizedValue)
            {
                return _template.GetLegalizedValue(rawValue);
            }
            
            return rawValue;
        }
    }
    public class UnaryOperationType
    {
        private readonly System.Func<float, float> _unaryFunc;
        public UnaryOperationType(System.Func<float, float> unaryMathFunc)
        {
            _unaryFunc = unaryMathFunc;
        }
        public float Evaluate(float input)
        {
            return _unaryFunc(input);
        }
    }
    public class BinaryOperationType
    {
        private readonly System.Func<float, float, float> _binaryFunc;
        public BinaryOperationType(System.Func<float, float, float> binaryMathFunc)
        {
            _binaryFunc = binaryMathFunc;
        }
        public float Evaluate(float left, float right)
        {
            return _binaryFunc(left, right);
        }
    }
    public class Operation_Unary : IExpression
    {
        private readonly IExpression _expression;
        private readonly UnaryOperationType _operationType;

        public Operation_Unary(UnaryOperationType operationType, IExpression expression)
        {
            _operationType = operationType;
            _expression = expression;
        }
        public float Evaluate(IStatSet forStatSet)
        {
            float expResult = _expression.Evaluate(forStatSet);
            return _operationType.Evaluate(expResult);
        }
    }
    public class Operation_Binary : IExpression
    {
        private readonly IExpression _lh, _rh;
        private readonly BinaryOperationType _operationType;

        public Operation_Binary(BinaryOperationType operationType, IExpression leftHandExpression, IExpression rightHandExpression)
        {
            _operationType = operationType;
            _lh = leftHandExpression;
            _rh = rightHandExpression;
        }
        public float Evaluate(IStatSet forStatSet)
        {
            float exp1Result = _lh.Evaluate(forStatSet);
            float exp2Result = _rh.Evaluate(forStatSet);

            return _operationType.Evaluate(exp1Result, exp2Result);
        }
    }
    public class Comparison
    {
        private readonly IExpression _lh, _rh;
        private readonly ComparisonType _type;

        public Comparison(IExpression leftHandExpression, ComparisonType comparisonType, IExpression rightHandExpression)
        {
            _lh = leftHandExpression;
            _type = comparisonType;
            _rh = rightHandExpression;
        }
        public bool Evaluate(IStatSet forStatSet)
        {
            float exp1Result = _lh.Evaluate(forStatSet);
            float exp2Result = _rh.Evaluate(forStatSet);

            return _type.Evaluate(exp1Result, exp2Result);
        }
    }
    public class ComparisonType
    {
        private readonly System.Func<float, float, bool> _func;
        public ComparisonType(System.Func<float, float, bool> comparisonFunc)
        {
            _func = comparisonFunc;
        }
        public bool Evaluate(float value1, float value2)
        {
            return _func(value1, value2);
        }
    }

    // a.k.a. Statement.
    public interface IAlgorithmStep
    {
        float Apply(IStatSet statSet, float currentValue);
    }
    public class Step_DoNothing : IAlgorithmStep
    {
        public float Apply(IStatSet statSet, float currentValue)
        {
            return currentValue;
        }
    }
    public class Step_UnaryOperation : IAlgorithmStep
    {
        private readonly UnaryOperationType _operation;
        public Step_UnaryOperation(UnaryOperationType operation)
        {
            _operation = operation;
        }
        public float Apply(IStatSet statSet, float inputValue)
        {
            return _operation.Evaluate(inputValue);
        }
    }
    public class Step_BinaryOperation : IAlgorithmStep
    {
        private readonly BinaryOperationType _op;
        private readonly IExpression _rh;

        public Step_BinaryOperation(BinaryOperationType operationType, IExpression rightHandExpression)
        {
            _op = operationType;
            _rh = rightHandExpression;
        }
        public float Apply(IStatSet statSet, float inputValue)
        {
            float rightHandExpResult = _rh.Evaluate(statSet);
            return _op.Evaluate(inputValue, rightHandExpResult);
        }
    }
    // TODO: Make boolean expressions inside comparisons.
    public class Step_Conditional : IAlgorithmStep
    {
        private readonly Comparison _comp;
        private readonly List<IAlgorithmStep> _t, _f;
        public Step_Conditional(Comparison comparison, List<IAlgorithmStep> trueCaseBlock, List<IAlgorithmStep> falseCaseBlock)
        {
            _comp = comparison;
            _t = trueCaseBlock;
            _f = falseCaseBlock;
        }
        public Step_Conditional(Comparison comparison, IAlgorithmStep trueCase, List<IAlgorithmStep> falseCaseBlock)
            : this(comparison,
                 trueCase != null ? new List<IAlgorithmStep>() { trueCase } : null,
                 falseCaseBlock)
        { }
        public Step_Conditional(Comparison comparison, List<IAlgorithmStep> trueCaseBlock, IAlgorithmStep falseCase)
            : this(comparison,
                 trueCaseBlock,
                 falseCase != null ? new List<IAlgorithmStep>() { falseCase } : null)
        { }
        public Step_Conditional(Comparison comparison, IAlgorithmStep trueCase, IAlgorithmStep falseCase)
            :this(comparison,
                 trueCase != null ? new List<IAlgorithmStep>() { trueCase } : null,
                 falseCase != null ? new List<IAlgorithmStep>() { falseCase } : null)
        { }
        public float Apply(IStatSet statSet, float inputValue)
        {
            return _comp.Evaluate(statSet) ?
                ApplyStatementBlock(statSet, inputValue, _t) :
                ApplyStatementBlock(statSet, inputValue, _f);
        }
        private float ApplyStatementBlock(IStatSet statSet, float inputValue, List<IAlgorithmStep> statementBlock)
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
