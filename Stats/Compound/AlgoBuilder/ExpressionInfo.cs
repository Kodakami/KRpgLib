using System.Collections.Generic;

namespace KRpgLib.Stats.Compound.AlgoBuilder
{
    // Internal class ExpressionRegistry uses to build individual expressions from an AbstractParser's stack.
    internal sealed class ExpressionInfo
    {
        private readonly PopParamsFunc _pop;
        private readonly ExpressionCtorDelegate _expCtor;

        public IEnumerable<string> Keywords { get; }

        public ExpressionInfo(IEnumerable<string> keywords, PopParamsFunc popFunc, ExpressionCtorDelegate expCtor)
        {
            Keywords = keywords;
            _pop = popFunc;
            _expCtor = expCtor;
        }

        public bool TryBuildAndPushExpression(Parser parser)
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
