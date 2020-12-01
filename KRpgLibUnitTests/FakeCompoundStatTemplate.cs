﻿using KRpgLib.Stats.Compound;

namespace StatsUnitTests.Compound
{
    public class FakeCompoundStatTemplate : AbstractCompoundStatTemplate_Int
    {
        public FakeCompoundStatTemplate(int? min, int? max, int? precision, CompoundStatAlgorithm<int> algorithm)
            :base(min, max, precision, algorithm) { }
    }
}