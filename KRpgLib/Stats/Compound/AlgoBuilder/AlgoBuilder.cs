using System;
using System.Collections.Generic;

namespace KRpgLib.Stats.Compound.AlgoBuilder
{
    public enum Result
    {
        UNDEFINED = -1,
        OK = 0,
        FAIL = 1,
    }
    public abstract class AlgoBuilder<TValue> where TValue : struct
    {
        private readonly ExpressionRegistry<TValue> _expressionRegistry;
        private readonly StatTemplateRegistry<TValue> _statTemplateRegistry;

        public Result BuildResult { get; private set; }
        public string StatusMessage { get; private set; }

        protected AlgoBuilder()
        {
            _expressionRegistry = new ExpressionRegistry<TValue>();
            _statTemplateRegistry = new StatTemplateRegistry<TValue>();

            _expressionRegistry.Add("not", new List<string>() { "not" }, Op_Not);

            _expressionRegistry.Add("and", new List<string>() { "and" }, Op_And);
            _expressionRegistry.Add("or", new List<string>() { "or" }, Op_Or);
            _expressionRegistry.Add("xor", new List<string>() { "xor" }, Op_Xor);

            _expressionRegistry.Add("all", new List<string>() { "all" }, Op_All);
            _expressionRegistry.Add("any", new List<string>() { "any" }, Op_Any);
            _expressionRegistry.Add("one", new List<string>() { "one" }, Op_One);
        }
        protected abstract AbstractParser<TValue> GetParser(List<Token> tokens);
        public void RegisterStatTemplate(string identifier, IStatTemplate<TValue> statTemplate)
        {
            _statTemplateRegistry.Add(identifier, statTemplate);
        }
        protected void Error(string message)
        {
            BuildResult = Result.FAIL;
            StatusMessage = message;
        }
        public bool TryBuild(string script, out CompoundStatAlgorithm<TValue> algo)
        {
            if (string.IsNullOrWhiteSpace(script))
            {
                throw new ArgumentException("Argument may not be null, empty, or consist only of white-space characters.", nameof(script));
            }

            Scanner<TValue> scanner = new Scanner<TValue>(script);
            if (!scanner.TryScanTokens(out List<Token> tokens))
            {
                Error(scanner.StatusMessage);
                algo = null;
                return false;
            }

            AbstractParser<TValue> parser = GetParser(tokens);
            if (!parser.TryParseTokens(out ValueExpression<TValue> algoExpression))
            {
                Error(parser.StatusMessage);
                algo = null;
                return false;
            }

            // Return result.
            algo = new CompoundStatAlgorithm<TValue>(algoExpression);
            return true;
        }
        protected void LogicalUnaryOp(AbstractParser<TValue> parser, Func<LogicExpression<TValue>, LogicOperation_Unary<TValue>> theFunc)
        {
            if (parser.TryPopLogicExpression(out LogicExpression<TValue> logicExpression))
            {
                parser.DoPushLogicExpression(theFunc(logicExpression));
            }
        }
        protected void LogicalBinaryOp(AbstractParser<TValue> parser, Func<LogicExpression<TValue>, LogicExpression<TValue>, LogicOperation_Binary<TValue>> theFunc)
        {
            if (parser.TryPopLogicExpression(out LogicExpression<TValue> left) && parser.TryPopLogicExpression(out LogicExpression<TValue> right))
            {
                parser.DoPushLogicExpression(theFunc(left, right));
            }
        }
        protected void LogicalMultiaryOp(AbstractParser<TValue> parser, Func<List<LogicExpression<TValue>>, LogicOperation_Multiary<TValue>> theFunc)
        {
            if (parser.TryPopManyLogicExpressions(out List<LogicExpression<TValue>> logicExpressions))
            {
                parser.DoPushLogicExpression(theFunc(logicExpressions));
            }
        }
        protected void Op_Not(AbstractParser<TValue> parser) => LogicalUnaryOp(parser, input => new LogicOperations.LogicOperation_Not<TValue>(input));
        protected void Op_And(AbstractParser<TValue> parser) => LogicalBinaryOp(parser, (l, r) => new LogicOperations.LogicOperation_And<TValue>(l, r));
        protected void Op_Or(AbstractParser<TValue> parser) => LogicalBinaryOp(parser, (l, r) => new LogicOperations.LogicOperation_Or<TValue>(l, r));
        protected void Op_Xor(AbstractParser<TValue> parser) => LogicalBinaryOp(parser, (l, r) => new LogicOperations.LogicOperation_Xor<TValue>(l, r));
        protected void Op_All(AbstractParser<TValue> parser) => LogicalMultiaryOp(parser, exps => new LogicOperations.LogicOperation_All<TValue>(exps));
        protected void Op_Any(AbstractParser<TValue> parser) => LogicalMultiaryOp(parser, exps => new LogicOperations.LogicOperation_Any<TValue>(exps));
        protected void Op_One(AbstractParser<TValue> parser) => LogicalMultiaryOp(parser, exps => new LogicOperations.LogicOperation_One<TValue>(exps));
    }
}
