using System;
using System.Collections.Generic;
using System.Text;
using KRpgLib.Stats;
using KRpgLib.Stats.Compound;

namespace KRpgLibUnitTests.Stats
{
    public class TestStatSet : AbstractStatSet
    {
        public TestStatTemplate TestStat1 = new TestStatTemplate("Test Stat 1", 0, 100, 1, 0);
        public TestStatTemplate TestStat2 = new TestStatTemplate("Test Stat 2", 0, 100, 1, 0);
        public TestStatTemplate TestStat3 = new TestStatTemplate("Test Stat 3", 0, 100, 1, 0);

        private readonly Dictionary<IStatTemplate, float> _dict = new Dictionary<IStatTemplate, float>();

        public TestStatSet()
        {
            _dict.Add(TestStat1, 15.1f);
            _dict.Add(TestStat2, 42.1f);
            _dict.Add(TestStat3, 79.1f);
        }

        protected override float GetCompoundStatValue_Internal(ICompoundStatTemplate safeCompoundStatTemplate)
        {
            return safeCompoundStatTemplate.Algorithm.CalculateValue(this);
        }

        protected override float GetStatValue_Internal(IStatTemplate safeStatTemplate)
        {
            if (_dict.ContainsKey(safeStatTemplate))
            {
                return _dict[safeStatTemplate];
            }
            return safeStatTemplate.DefaultValue;
        }
    }
}
