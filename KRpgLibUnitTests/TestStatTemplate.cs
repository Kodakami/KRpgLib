using System;
using System.Collections.Generic;
using System.Text;
using KRpgLib.Stats;

namespace KRpgLibUnitTests.Stats
{
    public class TestStatTemplate : AbstractStatTemplate
    {
        // Instance members.
        public string ExternalName { get; }

        public TestStatTemplate(string externalName, float? min, float? max, float? precision, float defaultValue)
            : base(min, max, precision, defaultValue)
        {
            ExternalName = externalName;
        }
    }
}
