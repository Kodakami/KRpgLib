using System.Collections.Generic;
using KRpgLib.Stats;
using KRpgLib.Flags;

namespace KRpgLib.Affixes
{
    public static class AffixUtilities
    {
        public static StatDeltaModTemplate CreateStatModTemplate(Stat stat, DeltaType deltaType, int minRoll, int maxRoll)
        {
            return new StatDeltaModTemplate(new StatDeltaValueBounds(stat, deltaType, minRoll, maxRoll));
        }
        public static FlagModTemplate CreateFlagModTemplate(Flag flag)
        {
            return new FlagModTemplate(new List<Flag>() { flag });
        }
    }
}
