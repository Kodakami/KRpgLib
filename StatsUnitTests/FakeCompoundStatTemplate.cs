﻿using KRpgLib.Stats.Compound;

namespace KRpgLib.UnitTests.StatsTests.Compound
{
    public class FakeCompoundStatTemplate : CompoundStat_Int
    {
        public FakeCompoundStatTemplate(int? min, int? max, int? precision, CompoundStatAlgorithm<int> algorithm)
            :base(min, max, precision, algorithm) { }
    }
}
