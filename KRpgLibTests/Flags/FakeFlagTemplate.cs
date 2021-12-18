﻿using KRpgLib.Flags;
using System.Collections.Generic;

namespace KRpgLibTests.Flags
{
    public class FakeFlagTemplate : IFlagTemplate
    {
        public int VariantCount { get; }

        public FakeFlagTemplate(int variantCount = 1)
        {
            VariantCount = variantCount;
        }

        public IEnumerable<Flag> GetAllImpliedFlags()
        {
            return new List<Flag>();
        }
    }
}
