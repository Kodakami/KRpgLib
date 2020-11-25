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

            ExpressionRegistry.Add(new List<string>() { "not" },
                ParserUtilities<TValue>.PopUnaryLogicParams,
                q => ParserUtilities<TValue>.ConstructUnaryOperation<LogicExpression<TValue>>(
                    q, input => new LogicOperations.LogicOperation_Not<TValue>(input)));

            ExpressionRegistry.Add(new List<string>() { "and" },
                ParserUtilities<TValue>.PopBinaryLogicParams,
                q => ParserUtilities<TValue>.ConstructBinaryOperation<LogicExpression<TValue>>(
                    q, (l,r) => new LogicOperations.LogicOperation_And<TValue>(l, r)));

            ExpressionRegistry.Add(new List<string>() { "or" },
                ParserUtilities<TValue>.PopBinaryLogicParams,
                q => ParserUtilities<TValue>.ConstructBinaryOperation<LogicExpression<TValue>>(
                    q, (l, r) => new LogicOperations.LogicOperation_Or<TValue>(l, r)));

            ExpressionRegistry.Add(new List<string>() { "xor" },
                ParserUtilities<TValue>.PopBinaryLogicParams,
                q => ParserUtilities<TValue>.ConstructBinaryOperation<LogicExpression<TValue>>(
                    q, (l, r) => new LogicOperations.LogicOperation_Xor<TValue>(l, r)));

            ExpressionRegistry.Add(new List<string>() { "all" },
                ParserUtilities<TValue>.PopMultiaryLogicParams,
                q => ParserUtilities<TValue>.ConstructMultiaryOperation<LogicExpression<TValue>>(
                    q, list => new LogicOperations.LogicOperation_All<TValue>(list)));

            ExpressionRegistry.Add(new List<string>() { "any" },
                ParserUtilities<TValue>.PopMultiaryLogicParams,
                q => ParserUtilities<TValue>.ConstructMultiaryOperation<LogicExpression<TValue>>(
                    q, list => new LogicOperations.LogicOperation_Any<TValue>(list)));

            ExpressionRegistry.Add(new List<string>() { "one" },
                ParserUtilities<TValue>.PopMultiaryLogicParams,
                q => ParserUtilities<TValue>.ConstructMultiaryOperation<LogicExpression<TValue>>(
                    q, list => new LogicOperations.LogicOperation_One<TValue>(list)));

            ExpressionRegistry.Add(new List<string>() { "if", "cond" },
                ParserUtilities<TValue>.PopConditionalParams,
                q => ParserUtilities<TValue>.ConstructConditional(q));
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
            ExpressionRegistry.Add(new List<string>() { "neg", "negative", "opposite" },
                ParserUtilities<int>.PopUnaryValueParams,
                q => ParserUtilities<int>.ConstructUnaryOperation<ValueExpression<int>>(
                    q, input => new ValueOperation_Unary<int>(CommonInstances.Int.Negative, input)));

            ExpressionRegistry.Add(new List<string>() { "div" },
                ParserUtilities<int>.PopBinaryValueParams,
                q => ParserUtilities<int>.ConstructBinaryOperation<ValueExpression<int>>(
                    q, (l,r) => new ValueOperation_Binary<int>(CommonInstances.Int.Divide, l, r)));
            ExpressionRegistry.Add(new List<string>() { "pow", },
                ParserUtilities<int>.PopBinaryValueParams,
                q => ParserUtilities<int>.ConstructBinaryOperation<ValueExpression<int>>(
                    q, (l, r) => new ValueOperation_Binary<int>(CommonInstances.Int.PowerOf, l, r)));
            ExpressionRegistry.Add(new List<string>() { "mod", "modulo", "modulus" },
                ParserUtilities<int>.PopBinaryValueParams,
                q => ParserUtilities<int>.ConstructBinaryOperation<ValueExpression<int>>(
                    q, (l, r) => new ValueOperation_Binary<int>(CommonInstances.Int.Modulo, l, r)));

            ExpressionRegistry.Add(new List<string>() { "add", "plus" },
                ParserUtilities<int>.PopMultiaryValueParams,
                q => ParserUtilities<int>.ConstructMultiaryOperation<ValueExpression<int>>(
                    q, list => new ValueOperation_Multiary<int>(CommonInstances.Int.Add, list)));
            ExpressionRegistry.Add(new List<string>() { "sub", "minus" },
                ParserUtilities<int>.PopMultiaryValueParams,
                q => ParserUtilities<int>.ConstructMultiaryOperation<ValueExpression<int>>(
                    q, list => new ValueOperation_Multiary<int>(CommonInstances.Int.Subtract, list)));
            ExpressionRegistry.Add(new List<string>() { "mul", "times" },
                ParserUtilities<int>.PopMultiaryValueParams,
                q => ParserUtilities<int>.ConstructMultiaryOperation<ValueExpression<int>>(
                    q, list => new ValueOperation_Multiary<int>(CommonInstances.Int.Multiply, list)));
            ExpressionRegistry.Add(new List<string>() { "min", "smallest" },
                ParserUtilities<int>.PopMultiaryValueParams,
                q => ParserUtilities<int>.ConstructMultiaryOperation<ValueExpression<int>>(
                    q, list => new ValueOperation_Multiary<int>(CommonInstances.Int.Min, list)));
            ExpressionRegistry.Add(new List<string>() { "max", "biggest" },
                ParserUtilities<int>.PopMultiaryValueParams,
                q => ParserUtilities<int>.ConstructMultiaryOperation<ValueExpression<int>>(
                    q, list => new ValueOperation_Multiary<int>(CommonInstances.Int.Max, list)));

            ExpressionRegistry.Add(new List<string>() { "equ", "eq", "is" },
                ParserUtilities<int>.PopBinaryValueParams,
                q => ParserUtilities<int>.ConstructComparison(q, CommonInstances.Int.EqualTo));
            ExpressionRegistry.Add(new List<string>() { "neq", "nequ", "isnt" },
                ParserUtilities<int>.PopBinaryValueParams,
                q => ParserUtilities<int>.ConstructComparison(q, CommonInstances.Int.NotEqualTo));
            ExpressionRegistry.Add(new List<string>() { "gt" },
                ParserUtilities<int>.PopBinaryValueParams,
                q => ParserUtilities<int>.ConstructComparison(q, CommonInstances.Int.GreaterThan));
            ExpressionRegistry.Add(new List<string>() { "geq", "gequ", "gteq", "gtequ" },
                ParserUtilities<int>.PopBinaryValueParams,
                q => ParserUtilities<int>.ConstructComparison(q, CommonInstances.Int.GreaterThanOrEqualTo));
            ExpressionRegistry.Add(new List<string>() { "lt" },
                ParserUtilities<int>.PopBinaryValueParams,
                q => ParserUtilities<int>.ConstructComparison(q, CommonInstances.Int.LessThan));
            ExpressionRegistry.Add(new List<string>() { "leq", "lequ", "lteq", "ltequ" },
                ParserUtilities<int>.PopBinaryValueParams,
                q => ParserUtilities<int>.ConstructComparison(q, CommonInstances.Int.LessThanOrEqualTo));
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
            ExpressionRegistry.Add(new List<string>() { "neg", "negative", "opposite" },
                ParserUtilities<float>.PopUnaryValueParams,
                q => ParserUtilities<float>.ConstructUnaryOperation<ValueExpression<float>>(
                    q, input => new ValueOperation_Unary<float>(CommonInstances.Float.Negative, input)));

            ExpressionRegistry.Add(new List<string>() { "div" },
                ParserUtilities<float>.PopBinaryValueParams,
                q => ParserUtilities<float>.ConstructBinaryOperation<ValueExpression<float>>(
                    q, (l, r) => new ValueOperation_Binary<float>(CommonInstances.Float.Divide, l, r)));
            ExpressionRegistry.Add(new List<string>() { "pow", },
                ParserUtilities<float>.PopBinaryValueParams,
                q => ParserUtilities<float>.ConstructBinaryOperation<ValueExpression<float>>(
                    q, (l, r) => new ValueOperation_Binary<float>(CommonInstances.Float.PowerOf, l, r)));
            ExpressionRegistry.Add(new List<string>() { "mod", "modulo", "modulus" },
                ParserUtilities<float>.PopBinaryValueParams,
                q => ParserUtilities<float>.ConstructBinaryOperation<ValueExpression<float>>(
                    q, (l, r) => new ValueOperation_Binary<float>(CommonInstances.Float.Modulo, l, r)));

            ExpressionRegistry.Add(new List<string>() { "add", "plus" },
                ParserUtilities<float>.PopMultiaryValueParams,
                q => ParserUtilities<float>.ConstructMultiaryOperation<ValueExpression<float>>(
                    q, list => new ValueOperation_Multiary<float>(CommonInstances.Float.Add, list)));
            ExpressionRegistry.Add(new List<string>() { "sub", "minus" },
                ParserUtilities<float>.PopMultiaryValueParams,
                q => ParserUtilities<float>.ConstructMultiaryOperation<ValueExpression<float>>(
                    q, list => new ValueOperation_Multiary<float>(CommonInstances.Float.Subtract, list)));
            ExpressionRegistry.Add(new List<string>() { "mul", "times" },
                ParserUtilities<float>.PopMultiaryValueParams,
                q => ParserUtilities<float>.ConstructMultiaryOperation<ValueExpression<float>>(
                    q, list => new ValueOperation_Multiary<float>(CommonInstances.Float.Multiply, list)));
            ExpressionRegistry.Add(new List<string>() { "min", "smallest" },
                ParserUtilities<float>.PopMultiaryValueParams,
                q => ParserUtilities<float>.ConstructMultiaryOperation<ValueExpression<float>>(
                    q, list => new ValueOperation_Multiary<float>(CommonInstances.Float.Min, list)));
            ExpressionRegistry.Add(new List<string>() { "max", "biggest" },
                ParserUtilities<float>.PopMultiaryValueParams,
                q => ParserUtilities<float>.ConstructMultiaryOperation<ValueExpression<float>>(
                    q, list => new ValueOperation_Multiary<float>(CommonInstances.Float.Max, list)));

            ExpressionRegistry.Add(new List<string>() { "equ", "eq", "is" },
                ParserUtilities<float>.PopBinaryValueParams,
                q => ParserUtilities<float>.ConstructComparison(q, CommonInstances.Float.EqualTo));
            ExpressionRegistry.Add(new List<string>() { "neq", "nequ", "isnt" },
                ParserUtilities<float>.PopBinaryValueParams,
                q => ParserUtilities<float>.ConstructComparison(q, CommonInstances.Float.NotEqualTo));
            ExpressionRegistry.Add(new List<string>() { "gt" },
                ParserUtilities<float>.PopBinaryValueParams,
                q => ParserUtilities<float>.ConstructComparison(q, CommonInstances.Float.GreaterThan));
            ExpressionRegistry.Add(new List<string>() { "geq", "gequ", "gteq", "gtequ" },
                ParserUtilities<float>.PopBinaryValueParams,
                q => ParserUtilities<float>.ConstructComparison(q, CommonInstances.Float.GreaterThanOrEqualTo));
            ExpressionRegistry.Add(new List<string>() { "lt" },
                ParserUtilities<float>.PopBinaryValueParams,
                q => ParserUtilities<float>.ConstructComparison(q, CommonInstances.Float.LessThan));
            ExpressionRegistry.Add(new List<string>() { "leq", "lequ", "lteq", "ltequ" },
                ParserUtilities<float>.PopBinaryValueParams,
                q => ParserUtilities<float>.ConstructComparison(q, CommonInstances.Float.LessThanOrEqualTo));
        }
        protected override AbstractParser<float> GetParser(List<Token> tokens)
        {
            return new Parser_Float(tokens, ExpressionRegistry, StatTemplateRegistry);
        }
    }
}
