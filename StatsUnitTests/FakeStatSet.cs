using KRpgLib.Stats;
using KRpgLib.Stats.Compound;
using System.Collections.Generic;

namespace KRpgLib.UnitTests.StatsTests
{
    public class FakeStatSet
    {
        public static readonly FakeStat TestStat_Raw3_Legal2_Provided = new FakeStat(null, null, 2, 0);
        public static readonly FakeStat TestStat_Default7_Legal6_Missing = new FakeStat(null, null, 2, 7);

        public StatSnapshot<int> Snapshot { get; }

        public FakeStatSet()
        {
            Snapshot = StatSnapshot<int>.Create(new Dictionary<IStat<int>, int>() { { TestStat_Raw3_Legal2_Provided, 3 } });
        }
    }
}
