using KRpgLib.Stats;

namespace KRpgLibUnitTests.Stats
{
    public class TestStatTemplate : AbstractStatTemplate_Int
    {
        // Instance members.
        public string ExternalName { get; }

        public TestStatTemplate(string externalName, int? min, int? max, int? precision, int defaultValue)
            : base(min, max, precision, defaultValue)
        {
            ExternalName = externalName;
        }
    }
}
