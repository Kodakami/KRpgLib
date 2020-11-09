using KRpgLib.Stats.Compound;
using KRpgLib.Stats;

namespace KRpgLibUnitTests.Stats.Compound
{
    public class TestExpression : IExpression
    {
        public float Evaluate(IStatSet forStatSet)
        {
            return 33;
        }
    }
}
