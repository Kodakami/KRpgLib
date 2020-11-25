using KRpgLib.Stats;
using KRpgLib.Stats.Compound;
using System.Collections.Generic;

namespace KRpgLibUnitTests.Stats
{
    public class FakeStatSet : AbstractStatSet<int>
    {
        public static readonly FakeStatTemplate TestStat_Raw3_Legal2_Provided = new FakeStatTemplate(null, null, 2, 0);
        public static readonly FakeStatTemplate TestStat_Default7_Legal6_Missing = new FakeStatTemplate(null, null, 2, 7);

        private readonly Dictionary<IStatTemplate<int>, int> _dict = new Dictionary<IStatTemplate<int>, int>();

        public FakeStatSet()
        {
            _dict.Add(TestStat_Raw3_Legal2_Provided, 3);
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
