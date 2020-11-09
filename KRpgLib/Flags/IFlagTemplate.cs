using System;
using System.Collections.Generic;
using System.Text;

namespace KRpgLib.Flags
{
    public interface IFlagTemplate
    {
        int VariantCount { get; }
        List<Flag> GetAllImpliedFlags();
    }
}
