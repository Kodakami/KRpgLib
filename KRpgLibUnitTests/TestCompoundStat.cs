using KRpgLib.Stats.Compound;

namespace KRpgLibUnitTests.Stats.Compound
{
    public class TestCompoundStat : AbstractCompoundStatTemplate
    {
        public string ExternalName { get; }
        public TestCompoundStat(string externalName, float? min, float? max, float? precision, CompoundStatAlgorithm algorithm)
            :base(min, max, precision, algorithm)
        {
            ExternalName = externalName;
        }
    }
}
