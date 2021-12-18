using KRpgLib.Utility.TemplateObject;
using System.Collections.Generic;
using KRpgLib.Utility;
using System.Linq;

namespace KRpgLib.Flags
{
    public interface IFlagTemplate : ITemplate
    {
        int VariantCount { get; }
        IEnumerable<Flag> GetAllImpliedFlags();
    }
    /// <summary>
    /// Default implementation.
    /// </summary>
    public sealed class FlagTemplate : IFlagTemplate
    {
        private readonly Flag[] _allImpliedFlags;
        public int VariantCount { get; }

        public FlagTemplate(int variantCount, IEnumerable<Flag> allImpliedFlags)
        {
            if (variantCount <= 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(variantCount));
            }

            VariantCount = variantCount;

            _allImpliedFlags = allImpliedFlags.ToArray();   // Makes a copy.
        }

        public IEnumerable<Flag> GetAllImpliedFlags()
        {
            return _allImpliedFlags;
        }
    }
}
