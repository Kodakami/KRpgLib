using KRpgLib.Stats.Compound;
using KRpgLib.Stats;

namespace KRpgLibUnitTests.Stats.Compound
{
    public class TestExpression : IExpression<int>
    {
        public int Evaluate(IStatSet<int> forStatSet)
        {
            return 33;
        }
    }
}
