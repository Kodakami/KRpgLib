using System;
using System.Collections.Generic;
using System.Text;

namespace KRpgLib.Stats.Compound.AlgoBuilder
{
    /// <summary>
    /// A delegate for functions which pop objects off the top of an AbstractParser's stack and push to an existing queue of parameters.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    /// <param name="parser">parser instance with object stack</param>
    /// <param name="queue">parameter queue</param>
    /// <returns>true if process encountered no errors</returns>
    public delegate bool PopParamsDelegate<TValue>(AbstractParser<TValue> parser, Queue<object> queue) where TValue : struct;
    /// <summary>
    /// A delegate for functions which return a new instance of an object (usually an Expression object) to be placed on an AbstractParser's object stack.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    /// <param name="queue">parameter queue</param>
    /// <returns>new object</returns>
    public delegate object ExpressionCtorDelegate<TValue>(Queue<object> queue) where TValue : struct;

    /// <summary>
    /// Methods for building Expression objects inside a parser.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public static class ParserUtilities<TValue> where TValue : struct
    {
        private static bool PopCountedParams_Internal<TExpression>(
            int count,
            string expectedObjName,
            AbstractParser<TValue> parser,
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
        private static bool PopMultiaryParams_Internal<TExpression>(string expectedObjName, AbstractParser<TValue> parser, Queue<object> paramQueue)
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
        public static bool PopConditionalParams(AbstractParser<TValue> parser, Queue<object> paramQueue)
        {
            if (parser.TryPopAsType("Logic expression", out LogicExpression<TValue> condition))
            {
                paramQueue.Enqueue(condition);

                if (parser.TryPopAsType("Value expression", out ValueExpression<TValue> consequent))
                {
                    paramQueue.Enqueue(consequent);

                    if (parser.TryPopAsType("Value expression", out ValueExpression<TValue> alternative))
                    {
                        paramQueue.Enqueue(alternative);
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool PopCountedLogicParams_Internal(int count, AbstractParser<TValue> parser, Queue<object> paramQueue)
        {
            return PopCountedParams_Internal<LogicExpression<TValue>>(count, "Logic Expression", parser, paramQueue);
        }
        private static bool PopCountedValueParams_Internal(int count, AbstractParser<TValue> parser, Queue<object> paramQueue)
        {
            return PopCountedParams_Internal<ValueExpression<TValue>>(count, "Value Expression", parser, paramQueue);
        }
        /// <summary>
        /// Pop the expected parameter for a unary logic operation (input (logic expression)).
        /// </summary>
        public static bool PopUnaryLogicParams(AbstractParser<TValue> parser, Queue<object> paramQueue)
        {
            return PopCountedLogicParams_Internal(1, parser, paramQueue);
        }
        /// <summary>
        /// Pop the expected parameters for a binary logic operation (leftHandValue (LogicExpression), rightHandValue (LogicExpression)).
        /// </summary>
        public static bool PopBinaryLogicParams(AbstractParser<TValue> parser, Queue<object> paramQueue)
        {
            return PopCountedLogicParams_Internal(2, parser, paramQueue);
        }
        /// <summary>
        /// Pop the expected parameters for a multiary logic operation (list (List<LogicExpression>)).
        /// </summary>
        public static bool PopMultiaryLogicParams(AbstractParser<TValue> parser, Queue<object> paramQueue)
        {
            return PopMultiaryParams_Internal<LogicExpression<TValue>>("Logic expression", parser, paramQueue);
        }
        /// <summary>
        /// Pop the expected parameter for a unary value operation (input (value expression)).
        /// </summary>
        public static bool PopUnaryValueParams(AbstractParser<TValue> parser, Queue<object> paramQueue)
        {
            return PopCountedValueParams_Internal(1, parser, paramQueue);
        }
        /// <summary>
        /// Pop the expected parameters for a binary value operation (leftHandValue (ValueExpression), rightHandValue (ValueExpression)).
        /// </summary>
        public static bool PopBinaryValueParams(AbstractParser<TValue> parser, Queue<object> paramQueue)
        {
            return PopCountedValueParams_Internal(2, parser, paramQueue);
        }
        /// <summary>
        /// Pop the expected parameters for a multiary value operation (list (List<ValueExpression>)).
        /// </summary>
        public static bool PopMultiaryValueParams(AbstractParser<TValue> parser, Queue<object> paramQueue)
        {
            return PopMultiaryParams_Internal<ValueExpression<TValue>>("Value expression", parser, paramQueue);
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
        public static object ConstructComparison(Queue<object> paramQueue, ComparisonType<TValue> comparisonType)
        {
            var left = (ValueExpression<TValue>)paramQueue.Dequeue();
            var right = (ValueExpression<TValue>)paramQueue.Dequeue();

            return new Comparison<TValue>(comparisonType, left, right);
        }
        /// <summary>
        /// Given a parameter queue, construct a conditional expression object.
        /// </summary>
        /// <param name="paramQueue">queue of parameters from PopParamsDelegate</param>
        /// <returns>new conditional expression object</returns>
        public static object ConstructConditional(Queue<object> paramQueue)
        {
            var condition = (LogicExpression<TValue>)paramQueue.Dequeue();
            var consequent = (ValueExpression<TValue>)paramQueue.Dequeue();
            var alternative = (ValueExpression<TValue>)paramQueue.Dequeue();

            return new ConditionalExpression<TValue>(condition, consequent, alternative);
        }
    }
}
