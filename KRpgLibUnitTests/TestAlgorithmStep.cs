using KRpgLib.Stats.Compound;
using KRpgLib.Stats;

namespace KRpgLibUnitTests.Stats.Compound
{
    public class TestAlgorithmStep : IAlgorithmStep
    {
        // Increment.
        public float Apply(IStatSet statSet, float currentValue)
        {
            return currentValue + 1;
        }
    }
}
