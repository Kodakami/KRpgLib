using KRpgLib.Stats;

namespace KRpgLib.UnitTests.StatsTests
{
    public class FakeStatTemplate : StatTemplate_Int
    {
        public FakeStatTemplate(int? min, int? max, int? precision, int defaultValue)
            : base(min, max, precision, defaultValue) { }
    }
}
