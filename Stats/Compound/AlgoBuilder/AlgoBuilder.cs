using System;
using System.Collections.Generic;

namespace KRpgLib.Stats.Compound.AlgoBuilder
{
    /// <summary>
    /// A result value from a stage of the AlgoBuilder process.
    /// </summary>
    public enum Result
    {
        UNDEFINED = -1,
        OK = 0,
        FAIL = 1,
    }
    /// <summary>
    /// A compound stat algorithm builder that uses a script called "Algo" unique to KRpgLib, which is beased on Scheme. Make a new subclass of this if your stat system needs to register special expressions. Register any math, comparison, and unique expressions with the ExpressionRegistry in the constructor.
    /// </summary>
    public class AlgoBuilder
    {
        protected ExpressionRegistry ExpressionRegistry { get; }
        protected StatRegistry StatRegistry { get; }

        /// <summary>
        /// The state of the builder after the last call to TryBuild().
        /// </summary>
        public Result BuildResult { get; private set; }
        /// <summary>
        /// The status message from the builder after the last call to TryBuild().
        /// </summary>
        public string StatusMessage { get; private set; }

        /// <summary>
        /// Register math, comparison, and unique expressions for your backing type with calls to ExpressionRegistry.Add().
        /// </summary>
        public AlgoBuilder()
        {
            // Assemble components.
            ExpressionRegistry = new ExpressionRegistry();
            StatRegistry = new StatRegistry();

            // Logical expressions and operations.
            ExpressionRegistry.Add(new List<string>() { "not" },
                ParserUtilities.PopUnaryLogicParams,
                q => ParserUtilities.ConstructUnaryOperation<LogicExpression>(
                    q, input => new LogicOperations.LogicOperation_Not(input)));

            ExpressionRegistry.Add(new List<string>() { "and" },
                ParserUtilities.PopBinaryLogicParams,
                q => ParserUtilities.ConstructBinaryOperation<LogicExpression>(
                    q, (l,r) => new LogicOperations.LogicOperation_And(l, r)));

            ExpressionRegistry.Add(new List<string>() { "or" },
                ParserUtilities.PopBinaryLogicParams,
                q => ParserUtilities.ConstructBinaryOperation<LogicExpression>(
                    q, (l, r) => new LogicOperations.LogicOperation_Or(l, r)));

            ExpressionRegistry.Add(new List<string>() { "xor" },
                ParserUtilities.PopBinaryLogicParams,
                q => ParserUtilities.ConstructBinaryOperation<LogicExpression>(
                    q, (l, r) => new LogicOperations.LogicOperation_Xor(l, r)));

            ExpressionRegistry.Add(new List<string>() { "all" },
                ParserUtilities.PopMultiaryLogicParams,
                q => ParserUtilities.ConstructMultiaryOperation<LogicExpression>(
                    q, list => new LogicOperations.LogicOperation_All(list)));

            ExpressionRegistry.Add(new List<string>() { "any" },
                ParserUtilities.PopMultiaryLogicParams,
                q => ParserUtilities.ConstructMultiaryOperation<LogicExpression>(
                    q, list => new LogicOperations.LogicOperation_Any(list)));

            ExpressionRegistry.Add(new List<string>() { "one" },
                ParserUtilities.PopMultiaryLogicParams,
                q => ParserUtilities.ConstructMultiaryOperation<LogicExpression>(
                    q, list => new LogicOperations.LogicOperation_One(list)));

            ExpressionRegistry.Add(new List<string>() { "if", "cond" },
                ParserUtilities.PopConditionalParams,
                q => ParserUtilities.ConstructConditional(q));

            // Value expressions and operations.
            ExpressionRegistry.Add(new List<string>() { "neg", "negative", "opposite" },
                ParserUtilities.PopUnaryValueParams,
                q => ParserUtilities.ConstructUnaryOperation<ValueExpression>(
                    q, input => new ValueOperation_Unary(CommonInstances.Negative, input)));

            ExpressionRegistry.Add(new List<string>() { "div", "divide" },
                ParserUtilities.PopBinaryValueParams,
                q => ParserUtilities.ConstructBinaryOperation<ValueExpression>(
                    q, (l, r) => new ValueOperation_Binary(CommonInstances.Divide, l, r)));
            ExpressionRegistry.Add(new List<string>() { "pow", "power" },
                ParserUtilities.PopBinaryValueParams,
                q => ParserUtilities.ConstructBinaryOperation<ValueExpression>(
                    q, (l, r) => new ValueOperation_Binary(CommonInstances.PowerOf, l, r)));
            ExpressionRegistry.Add(new List<string>() { "mod", "modulo", "modulus" },
                ParserUtilities.PopBinaryValueParams,
                q => ParserUtilities.ConstructBinaryOperation<ValueExpression>(
                    q, (l, r) => new ValueOperation_Binary(CommonInstances.Modulo, l, r)));

            ExpressionRegistry.Add(new List<string>() { "add", "plus", "sum" },
                ParserUtilities.PopMultiaryValueParams,
                q => ParserUtilities.ConstructMultiaryOperation<ValueExpression>(
                    q, list => new ValueOperation_Multiary(CommonInstances.Add, list)));
            ExpressionRegistry.Add(new List<string>() { "sub", "subtract", "minus" },
                ParserUtilities.PopMultiaryValueParams,
                q => ParserUtilities.ConstructMultiaryOperation<ValueExpression>(
                    q, list => new ValueOperation_Multiary(CommonInstances.Subtract, list)));
            ExpressionRegistry.Add(new List<string>() { "mul", "multiply", "times" },
                ParserUtilities.PopMultiaryValueParams,
                q => ParserUtilities.ConstructMultiaryOperation<ValueExpression>(
                    q, list => new ValueOperation_Multiary(CommonInstances.Multiply, list)));
            ExpressionRegistry.Add(new List<string>() { "min", "minimum", "smallest", "least" },
                ParserUtilities.PopMultiaryValueParams,
                q => ParserUtilities.ConstructMultiaryOperation<ValueExpression>(
                    q, list => new ValueOperation_Multiary(CommonInstances.Smallest, list)));
            ExpressionRegistry.Add(new List<string>() { "max", "maximum", "biggest", "greatest" },
                ParserUtilities.PopMultiaryValueParams,
                q => ParserUtilities.ConstructMultiaryOperation<ValueExpression>(
                    q, list => new ValueOperation_Multiary(CommonInstances.Biggest, list)));

            ExpressionRegistry.Add(new List<string>() { "equ", "eq", "is", "equal" },
                ParserUtilities.PopBinaryValueParams,
                q => ParserUtilities.ConstructComparison(q, CommonInstances.EqualTo));
            ExpressionRegistry.Add(new List<string>() { "neq", "nequ", "isnt", "notequal" },
                ParserUtilities.PopBinaryValueParams,
                q => ParserUtilities.ConstructComparison(q, CommonInstances.NotEqualTo));
            ExpressionRegistry.Add(new List<string>() { "gt", "greater", "greaterthan" },
                ParserUtilities.PopBinaryValueParams,
                q => ParserUtilities.ConstructComparison(q, CommonInstances.GreaterThan));
            ExpressionRegistry.Add(new List<string>() { "geq", "gequ", "gteq", "gtequ" },
                ParserUtilities.PopBinaryValueParams,
                q => ParserUtilities.ConstructComparison(q, CommonInstances.GreaterThanOrEqualTo));
            ExpressionRegistry.Add(new List<string>() { "lt", "less", "lessthan" },
                ParserUtilities.PopBinaryValueParams,
                q => ParserUtilities.ConstructComparison(q, CommonInstances.LessThan));
            ExpressionRegistry.Add(new List<string>() { "leq", "lequ", "lteq", "ltequ" },
                ParserUtilities.PopBinaryValueParams,
                q => ParserUtilities.ConstructComparison(q, CommonInstances.LessThanOrEqualTo));
        }
        /// <summary>
        /// Call this when your custom builder functions encounter an unrecoverable error, then return false from the function if applicable.
        /// </summary>
        /// <param name="message">error message</param>
        protected void Error(string message)
        {
            BuildResult = Result.FAIL;
            StatusMessage = message;
        }
        /// <summary>
        /// Tries to build a CompoundStatAlgorithm from the provided Algo script string.
        /// </summary>
        /// <param name="script">your Algo script string</param>
        /// <param name="algorithm">the built algorithm object</param>
        /// <returns>true if build was successful</returns>
        public bool TryBuild(string script, out CompoundStatAlgorithm algorithm)
        {
            if (string.IsNullOrWhiteSpace(script))
            {
                throw new ArgumentException("Argument may not be null, empty, or consist only of white-space characters.", nameof(script));
            }

            Scanner scanner = new Scanner(script);
            if (!scanner.TryScanTokens(out IReadOnlyList<Token> tokens))
            {
                Error(scanner.StatusMessage);
                algorithm = null;
                return false;
            }

            Parser parser = new Parser(tokens, ExpressionRegistry, StatRegistry);
            if (!parser.TryParseTokens(out ValueExpression algoExpressionTree))
            {
                Error(parser.StatusMessage);
                algorithm = null;
                return false;
            }

            // Return result.
            algorithm = new CompoundStatAlgorithm(algoExpressionTree);
            BuildResult = Result.OK;
            return true;
        }
        public void RegisterStat(string identifier, Stat statTemplate)
        {
            StatRegistry.Add(identifier, statTemplate);
        }
    }
}
