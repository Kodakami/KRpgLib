using KRpgLib.Stats;
using KRpgLib.Stats.Compound;

namespace StatsUnitTests.Compound
{
    public class FakeValueExpression : ValueExpression<int>
    {
        private readonly int _literal;
        public FakeValueExpression(int literal)
        {
            _literal = literal;
        }
        protected override int Evaluate_Internal(IStatSet<int> safeStatSet)
        {
            return _literal;
        }
    }
}
