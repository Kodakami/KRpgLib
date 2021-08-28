using KRpgLib.Utility.TemplateObject;
using System.Collections.Generic;

namespace KRpgLib.Flags
{
    public interface IFlagTemplate : ITemplate
    {
        int VariantCount { get; }
        IEnumerable<Flag> GetAllImpliedFlags();
    }
}
