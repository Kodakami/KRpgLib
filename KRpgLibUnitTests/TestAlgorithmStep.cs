using KRpgLib.Stats.Compound;
using KRpgLib.Stats;

namespace KRpgLibUnitTests.Stats.Compound
{
    public class TestAlgorithmStep : IAlgorithmStep<int>
    {
        // Increment.
        public int Apply(IStatSet<int> statSet, int currentValue)
        {
            return currentValue + 23;
        }
    }
}
