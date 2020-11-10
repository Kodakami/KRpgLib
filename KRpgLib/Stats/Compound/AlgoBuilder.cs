using System;
using System.Collections.Generic;
using System.Text;

namespace KRpgLib.Stats.Compound
{
    /// <summary>
    /// Build a CompoundStatAlgorithm in similar fashion to .NET's StringBuilder class.
    /// </summary>
    public class AlgoBuilder<TValue> where TValue : struct
    {
        protected readonly List<IAlgorithmStep<TValue>> _steps = new List<IAlgorithmStep<TValue>>();

        protected readonly ConditionalScopeManager _conditionalScopeManager = new ConditionalScopeManager();

        // Most start with a blank builder.
        public AlgoBuilder() { }
        
        // TODO: These are best replaced after statements (IAlgorithmStep) are refactored out.

        //public AlgoBuilder(IExpression<TValue> initialValue)
        //{
        //    if (initialValue != null)
        //    {
        //        SetTo(initialValue);
        //    }
        //}
        //public AlgoBuilder(TValue initialValue)
        //    :this(new Literal<TValue>(initialValue)) { }
        //public AlgoBuilder(IStatTemplate<TValue> initialValue, bool useLegalizedValue)
        //    :this(new StatLiteral<TValue>(initialValue, useLegalizedValue)) { }

        public CompoundStatAlgorithm<TValue> Build()
        {
            return new CompoundStatAlgorithm<TValue>(_steps.ToArray());
        }

        // Raw statement injection for slightly more fluent folks (who should probably be making algorithms directly).
        public void Inject(IAlgorithmStep<TValue> algorithmStep)
        {
            if (algorithmStep != null)
            {
                _steps.Add(algorithmStep ?? throw new System.ArgumentNullException(nameof(algorithmStep)));
            }
        }
        // Step generation.
        public void DoNothing()
        {
            IAlgorithmStep<TValue> step = new Step_DoNothing<TValue>();

            if (!_conditionalScopeManager.IsInsideConditional)
            {
                _steps.Add(step);
            }
            else
            {
                _conditionalScopeManager.AddStatementToCurrent(step);
            }
        }
        protected void BinaryOpStatement(BinaryOperationType<TValue> opType, IExpression<TValue> rightHand)
        {
            IAlgorithmStep<TValue> step = new Step_BinaryOperation<TValue>(
                    opType ?? throw new ArgumentNullException(nameof(opType)),
                    rightHand ?? throw new ArgumentNullException(nameof(rightHand)));

            if (!_conditionalScopeManager.IsInsideConditional)
            {
                _steps.Add(step);
            }
            else
            {
                _conditionalScopeManager.AddStatementToCurrent(step);
            }
        }
        protected void UnaryOpStatement(UnaryOperationType<TValue> opType)
        {
            IAlgorithmStep<TValue> step = new Step_UnaryOperation<TValue>(opType ?? throw new ArgumentNullException(nameof(opType)));

            if (!_conditionalScopeManager.IsInsideConditional)
            {
                _steps.Add(step);
            }
            else
            {
                _conditionalScopeManager.AddStatementToCurrent(step);
            }
        }

        // Conditional branching.
        public void If()
        {
            _conditionalScopeManager.BeginNewScope();
        }
        public void Then()
        {
            _conditionalScopeManager.BeginTrueCaseBlock();
        }
        public void Else()
        {
            _conditionalScopeManager.BeginFalseCaseBlock();
        }
        public void EndIf()
        {
            var step = _conditionalScopeManager.ConcludeAndGetCurrentStep();
            if (step != null)
            {
                if (!_conditionalScopeManager.IsInsideConditional)
                {
                    _steps.Add(step);
                }
                else
                {
                    _conditionalScopeManager.AddStatementToCurrent(step);
                }
            }
        }

        // Comparison generation.
        //Exp + Exp
        public void Compare(IExpression<TValue> leftHand, ComparisonType<TValue> comparisonType, IExpression<TValue> rightHand)
        {
            var newComparison = new Comparison<TValue>(
                leftHand ?? throw new ArgumentNullException(nameof(leftHand)),
                comparisonType ?? throw new ArgumentNullException(nameof(comparisonType)),
                rightHand ?? throw new ArgumentNullException(nameof(rightHand))
                );

            if (_conditionalScopeManager.IsInsideConditional)
            {
                _conditionalScopeManager.AddComparisonToCurrent(newComparison);
            }
            else
            {
                throw new InvalidOperationException("Unable to add comparison while not inside conditional scope.");
            }
        }
        //Stat + Stat
        public void Compare(IStatTemplate<TValue> leftHandValue, ComparisonType<TValue> comparisonType, IStatTemplate<TValue> rightHandValue) =>
            Compare(ValueOf(leftHandValue), comparisonType, ValueOf(rightHandValue));
        //We skip Num + Num because it's useless.

        //Num + Exp
        public void Compare(TValue lefthandNumber, ComparisonType<TValue> comparisonType, IExpression<TValue> rightHand) =>
            Compare(Number(lefthandNumber), comparisonType, rightHand);
        //Exp + Num
        public void Compare(IExpression<TValue> leftHand, ComparisonType<TValue> comparisonType, TValue rightHandNumber) =>
            Compare(leftHand, comparisonType, Number(rightHandNumber));

        //Stat + Exp
        public void Compare(IStatTemplate<TValue> leftHandValue, ComparisonType<TValue> comparisonType, IExpression<TValue> rightHand) =>
            Compare(ValueOf(leftHandValue), comparisonType, rightHand);
        //Exp + Stat
        public void Compare(IExpression<TValue> leftHand, ComparisonType<TValue> comparisonType, IStatTemplate<TValue> rightHandValue) =>
            Compare(leftHand, comparisonType, ValueOf(rightHandValue));

        //Num + Stat
        public void Compare(TValue leftHandNumber, ComparisonType<TValue> comparisonType, IStatTemplate<TValue> rightHandValue) =>
            Compare(Number(leftHandNumber), comparisonType, ValueOf(rightHandValue));
        //Stat + Num
        public void Compare(IStatTemplate<TValue> leftHandValue, ComparisonType<TValue> comparisonType, TValue rightHandNumber) =>
            Compare(ValueOf(leftHandValue), comparisonType, Number(rightHandNumber));

        // Literal generation.
        public Literal<TValue> Number(TValue number) => new Literal<TValue>(number);
        public StatLiteral<TValue> ValueOf(IStatTemplate<TValue> stat, bool useLegalizedValue = true)
        {
            if (stat != null)
            {
                return new StatLiteral<TValue>(stat, useLegalizedValue);
            }
            throw new ArgumentNullException(nameof(stat));
        }

        // Helper classes.
        protected sealed class ConditionalScopeManager
        {
            private readonly Stack<ConditionalScope> _stack = new Stack<ConditionalScope>();
            public bool IsInsideConditional => _stack.Count > 0;

            private ConditionalScope GetCurrentScope()
            {
                return _stack.Peek();
            }
            public void BeginNewScope()
            {
                _stack.Push(new ConditionalScope());
            }
            public void AddComparisonToCurrent(Comparison<TValue> comparison)
            {
                if (IsInsideConditional)
                {
                    var current = GetCurrentScope();
                    current.AddComparison(comparison);
                }
            }
            public void AddStatementToCurrent(IAlgorithmStep<TValue> statement)
            {
                if (IsInsideConditional)
                {
                    var current = GetCurrentScope();
                    current.AddStatement(statement);
                }
                throw new System.InvalidOperationException("Unable to add to case block while not inside conditional scope.");
            }
            public void BeginTrueCaseBlock()
            {
                if (IsInsideConditional)
                {
                    var current = GetCurrentScope();
                    current.Then();
                }
                throw new System.InvalidOperationException("Unable to begin true case block while not inside conditional scope.");
            }
            public void BeginFalseCaseBlock()
            {
                if (IsInsideConditional)
                {
                    var current = GetCurrentScope();
                    current.Else();
                }
                throw new System.InvalidOperationException("Unable to begin false case block while not inside conditional scope.");
            }
            /// <summary>
            /// May return null if conditional was invalid or had two empty branches.
            /// </summary>
            public IAlgorithmStep<TValue> ConcludeAndGetCurrentStep()
            {
                if (IsInsideConditional)
                {
                    ConditionalScope complete = _stack.Pop();
                    return complete.GetCompleteStep();
                }
                throw new System.InvalidOperationException("Unable to end current conditional scope while not inside conditional scope.");
            }
        }
        protected sealed class ConditionalScope
        {
            public enum ConditionalScopeState
            {
                COMPARISON = 0,
                TRUE_CASE = 1,
                FALSE_CASE = 2,
            }

            private Comparison<TValue> _comparison;
            private List<IAlgorithmStep<TValue>> _trueCaseBlock;
            private List<IAlgorithmStep<TValue>> _falseCaseBlock;

            public bool IsValid => _comparison != null;
            public ConditionalScopeState State;

            public ConditionalScope()
            {
                State = ConditionalScopeState.COMPARISON;
            }

            public void Then()
            {
                if (State == ConditionalScopeState.COMPARISON)
                {
                    State = ConditionalScopeState.TRUE_CASE;
                }
                throw new InvalidOperationException("Unexpected use of Then(). Expected: new algorithm step, Else(), or EndIf().");
            }
            public void Else()
            {
                if (State != ConditionalScopeState.FALSE_CASE)
                {
                    State = ConditionalScopeState.FALSE_CASE;
                }
                throw new InvalidOperationException("Unexpected use of Else(). Expected: new algorithm step or EndIf().");
            }
            public void AddComparison(Comparison<TValue> comparison)
            {
                if (State == ConditionalScopeState.COMPARISON)
                {
                    if (comparison == null)
                    {
                        return;
                    }
                    if (_comparison == null)
                    {
                        _comparison = comparison;
                    }
                    throw new InvalidOperationException("Unable to add more than one comparison to conditional statement. Expected: Then(), or Else().");
                }
                throw new InvalidOperationException("Unable to add comparison. Expected: Then(), or Else().");
            }
            public void AddStatement(IAlgorithmStep<TValue> statement)
            {
                if (statement == null)
                {
                    return;
                }

                if (State == ConditionalScopeState.TRUE_CASE)
                {
                    AddStatementToBlock(statement, ref _trueCaseBlock);
                }
                else if (State == ConditionalScopeState.FALSE_CASE)
                {
                    AddStatementToBlock(statement, ref _falseCaseBlock);
                }
                else
                {
                    throw new InvalidOperationException("Unable to add new algorithm step. Expected: Comparison.");
                }
            }
            private void AddStatementToBlock(IAlgorithmStep<TValue> statement, ref List<IAlgorithmStep<TValue>> block)
            {
                if (block == null)
                {
                    block = new List<IAlgorithmStep<TValue>>();
                }

                block.Add(statement);
            }
            public IAlgorithmStep<TValue> GetCompleteStep()
            {
                if (!IsValid)
                {
                    return null;
                }

                if (_trueCaseBlock == null && _falseCaseBlock == null)
                {
                    return null;
                }

                List<IAlgorithmStep<TValue>> sanitizedTrueCase =
                    _trueCaseBlock != null && _trueCaseBlock.Count > 0 ?
                    _trueCaseBlock :
                    new List<IAlgorithmStep<TValue>>() { new Step_DoNothing<TValue>() };

                List<IAlgorithmStep<TValue>> sanitizedFalseCase =
                    _falseCaseBlock != null && _falseCaseBlock.Count > 0 ?
                    _falseCaseBlock :
                    new List<IAlgorithmStep<TValue>>() { new Step_DoNothing<TValue>() };

                return new Step_Conditional<TValue>(_comparison, sanitizedTrueCase, sanitizedFalseCase);
            }
        }
    }
    public class AlgoBuilder_Float : AlgoBuilder<float>
    {
        //Addition
        public void Add(IExpression<float> expression) => BinaryOpStatement(CommonInstances.Float.Add, expression);
        public void Add(float literal) => Add(new Literal<float>(literal));
        public void Add(IStatTemplate<float> stat, bool useLegalizedValue = true) => Add(ValueOf(stat, useLegalizedValue));

        //Subtraction
        public void Subtract(IExpression<float> expression) => BinaryOpStatement(CommonInstances.Float.Subtract, expression);
        public void Subtract(float literal) => Subtract(new Literal<float>(literal));
        public void Subtract(IStatTemplate<float> stat, bool useLegalizedValue = true) => Subtract(ValueOf(stat, useLegalizedValue));

        //Multiplication
        public void MultiplyBy(IExpression<float> expression) => BinaryOpStatement(CommonInstances.Float.Multiply, expression);
        public void MultiplyBy(float literal) => Add(new Literal<float>(literal));
        public void MultiplyBy(IStatTemplate<float> stat, bool useLegalizedValue = true) => MultiplyBy(ValueOf(stat, useLegalizedValue));

        //Division
        public void DivideBy(IExpression<float> expression) => BinaryOpStatement(CommonInstances.Float.Divide, expression);
        public void DivideBy(float literal) => DivideBy(new Literal<float>(literal));
        public void DivideBy(IStatTemplate<float> stat, bool useLegalizedValue = true) => DivideBy(ValueOf(stat, useLegalizedValue));

        //Powers
        public void PowerOf(IExpression<float> expression) => BinaryOpStatement(CommonInstances.Float.PowerOf, expression);
        public void PowerOf(float literal) => PowerOf(new Literal<float>(literal));
        public void PowerOf(IStatTemplate<float> stat, bool useLegalizedValue = true) => PowerOf(ValueOf(stat, useLegalizedValue));

        //Min
        public void SmallerOf(IExpression<float> expression) => BinaryOpStatement(CommonInstances.Float.Min, expression);
        public void SmallerOf(float literal) => SmallerOf(new Literal<float>(literal));
        public void SmallerOf(IStatTemplate<float> stat, bool useLegalizedValue = true) => SmallerOf(ValueOf(stat, useLegalizedValue));

        //Max
        public void LargerOf(IExpression<float> expression) => BinaryOpStatement(CommonInstances.Float.Max, expression);
        public void LargerOf(float literal) => LargerOf(new Literal<float>(literal));
        public void LargerOf(IStatTemplate<float> stat, bool useLegalizedValue = true) => LargerOf(ValueOf(stat, useLegalizedValue));

        //Modulo
        public void Modulo(IExpression<float> expression) => BinaryOpStatement(CommonInstances.Float.Modulo, expression);
        public void Modulo(float literal) => Modulo(new Literal<float>(literal));
        public void Modulo(IStatTemplate<float> stat, bool useLegalizedValue = true) => Modulo(ValueOf(stat, useLegalizedValue));

        //Set
        public void SetTo(IExpression<float> expression) => BinaryOpStatement(CommonInstances.Float.SetTo, expression);
        public void SetTo(float literal) => SetTo(new Literal<float>(literal));
        public void SetTo(IStatTemplate<float> stat, bool useLegalizedValue = true) => SetTo(ValueOf(stat, useLegalizedValue));

        //Negation (two methods that do the same thing)
        public void FlipValue() => UnaryOpStatement(CommonInstances.Float.Negative);
        public void Negative() => UnaryOpStatement(CommonInstances.Float.Negative);

        //Comparison type shortcuts.
        public ComparisonType<float> EqualTo => CommonInstances.Float.EqualTo;
        public ComparisonType<float> NotEqualTo => CommonInstances.Float.NotEqualTo;
        public ComparisonType<float> GreaterThan => CommonInstances.Float.GreaterThan;
        public ComparisonType<float> GreaterThanOrEqualTo => CommonInstances.Float.GreaterThanOrEqualTo;
        public ComparisonType<float> LessThan => CommonInstances.Float.LessThan;
        public ComparisonType<float> LessThanOrEqualTo => CommonInstances.Float.LessThanOrEqualTo;
    }
    // Copy-pasting is ugly.
    public class AlgoBuilder_Int : AlgoBuilder<int>
    {
        //Addition
        public void Add(IExpression<int> expression) => BinaryOpStatement(CommonInstances.Int.Add, expression);
        public void Add(int literal) => Add(new Literal<int>(literal));
        public void Add(IStatTemplate<int> stat, bool useLegalizedValue = true) => Add(ValueOf(stat, useLegalizedValue));

        //Subtraction
        public void Subtract(IExpression<int> expression) => BinaryOpStatement(CommonInstances.Int.Subtract, expression);
        public void Subtract(int literal) => Subtract(new Literal<int>(literal));
        public void Subtract(IStatTemplate<int> stat, bool useLegalizedValue = true) => Subtract(ValueOf(stat, useLegalizedValue));

        //Multiplication
        public void MultiplyBy(IExpression<int> expression) => BinaryOpStatement(CommonInstances.Int.Multiply, expression);
        public void MultiplyBy(int literal) => Add(new Literal<int>(literal));
        public void MultiplyBy(IStatTemplate<int> stat, bool useLegalizedValue = true) => MultiplyBy(ValueOf(stat, useLegalizedValue));

        //Division
        public void DivideBy(IExpression<int> expression) => BinaryOpStatement(CommonInstances.Int.Divide, expression);
        public void DivideBy(int literal) => DivideBy(new Literal<int>(literal));
        public void DivideBy(IStatTemplate<int> stat, bool useLegalizedValue = true) => DivideBy(ValueOf(stat, useLegalizedValue));

        //Powers
        public void PowerOf(IExpression<int> expression) => BinaryOpStatement(CommonInstances.Int.PowerOf, expression);
        public void PowerOf(int literal) => PowerOf(new Literal<int>(literal));
        public void PowerOf(IStatTemplate<int> stat, bool useLegalizedValue = true) => PowerOf(ValueOf(stat, useLegalizedValue));

        //Min
        public void SmallerOf(IExpression<int> expression) => BinaryOpStatement(CommonInstances.Int.Min, expression);
        public void SmallerOf(int literal) => SmallerOf(new Literal<int>(literal));
        public void SmallerOf(IStatTemplate<int> stat, bool useLegalizedValue = true) => SmallerOf(ValueOf(stat, useLegalizedValue));

        //Max
        public void LargerOf(IExpression<int> expression) => BinaryOpStatement(CommonInstances.Int.Max, expression);
        public void LargerOf(int literal) => LargerOf(new Literal<int>(literal));
        public void LargerOf(IStatTemplate<int> stat, bool useLegalizedValue = true) => LargerOf(ValueOf(stat, useLegalizedValue));

        //Modulo
        public void Modulo(IExpression<int> expression) => BinaryOpStatement(CommonInstances.Int.Modulo, expression);
        public void Modulo(int literal) => Modulo(new Literal<int>(literal));
        public void Modulo(IStatTemplate<int> stat, bool useLegalizedValue = true) => Modulo(ValueOf(stat, useLegalizedValue));

        //Set
        public void SetTo(IExpression<int> expression) => BinaryOpStatement(CommonInstances.Int.SetTo, expression);
        public void SetTo(int literal) => SetTo(new Literal<int>(literal));
        public void SetTo(IStatTemplate<int> stat, bool useLegalizedValue = true) => SetTo(ValueOf(stat, useLegalizedValue));

        //Negation (two methods that do the same thing)
        public void FlipValue() => UnaryOpStatement(CommonInstances.Int.Negative);
        public void Negative() => UnaryOpStatement(CommonInstances.Int.Negative);

        //Comparison type shortcuts.
        public ComparisonType<int> EqualTo => CommonInstances.Int.EqualTo;
        public ComparisonType<int> NotEqualTo => CommonInstances.Int.NotEqualTo;
        public ComparisonType<int> GreaterThan => CommonInstances.Int.GreaterThan;
        public ComparisonType<int> GreaterThanOrEqualTo => CommonInstances.Int.GreaterThanOrEqualTo;
        public ComparisonType<int> LessThan => CommonInstances.Int.LessThan;
        public ComparisonType<int> LessThanOrEqualTo => CommonInstances.Int.LessThanOrEqualTo;
    }
}
