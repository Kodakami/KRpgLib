using KRpgLib.Stats;

namespace StatsUnitTests
{
    public class FakeStatTemplate : AbstractStatTemplate_Int
    {
        public FakeStatTemplate(int? min, int? max, int? precision, int defaultValue)
            : base(min, max, precision, defaultValue) { }
    }
}
