using KRpgLib.Stats.Compound;

namespace KRpgLibTests.Stats.Compound
{
    public class FakeCompoundStat : CompoundStat
    {
        public FakeCompoundStat(int? min, int? max, int? precision, CompoundStatAlgorithm algorithm)
            : base(min, max, precision, algorithm) { }
    }
}
