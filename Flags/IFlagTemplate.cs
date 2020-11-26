using System.Collections.Generic;

namespace KRpgLib.Flags
{
    public interface IFlagTemplate
    {
        int VariantCount { get; }
        List<Flag> GetAllImpliedFlags();
    }
}
