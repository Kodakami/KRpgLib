using KRpgLib.Stats;
using KRpgLib.Stats.Compound;
using System.Collections.Generic;

namespace KRpgLibTests.Stats
{
    public class FakeStatSet
    {
        public static readonly FakeStat TestStat_Raw3_Legal2_Provided = new(0, null, null, 2);
        public static readonly FakeStat TestStat_Default7_Legal6_Missing = new(7, null, null, 2);

        public StatSnapshot Snapshot { get; }

        public FakeStatSet()
        {
            Snapshot = new StatSnapshot(new Dictionary<Stat, int>() { { TestStat_Raw3_Legal2_Provided, 3 } });
        }
    }
}
