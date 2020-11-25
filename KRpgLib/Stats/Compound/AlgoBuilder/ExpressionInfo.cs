using System.Collections.Generic;

namespace KRpgLib.Stats.Compound.AlgoBuilder
{
    // Internal class ExpressionRegistry uses to build individual expressions from an AbstractParser's stack.
    internal sealed class ExpressionInfo<TValue> where TValue : struct
    {
        private readonly PopParamsDelegate<TValue> _pop;
        private readonly ExpressionCtorDelegate<TValue> _expCtor;

        public List<string> Keywords { get; }

        public ExpressionInfo(List<string> keywords, PopParamsDelegate<TValue> popDelegate, ExpressionCtorDelegate<TValue> expCtor)
        {
            Keywords = keywords;
            _pop = popDelegate;
            _expCtor = expCtor;
        }

        public bool TryBuildAndPushExpression(AbstractParser<TValue> parser)
        {
            var paramQueue = new Queue<object>();
            if (_pop(parser, paramQueue))
            {
                // Consume the NULL on the stack _-=: B E F O R E :=-_ you push the result.
                if (parser.TryConsumeEndExpression())
                {
                    parser.DoPushObject(_expCtor.Invoke(paramQueue));
                    return true;
                }
            }
            return false;
        }
    }
}
