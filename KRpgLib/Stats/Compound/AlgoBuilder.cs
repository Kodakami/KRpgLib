using System;
using System.Collections.Generic;
using System.Text;

namespace KRpgLib.Stats.Compound
{
    /// <summary>
    /// Build a CompoundStatAlgorithm in similar fashion to .NET's StringBuilder class.
    /// </summary>
    public class AlgoBuilder
    {
        protected readonly List<IAlgorithmStep> _steps = new List<IAlgorithmStep>();

        protected readonly ConditionalScopeManager _conditionalScopeManager = new ConditionalScopeManager();

        // Most start with a blank builder.
        public AlgoBuilder() { }
        public AlgoBuilder(IExpression initialValue)
        {
            if (initialValue != null)
            {
                SetTo(initialValue);
            }
        }
        public AlgoBuilder(float initialValue)
            :this(new Literal(initialValue)) { }
        public AlgoBuilder(IStatTemplate initialValue, bool useLegalizedValue)
            :this(new StatLiteral(initialValue, useLegalizedValue)) { }

        public CompoundStatAlgorithm Build()
        {
            return new CompoundStatAlgorithm(_steps.ToArray());
        }

        // Raw statement injection for slightly more fluent folks (who should probably be making algorithms directly).
        public void Inject(IAlgorithmStep algorithmStep)
        {
            if (algorithmStep != null)
            {
                _steps.Add(algorithmStep);
            }
        }
        // Step generation.
        public void DoNothing()
        {
            IAlgorithmStep step = new Step_DoNothing();

            if (!_conditionalScopeManager.IsInsideConditional)
            {
                _steps.Add(step);
            }
            else
            {
                _conditionalScopeManager.AddStatementToCurrent(step);
            }
        }
        protected void BinaryOpStatement(BinaryOperationType opType, IExpression rightHand)
        {
            if (rightHand != null && opType != null)
            {
                IAlgorithmStep step = new Step_BinaryOperation(opType, rightHand);

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
        protected void UnaryOpStatement(UnaryOperationType opType)
        {
            if (opType != null)
            {
                IAlgorithmStep step = new Step_UnaryOperation(opType);

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

        //Addition
        public void Add(IExpression expression) => BinaryOpStatement(CommonInstances.Add, expression);
        public void Add(float literal) => Add(new Literal(literal));
        public void Add(IStatTemplate stat, bool useLegalizedValue = true) => Add(ValueOf(stat, useLegalizedValue));

        //Subtraction
        public void Subtract(IExpression expression) => BinaryOpStatement(CommonInstances.Subtract, expression);
        public void Subtract(float literal) => Subtract(new Literal(literal));
        public void Subtract(IStatTemplate stat, bool useLegalizedValue = true) => Subtract(ValueOf(stat, useLegalizedValue));

        //Multiplication
        public void MultiplyBy(IExpression expression) => BinaryOpStatement(CommonInstances.Multiply, expression);
        public void MultiplyBy(float literal) => Add(new Literal(literal));
        public void MultiplyBy(IStatTemplate stat, bool useLegalizedValue = true) => MultiplyBy(ValueOf(stat, useLegalizedValue));

        //Division
        public void DivideBy(IExpression expression) => BinaryOpStatement(CommonInstances.Divide, expression);
        public void DivideBy(float literal) => DivideBy(new Literal(literal));
        public void DivideBy(IStatTemplate stat, bool useLegalizedValue = true) => DivideBy(ValueOf(stat, useLegalizedValue));

        //Powers
        public void PowerOf(IExpression expression) => BinaryOpStatement(CommonInstances.PowerOf, expression);
        public void PowerOf(float literal) => PowerOf(new Literal(literal));
        public void PowerOf(IStatTemplate stat, bool useLegalizedValue = true) => PowerOf(ValueOf(stat, useLegalizedValue));

        //Min
        public void SmallerOf(IExpression expression) => BinaryOpStatement(CommonInstances.Min, expression);
        public void SmallerOf(float literal) => SmallerOf(new Literal(literal));
        public void SmallerOf(IStatTemplate stat, bool useLegalizedValue = true) => SmallerOf(ValueOf(stat, useLegalizedValue));

        //Max
        public void LargerOf(IExpression expression) => BinaryOpStatement(CommonInstances.Max, expression);
        public void LargerOf(float literal) => LargerOf(new Literal(literal));
        public void LargerOf(IStatTemplate stat, bool useLegalizedValue = true) => LargerOf(ValueOf(stat, useLegalizedValue));

        //Modulo
        public void Modulo(IExpression expression) => BinaryOpStatement(CommonInstances.Modulo, expression);
        public void Modulo(float literal) => Modulo(new Literal(literal));
        public void Modulo(IStatTemplate stat, bool useLegalizedValue = true) => Modulo(ValueOf(stat, useLegalizedValue));

        //Set
        public void SetTo(IExpression expression) => BinaryOpStatement(CommonInstances.SetTo, expression);
        public void SetTo(float literal) => SetTo(new Literal(literal));
        public void SetTo(IStatTemplate stat, bool useLegalizedValue = true) => SetTo(ValueOf(stat, useLegalizedValue));

        //Negation (two methods that do the same thing)
        public void FlipValue() => UnaryOpStatement(CommonInstances.Negative);
        public void Negative() => UnaryOpStatement(CommonInstances.Negative);

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
        public void Compare(IExpression leftHand, ComparisonType comparisonType, IExpression rightHand)
        {
            var newComparison = new Comparison(
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
        public void Compare(IStatTemplate leftHandValue, ComparisonType comparisonType, IStatTemplate rightHandValue) =>
            Compare(ValueOf(leftHandValue), comparisonType, ValueOf(rightHandValue));
        //We skip Num + Num because it's useless.

        //Num + Exp
        public void Compare(float lefthandNumber, ComparisonType comparisonType, IExpression rightHand) =>
            Compare(Number(lefthandNumber), comparisonType, rightHand);
        //Exp + Num
        public void Compare(IExpression leftHand, ComparisonType comparisonType, float rightHandNumber) =>
            Compare(leftHand, comparisonType, Number(rightHandNumber));

        //Stat + Exp
        public void Compare(IStatTemplate leftHandValue, ComparisonType comparisonType, IExpression rightHand) =>
            Compare(ValueOf(leftHandValue), comparisonType, rightHand);
        //Exp + Stat
        public void Compare(IExpression leftHand, ComparisonType comparisonType, IStatTemplate rightHandValue) =>
            Compare(leftHand, comparisonType, ValueOf(rightHandValue));

        //Num + Stat
        public void Compare(float leftHandNumber, ComparisonType comparisonType, IStatTemplate rightHandValue) =>
            Compare(Number(leftHandNumber), comparisonType, ValueOf(rightHandValue));
        //Stat + Num
        public void Compare(IStatTemplate leftHandValue, ComparisonType comparisonType, float rightHandNumber) =>
            Compare(ValueOf(leftHandValue), comparisonType, Number(rightHandNumber));

        //Comparison type shortcuts.
        public ComparisonType EqualTo => CommonInstances.EqualTo;
        public ComparisonType NotEqualTo => CommonInstances.NotEqualTo;
        public ComparisonType GreaterThan => CommonInstances.GreaterThan;
        public ComparisonType GreaterThanOrEqualTo => CommonInstances.GreaterThanOrEqualTo;
        public ComparisonType LessThan => CommonInstances.LessThan;
        public ComparisonType LessThanOrEqualTo => CommonInstances.LessThanOrEqualTo;

        // Literal generation.
        public Literal Number(float number) => new Literal(number);
        public StatLiteral ValueOf(IStatTemplate stat, bool useLegalizedValue = true)
        {
            if (stat != null)
            {
                return new StatLiteral(stat, useLegalizedValue);
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
            public void AddComparisonToCurrent(Comparison comparison)
            {
                if (IsInsideConditional)
                {
                    var current = GetCurrentScope();
                    current.AddComparison(comparison);
                }
            }
            public void AddStatementToCurrent(IAlgorithmStep statement)
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
            public IAlgorithmStep ConcludeAndGetCurrentStep()
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

            private Comparison _comparison;
            private List<IAlgorithmStep> _trueCaseBlock;
            private List<IAlgorithmStep> _falseCaseBlock;

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
            public void AddComparison(Comparison comparison)
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
            public void AddStatement(IAlgorithmStep statement)
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
            private void AddStatementToBlock(IAlgorithmStep statement, ref List<IAlgorithmStep> block)
            {
                if (block == null)
                {
                    block = new List<IAlgorithmStep>();
                }

                block.Add(statement);
            }
            public IAlgorithmStep GetCompleteStep()
            {
                if (!IsValid)
                {
                    return null;
                }

                if (_trueCaseBlock == null && _falseCaseBlock == null)
                {
                    return null;
                }

                List<IAlgorithmStep> sanitizedTrueCase =
                    _trueCaseBlock != null && _trueCaseBlock.Count > 0 ?
                    _trueCaseBlock :
                    new List<IAlgorithmStep>() { new Step_DoNothing() };

                List<IAlgorithmStep> sanitizedFalseCase =
                    _falseCaseBlock != null && _falseCaseBlock.Count > 0 ?
                    _falseCaseBlock :
                    new List<IAlgorithmStep>() { new Step_DoNothing() };

                return new Step_Conditional(_comparison, sanitizedTrueCase, sanitizedFalseCase);
            }
        }
    }
}
