using KRpgLib.Stats;

namespace KRpgLibTests.Stats
{
    public class FakeStat : Stat
    {
        public FakeStat(int defaultValue, int? min, int? max, int? precision)
            : base(defaultValue, min, max, precision) { }
    }
}
