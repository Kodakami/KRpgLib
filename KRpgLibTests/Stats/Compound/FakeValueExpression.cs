using KRpgLib.Stats;
using KRpgLib.Stats.Compound;

namespace KRpgLibTests.Stats.Compound
{
    public class FakeValueExpression : ValueExpression
    {
        private readonly int _literal;
        public FakeValueExpression(int literal)
        {
            _literal = literal;
        }
        protected override int Evaluate_Internal(IStatSet safeStatSet)
        {
            return _literal;
        }
    }
}
