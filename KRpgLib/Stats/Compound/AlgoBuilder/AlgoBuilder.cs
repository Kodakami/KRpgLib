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
    /// Base class for Algo builders. Make a new subclass of this if your stat system uses non-int, non-float backing values. Register any math, comparison, and unique expressions with the ExpressionRegistry in the constructor. Override GetParser() with a new instance of your concrete AbstractParser<YourBackingType> subclass.
    /// </summary>
    /// <typeparam name="TValue">stat backing value</typeparam>
    public abstract class AlgoBuilder<TValue> where TValue : struct
    {
        protected ExpressionRegistry<TValue> ExpressionRegistry { get; }
        protected StatTemplateRegistry<TValue> StatTemplateRegistry { get; }

        /// <summary>
        /// The state of the builder after the last call to TryBuild().
        /// </summary>
        public Result BuildResult { get; private set; }
        /// <summary>
        /// The status message from the builder after the last call to TryBuild().
        /// </summary>
        public string StatusMessage { get; private set; }

        /// <summary>
        /// Register math, comparison, and unique expressions for your backing type with calls to ExpressionRegistry,Add().
        /// </summary>
        protected AlgoBuilder()
        {
            ExpressionRegistry = new ExpressionRegistry<TValue>();
            StatTemplateRegistry = new StatTemplateRegistry<TValue>();

            ExpressionRegistry.Add(
                "not", new List<string>() { "not" },
                ExpressionInfoUtilities<TValue>.PopUnaryLogicParams,
                q => ExpressionInfoUtilities<TValue>.ConstructUnaryOperation<LogicExpression<TValue>>(
                    q, input => new LogicOperations.LogicOperation_Not<TValue>(input)));

            ExpressionRegistry.Add(
                "and", new List<string>() { "and" },
                ExpressionInfoUtilities<TValue>.PopBinaryLogicParams,
                q => ExpressionInfoUtilities<TValue>.ConstructBinaryOperation<LogicExpression<TValue>>(
                    q, (l,r) => new LogicOperations.LogicOperation_And<TValue>(l, r)));

            ExpressionRegistry.Add(
                "or", new List<string>() { "or" },
                ExpressionInfoUtilities<TValue>.PopBinaryLogicParams,
                q => ExpressionInfoUtilities<TValue>.ConstructBinaryOperation<LogicExpression<TValue>>(
                    q, (l, r) => new LogicOperations.LogicOperation_Or<TValue>(l, r)));

            ExpressionRegistry.Add(
                "xor", new List<string>() { "xor" },
                ExpressionInfoUtilities<TValue>.PopBinaryLogicParams,
                q => ExpressionInfoUtilities<TValue>.ConstructBinaryOperation<LogicExpression<TValue>>(
                    q, (l, r) => new LogicOperations.LogicOperation_Xor<TValue>(l, r)));

            ExpressionRegistry.Add(
                "all", new List<string>() { "all" },
                ExpressionInfoUtilities<TValue>.PopMultiaryLogicParams,
                q => ExpressionInfoUtilities<TValue>.ConstructMultiaryOperation<LogicExpression<TValue>>(
                    q, list => new LogicOperations.LogicOperation_All<TValue>(list)));

            ExpressionRegistry.Add(
                "any", new List<string>() { "any" },
                ExpressionInfoUtilities<TValue>.PopMultiaryLogicParams,
                q => ExpressionInfoUtilities<TValue>.ConstructMultiaryOperation<LogicExpression<TValue>>(
                    q, list => new LogicOperations.LogicOperation_Any<TValue>(list)));

            ExpressionRegistry.Add(
                "one", new List<string>() { "one" },
                ExpressionInfoUtilities<TValue>.PopMultiaryLogicParams,
                q => ExpressionInfoUtilities<TValue>.ConstructMultiaryOperation<LogicExpression<TValue>>(
                    q, list => new LogicOperations.LogicOperation_One<TValue>(list)));

            ExpressionRegistry.Add(
                "if", new List<string>() { "if", "cond" },
                ExpressionInfoUtilities<TValue>.PopConditionalParams,
                q => ExpressionInfoUtilities<TValue>.ConstructConditional(q));
        }
        /// <summary>
        /// Get a new instance of your backing value's own concrete subclass of AbstractParser<YourBackingType>.
        /// </summary>
        /// <param name="tokens">a list of tokens emitted by the scanner</param>
        /// <returns></returns>
        protected abstract AbstractParser<TValue> GetParser(List<Token> tokens);
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
        /// Tries to build a CompoundStatAlgorithm<YourBackingType> from the provided Algo script string. Calls to Error() beforehand if returning false.
        /// </summary>
        /// <param name="script">your Algo script string</param>
        /// <param name="algorithm">the built algorithm object</param>
        /// <returns>true if build was successful</returns>
        public bool TryBuild(string script, out CompoundStatAlgorithm<TValue> algorithm)
        {
            if (string.IsNullOrWhiteSpace(script))
            {
                throw new ArgumentException("Argument may not be null, empty, or consist only of white-space characters.", nameof(script));
            }

            Scanner scanner = new Scanner(script);
            if (!scanner.TryScanTokens(out List<Token> tokens))
            {
                Error(scanner.StatusMessage);
                algorithm = null;
                return false;
            }

            AbstractParser<TValue> parser = GetParser(tokens);
            if (!parser.TryParseTokens(out ValueExpression<TValue> algoExpression))
            {
                Error(parser.StatusMessage);
                algorithm = null;
                return false;
            }

            // Return result.
            algorithm = new CompoundStatAlgorithm<TValue>(algoExpression);
            BuildResult = Result.OK;
            return true;
        }
        public void RegisterStat(string identifier, IStatTemplate<TValue> statTemplate)
        {
            StatTemplateRegistry.Add(identifier, statTemplate);
        }
    }
    /// <summary>
    /// Concrete builder class for stats with int (System.Int32) backing values. Create a subclass of this to register additional expressions in the constructor.
    /// </summary>
    public class AlgoBuilder_Int : AlgoBuilder<int>
    {
        public AlgoBuilder_Int()
        {
            ExpressionRegistry.Add(
                "negate", new List<string>() { "neg", "negative", "opposite" },
                ExpressionInfoUtilities<int>.PopUnaryValueParams,
                q => ExpressionInfoUtilities<int>.ConstructUnaryOperation<ValueExpression<int>>(
                    q, input => new ValueOperation_Unary<int>(CommonInstances.Int.Negative, input)));

            ExpressionRegistry.Add(
                "divide", new List<string>() { "div" },
                ExpressionInfoUtilities<int>.PopBinaryValueParams,
                q => ExpressionInfoUtilities<int>.ConstructBinaryOperation<ValueExpression<int>>(
                    q, (l,r) => new ValueOperation_Binary<int>(CommonInstances.Int.Divide, l, r)));
            ExpressionRegistry.Add(
                "power", new List<string>() { "pow", },
                ExpressionInfoUtilities<int>.PopBinaryValueParams,
                q => ExpressionInfoUtilities<int>.ConstructBinaryOperation<ValueExpression<int>>(
                    q, (l, r) => new ValueOperation_Binary<int>(CommonInstances.Int.PowerOf, l, r)));
            ExpressionRegistry.Add(
                "modulo", new List<string>() { "mod", "modulo", "modulus" },
                ExpressionInfoUtilities<int>.PopBinaryValueParams,
                q => ExpressionInfoUtilities<int>.ConstructBinaryOperation<ValueExpression<int>>(
                    q, (l, r) => new ValueOperation_Binary<int>(CommonInstances.Int.Modulo, l, r)));

            ExpressionRegistry.Add(
                "add", new List<string>() { "add", "plus" },
                ExpressionInfoUtilities<int>.PopMultiaryValueParams,
                q => ExpressionInfoUtilities<int>.ConstructMultiaryOperation<ValueExpression<int>>(
                    q, list => new ValueOperation_Multiary<int>(CommonInstances.Int.Add, list)));
            ExpressionRegistry.Add(
                "subtract", new List<string>() { "sub", "minus" },
                ExpressionInfoUtilities<int>.PopMultiaryValueParams,
                q => ExpressionInfoUtilities<int>.ConstructMultiaryOperation<ValueExpression<int>>(
                    q, list => new ValueOperation_Multiary<int>(CommonInstances.Int.Subtract, list)));
            ExpressionRegistry.Add(
                "multiply", new List<string>() { "mul", "times" },
                ExpressionInfoUtilities<int>.PopMultiaryValueParams,
                q => ExpressionInfoUtilities<int>.ConstructMultiaryOperation<ValueExpression<int>>(
                    q, list => new ValueOperation_Multiary<int>(CommonInstances.Int.Multiply, list)));
            ExpressionRegistry.Add(
                "minimum", new List<string>() { "min", "smallest" },
                ExpressionInfoUtilities<int>.PopMultiaryValueParams,
                q => ExpressionInfoUtilities<int>.ConstructMultiaryOperation<ValueExpression<int>>(
                    q, list => new ValueOperation_Multiary<int>(CommonInstances.Int.Min, list)));
            ExpressionRegistry.Add(
                "maximum", new List<string>() { "max", "biggest" },
                ExpressionInfoUtilities<int>.PopMultiaryValueParams,
                q => ExpressionInfoUtilities<int>.ConstructMultiaryOperation<ValueExpression<int>>(
                    q, list => new ValueOperation_Multiary<int>(CommonInstances.Int.Max, list)));

            ExpressionRegistry.Add(
                "equalto", new List<string>() { "equ", "eq", "is" },
                ExpressionInfoUtilities<int>.PopBinaryValueParams,
                q => ExpressionInfoUtilities<int>.ConstructComparison(q, CommonInstances.Int.EqualTo));
            ExpressionRegistry.Add(
                "notequalto", new List<string>() { "neq", "nequ", "isnt" },
                ExpressionInfoUtilities<int>.PopBinaryValueParams,
                q => ExpressionInfoUtilities<int>.ConstructComparison(q, CommonInstances.Int.NotEqualTo));
            ExpressionRegistry.Add(
                "greaterthan", new List<string>() { "gt" },
                ExpressionInfoUtilities<int>.PopBinaryValueParams,
                q => ExpressionInfoUtilities<int>.ConstructComparison(q, CommonInstances.Int.GreaterThan));
            ExpressionRegistry.Add(
                "greaterthanorequalto", new List<string>() { "geq", "gequ", "gteq", "gtequ" },
                ExpressionInfoUtilities<int>.PopBinaryValueParams,
                q => ExpressionInfoUtilities<int>.ConstructComparison(q, CommonInstances.Int.GreaterThanOrEqualTo));
            ExpressionRegistry.Add(
                "lessthan", new List<string>() { "lt" },
                ExpressionInfoUtilities<int>.PopBinaryValueParams,
                q => ExpressionInfoUtilities<int>.ConstructComparison(q, CommonInstances.Int.LessThan));
            ExpressionRegistry.Add(
                "lessthanorequalto", new List<string>() { "leq", "lequ", "lteq", "ltequ" },
                ExpressionInfoUtilities<int>.PopBinaryValueParams,
                q => ExpressionInfoUtilities<int>.ConstructComparison(q, CommonInstances.Int.LessThanOrEqualTo));
        }
        protected override AbstractParser<int> GetParser(List<Token> tokens)
        {
            return new Parser_Int(tokens, ExpressionRegistry, StatTemplateRegistry);
        }
    }
    /// <summary>
    /// Concrete builder class for stats with float (System.Single) backing values. Create a subclass of this to register additional expressions in the constructor.
    /// </summary>
    public class AlgoBuilder_Float : AlgoBuilder<float>
    {
        public AlgoBuilder_Float()
        {
            ExpressionRegistry.Add(
                "negate", new List<string>() { "neg", "negative", "opposite" },
                ExpressionInfoUtilities<float>.PopUnaryValueParams,
                q => ExpressionInfoUtilities<float>.ConstructUnaryOperation<ValueExpression<float>>(
                    q, input => new ValueOperation_Unary<float>(CommonInstances.Float.Negative, input)));

            ExpressionRegistry.Add(
                "divide", new List<string>() { "div" },
                ExpressionInfoUtilities<float>.PopBinaryValueParams,
                q => ExpressionInfoUtilities<float>.ConstructBinaryOperation<ValueExpression<float>>(
                    q, (l, r) => new ValueOperation_Binary<float>(CommonInstances.Float.Divide, l, r)));
            ExpressionRegistry.Add(
                "power", new List<string>() { "pow", },
                ExpressionInfoUtilities<float>.PopBinaryValueParams,
                q => ExpressionInfoUtilities<float>.ConstructBinaryOperation<ValueExpression<float>>(
                    q, (l, r) => new ValueOperation_Binary<float>(CommonInstances.Float.PowerOf, l, r)));
            ExpressionRegistry.Add(
                "modulo", new List<string>() { "mod", "modulo", "modulus" },
                ExpressionInfoUtilities<float>.PopBinaryValueParams,
                q => ExpressionInfoUtilities<float>.ConstructBinaryOperation<ValueExpression<float>>(
                    q, (l, r) => new ValueOperation_Binary<float>(CommonInstances.Float.Modulo, l, r)));

            ExpressionRegistry.Add(
                "add", new List<string>() { "add", "plus" },
                ExpressionInfoUtilities<float>.PopMultiaryValueParams,
                q => ExpressionInfoUtilities<float>.ConstructMultiaryOperation<ValueExpression<float>>(
                    q, list => new ValueOperation_Multiary<float>(CommonInstances.Float.Add, list)));
            ExpressionRegistry.Add(
                "subtract", new List<string>() { "sub", "minus" },
                ExpressionInfoUtilities<float>.PopMultiaryValueParams,
                q => ExpressionInfoUtilities<float>.ConstructMultiaryOperation<ValueExpression<float>>(
                    q, list => new ValueOperation_Multiary<float>(CommonInstances.Float.Subtract, list)));
            ExpressionRegistry.Add(
                "multiply", new List<string>() { "mul", "times" },
                ExpressionInfoUtilities<float>.PopMultiaryValueParams,
                q => ExpressionInfoUtilities<float>.ConstructMultiaryOperation<ValueExpression<float>>(
                    q, list => new ValueOperation_Multiary<float>(CommonInstances.Float.Multiply, list)));
            ExpressionRegistry.Add(
                "minimum", new List<string>() { "min", "smallest" },
                ExpressionInfoUtilities<float>.PopMultiaryValueParams,
                q => ExpressionInfoUtilities<float>.ConstructMultiaryOperation<ValueExpression<float>>(
                    q, list => new ValueOperation_Multiary<float>(CommonInstances.Float.Min, list)));
            ExpressionRegistry.Add(
                "maximum", new List<string>() { "max", "biggest" },
                ExpressionInfoUtilities<float>.PopMultiaryValueParams,
                q => ExpressionInfoUtilities<float>.ConstructMultiaryOperation<ValueExpression<float>>(
                    q, list => new ValueOperation_Multiary<float>(CommonInstances.Float.Max, list)));

            ExpressionRegistry.Add(
                "equalto", new List<string>() { "equ", "eq", "is" },
                ExpressionInfoUtilities<float>.PopBinaryValueParams,
                q => ExpressionInfoUtilities<float>.ConstructComparison(q, CommonInstances.Float.EqualTo));
            ExpressionRegistry.Add(
                "notequalto", new List<string>() { "neq", "nequ", "isnt" },
                ExpressionInfoUtilities<float>.PopBinaryValueParams,
                q => ExpressionInfoUtilities<float>.ConstructComparison(q, CommonInstances.Float.NotEqualTo));
            ExpressionRegistry.Add(
                "greaterthan", new List<string>() { "gt" },
                ExpressionInfoUtilities<float>.PopBinaryValueParams,
                q => ExpressionInfoUtilities<float>.ConstructComparison(q, CommonInstances.Float.GreaterThan));
            ExpressionRegistry.Add(
                "greaterthanorequalto", new List<string>() { "geq", "gequ", "gteq", "gtequ" },
                ExpressionInfoUtilities<float>.PopBinaryValueParams,
                q => ExpressionInfoUtilities<float>.ConstructComparison(q, CommonInstances.Float.GreaterThanOrEqualTo));
            ExpressionRegistry.Add(
                "lessthan", new List<string>() { "lt" },
                ExpressionInfoUtilities<float>.PopBinaryValueParams,
                q => ExpressionInfoUtilities<float>.ConstructComparison(q, CommonInstances.Float.LessThan));
            ExpressionRegistry.Add(
                "lessthanorequalto", new List<string>() { "leq", "lequ", "lteq", "ltequ" },
                ExpressionInfoUtilities<float>.PopBinaryValueParams,
                q => ExpressionInfoUtilities<float>.ConstructComparison(q, CommonInstances.Float.LessThanOrEqualTo));
        }
        protected override AbstractParser<float> GetParser(List<Token> tokens)
        {
            return new Parser_Float(tokens, ExpressionRegistry, StatTemplateRegistry);
        }
    }
}
