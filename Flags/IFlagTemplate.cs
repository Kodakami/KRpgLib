using System.Collections.Generic;

namespace KRpgLib.Flags
{
    public interface IFlagTemplate
    {
        int VariantCount { get; }
        IEnumerable<Flag> GetAllImpliedFlags();
    }
}
