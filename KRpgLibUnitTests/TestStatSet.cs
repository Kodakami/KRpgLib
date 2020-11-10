using System;
using System.Collections.Generic;
using System.Text;
using KRpgLib.Stats;
using KRpgLib.Stats.Compound;

namespace KRpgLibUnitTests.Stats
{
    public class TestStatSet : AbstractStatSet<int>
    {
        /// <summary>
        /// 13 (12 legal)
        /// </summary>
        public TestStatTemplate TestStat1 = new TestStatTemplate("Test Stat 1", 0, 100, 2, 0);
        /// <summary>
        /// 43 (42 legal)
        /// </summary>
        public TestStatTemplate TestStat2 = new TestStatTemplate("Test Stat 2", 0, 100, 2, 0);
        /// <summary>
        /// 79 (78 legal)
        /// </summary>
        public TestStatTemplate TestStat3 = new TestStatTemplate("Test Stat 3", 0, 100, 2, 0);

        private readonly Dictionary<IStatTemplate<int>, int> _dict = new Dictionary<IStatTemplate<int>, int>();

        public TestStatSet()
        {
            _dict.Add(TestStat1, 13);
            _dict.Add(TestStat2, 43);
            _dict.Add(TestStat3, 79);
        }

        protected override int GetCompoundStatValue_Internal(ICompoundStatTemplate<int> safeCompoundStatTemplate)
        {
            return safeCompoundStatTemplate.CalculateValue(this);
        }

        protected override int GetStatValue_Internal(IStatTemplate<int> safeStatTemplate)
        {
            if (_dict.ContainsKey(safeStatTemplate))
            {
                return _dict[safeStatTemplate];
            }
            return safeStatTemplate.DefaultValue;
        }
    }
}
