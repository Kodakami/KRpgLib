using KRpgLib.Stats;
using KRpgLib.Stats.Compound;
using System.Collections.Generic;

namespace KRpgLib.UnitTests.StatsTests
{
    public class FakeStatSet
    {
        public static readonly FakeStatTemplate TestStat_Raw3_Legal2_Provided = new FakeStatTemplate(null, null, 2, 0);
        public static readonly FakeStatTemplate TestStat_Default7_Legal6_Missing = new FakeStatTemplate(null, null, 2, 7);

        public StatSnapshot<int> Snapshot { get; }

        public FakeStatSet()
        {
            Snapshot = StatSnapshot<int>.Create(new Dictionary<IStatTemplate<int>, int>() { { TestStat_Raw3_Legal2_Provided, 3 } });
        }
    }
}
