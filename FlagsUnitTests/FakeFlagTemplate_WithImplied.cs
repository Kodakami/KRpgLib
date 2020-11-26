using KRpgLib.Flags;
using System.Collections.Generic;

namespace FlagsUnitTests
{
    public class FakeFlagTemplate_WithImplied : IFlagTemplate
    {
        public static readonly FakeFlagTemplate ImpliedFlagTemplate = new FakeFlagTemplate();

        public int VariantCount => 1;

        public List<Flag> GetAllImpliedFlags()
        {
            return new List<Flag>() { Flag.Create(ImpliedFlagTemplate, 0) };
        }
    }
}
