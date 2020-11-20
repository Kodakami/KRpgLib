using KRpgLib.Stats;

namespace KRpgLibUnitTests.Stats
{
    public class FakeStatTemplate : AbstractStatTemplate_Int
    {
        public FakeStatTemplate(int? min, int? max, int? precision, int defaultValue)
            : base(min, max, precision, defaultValue) { }
    }
}
