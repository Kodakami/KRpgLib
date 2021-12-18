using System;
using System.Collections.Generic;
using System.Text;

namespace KRpgLib.Stats.Compound.AlgoBuilder
{
    /// <summary>
    /// A delegate for functions which pop objects off the top of a Parser's stack and push to an existing queue of parameters.
    /// </summary>
    /// <param name="parser">parser instance with object stack</param>
    /// <param name="queue">parameter queue</param>
    /// <returns>true if process encountered no errors</returns>
    public delegate bool PopParamsFunc(Parser parser, Queue<object> queue);
    /// <summary>
    /// A delegate for functions which return a new instance of an object (usually an Expression object) to be placed on a Parser's object stack.
    /// </summary>
    /// <param name="queue">parameter queue</param>
    /// <returns>new object</returns>
    public delegate object ExpressionCtorDelegate(Queue<object> queue);

    /// <summary>
    /// Methods for building Expression objects inside a parser.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public static class ParserUtilities
    {
        private static bool PopCountedParams_Internal<TExpression>(
            int count,
            string expectedObjName,
            Parser parser,
            Queue<object> queue)
        {
            for (int i = 0; i < count; i++)
            {
                if (!parser.TryPopAsType(expectedObjName, out TExpression result))
                {
                    return false;
                }
                queue.Enqueue(result);
            }
            return true;
        }
        private static bool PopMultiaryParams_Internal<TExpression>(string expectedObjName, Parser parser, Queue<object> paramQueue)
        {
            if (parser.TryPopManyAsType(expectedObjName, out List<TExpression> list))
            {
                foreach (var paramItem in list)
                {
                    paramQueue.Enqueue(paramItem);
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// Pop the expected parameters for a conditional expression (condition (LogicExpression), consequent (ValueExpression), alternative (ValueExpression)).
        /// </summary>
        public static bool PopConditionalParams(Parser parser, Queue<object> paramQueue)
        {
            if (parser.TryPopAsType("Logic expression", out LogicExpression condition))
            {
                paramQueue.Enqueue(condition);

                if (parser.TryPopAsType("Value expression", out ValueExpression consequent))
                {
                    paramQueue.Enqueue(consequent);

                    if (parser.TryPopAsType("Value expression", out ValueExpression alternative))
                    {
                        paramQueue.Enqueue(alternative);
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool PopCountedLogicParams_Internal(int count, Parser parser, Queue<object> paramQueue)
        {
            return PopCountedParams_Internal<LogicExpression>(count, "Logic Expression", parser, paramQueue);
        }
        private static bool PopCountedValueParams_Internal(int count, Parser parser, Queue<object> paramQueue)
        {
            return PopCountedParams_Internal<ValueExpression>(count, "Value Expression", parser, paramQueue);
        }
        /// <summary>
        /// Pop the expected parameter for a unary logic operation (input (LogicExpression)).
        /// </summary>
        public static bool PopUnaryLogicParams(Parser parser, Queue<object> paramQueue)
        {
            return PopCountedLogicParams_Internal(1, parser, paramQueue);
        }
        /// <summary>
        /// Pop the expected parameters for a binary logic operation (leftHandValue (LogicExpression), rightHandValue (LogicExpression)).
        /// </summary>
        public static bool PopBinaryLogicParams(Parser parser, Queue<object> paramQueue)
        {
            return PopCountedLogicParams_Internal(2, parser, paramQueue);
        }
        /// <summary>
        /// Pop the expected parameters for a multiary logic operation (params (IEnumerable<LogicExpression>)).
        /// </summary>
        public static bool PopMultiaryLogicParams(Parser parser, Queue<object> paramQueue)
        {
            return PopMultiaryParams_Internal<LogicExpression>("Logic Expression", parser, paramQueue);
        }
        /// <summary>
        /// Pop the expected parameter for a unary value operation (input (ValueExpression)).
        /// </summary>
        public static bool PopUnaryValueParams(Parser parser, Queue<object> paramQueue)
        {
            return PopCountedValueParams_Internal(1, parser, paramQueue);
        }
        /// <summary>
        /// Pop the expected parameters for a binary value operation (leftHandValue (ValueExpression), rightHandValue (ValueExpression)).
        /// </summary>
        public static bool PopBinaryValueParams(Parser parser, Queue<object> paramQueue)
        {
            return PopCountedValueParams_Internal(2, parser, paramQueue);
        }
        /// <summary>
        /// Pop the expected parameters for a multiary value operation (params (IEnumerable<ValueExpression>)).
        /// </summary>
        public static bool PopMultiaryValueParams(Parser parser, Queue<object> paramQueue)
        {
            return PopMultiaryParams_Internal<ValueExpression>("Value Expression", parser, paramQueue);
        }

        /// <summary>
        /// Given a parameter queue and constructor, construct a unary operation object.
        /// </summary>
        /// <typeparam name="TParam">parameter expected type</typeparam>
        /// <param name="paramQueue">queue of parameters from PopParamsDelegate</param>
        /// <param name="unaryCtor">unary operation constructor</param>
        /// <returns>new unary operation object</returns>
        public static object ConstructUnaryOperation<TParam>(Queue<object> paramQueue, System.Func<TParam, object> unaryCtor)
        {
            var input = (TParam)paramQueue.Dequeue();

            return unaryCtor.Invoke(input);
        }
        /// <summary>
        /// Given a parameter queue and constructor, construct a binary operation object.
        /// </summary>
        /// <typeparam name="TParam">parameter expected type</typeparam>
        /// <param name="paramQueue">queue of parameters from PopParamsDelegate</param>
        /// <param name="binaryCtor">binary operation constructor</param>
        /// <returns>new binary operation object</returns>
        public static object ConstructBinaryOperation<TParam>(Queue<object> paramQueue, System.Func<TParam, TParam, object> binaryCtor)
        {
            var left = (TParam)paramQueue.Dequeue();
            var right = (TParam)paramQueue.Dequeue();

            return binaryCtor.Invoke(left, right);
        }
        /// <summary>
        /// Given a parameter queue and constructor, construct a multiary operation object.
        /// </summary>
        /// <typeparam name="TParam">parameter expected type</typeparam>
        /// <param name="paramQueue">queue of parameters from PopParamsDelegate</param>
        /// <param name="multiaryCtor">multiary operation constructor</param>
        /// <returns>new multiary operation object</returns>
        public static object ConstructMultiaryOperation<TParam>(Queue<object> paramQueue, System.Func<List<TParam>, object> multiaryCtor)
        {
            List<TParam> list = new List<TParam>();
            while (paramQueue.Count > 0)
            {
                list.Add((TParam)paramQueue.Dequeue());
            }

            return multiaryCtor.Invoke(list);
        }
        /// <summary>
        /// Given a parameter queue and comparison type, construct a comparison expression object.
        /// </summary>
        /// <param name="paramQueue">queue of parameters from PopParamsDelegate</param>
        /// <param name="comparisonType">comparison type</param>
        /// <returns>new comparison expression object</returns>
        public static object ConstructComparison(Queue<object> paramQueue, ComparisonType comparisonType)
        {
            var left = (ValueExpression)paramQueue.Dequeue();
            var right = (ValueExpression)paramQueue.Dequeue();

            return new Comparison(comparisonType, left, right);
        }
        /// <summary>
        /// Given a parameter queue, construct a conditional expression object.
        /// </summary>
        /// <param name="paramQueue">queue of parameters from PopParamsDelegate</param>
        /// <returns>new conditional expression object</returns>
        public static object ConstructConditional(Queue<object> paramQueue)
        {
            var condition = (LogicExpression)paramQueue.Dequeue();
            var consequent = (ValueExpression)paramQueue.Dequeue();
            var alternative = (ValueExpression)paramQueue.Dequeue();

            return new ConditionalExpression(condition, consequent, alternative);
        }
    }
}
