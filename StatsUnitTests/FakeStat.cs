using KRpgLib.Stats;

namespace KRpgLib.UnitTests.StatsTests
{
    public class FakeStat : Stat_Int
    {
        public FakeStat(int? min, int? max, int? precision, int defaultValue)
            : base(min, max, precision, defaultValue) { }
    }
}
