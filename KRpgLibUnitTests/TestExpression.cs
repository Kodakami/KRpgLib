using KRpgLib.Stats;
using KRpgLib.Stats.Compound;

namespace KRpgLibUnitTests.Stats.Compound
{
    public class TestExpression : ValueExpression<int>
    {
        protected override int Evaluate_Internal(IStatSet<int> safeStatSet)
        {
            return 33;
        }
    }
}
