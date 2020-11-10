using KRpgLib.Stats.Compound;

namespace KRpgLibUnitTests.Stats.Compound
{
    public class TestCompoundStat : AbstractCompoundStatTemplate_Int
    {
        public string ExternalName { get; }
        public TestCompoundStat(string externalName, int? min, int? max, int? precision, CompoundStatAlgorithm<int> algorithm)
            :base(min, max, precision, algorithm)
        {
            ExternalName = externalName;
        }
    }
}
