using KRpgLib.Stats;

namespace StatsUnitTests
{
    public class FakeStatTemplate : StatTemplate_Int
    {
        public FakeStatTemplate(int? min, int? max, int? precision, int defaultValue)
            : base(min, max, precision, defaultValue) { }
    }
}
