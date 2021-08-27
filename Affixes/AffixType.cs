using KRpgLib.Utility;
using System;
using System.Collections.Generic;

namespace KRpgLib.Affixes
{
    /// <summary>
    /// A type (or slot) for an affix. Helps determine which affixes are valid in certain cases. Think "Prefix mod" and "Suffix mod" in a standard ARPG.
    /// </summary>
    public sealed class AffixType
    {
        // Instance members.
        public int MaxAffixesOfType { get; }

        public AffixType(int maxAffixesOfType)
        {
            MaxAffixesOfType = maxAffixesOfType >= 0 ? maxAffixesOfType :
                throw new ArgumentOutOfRangeException(nameof(maxAffixesOfType), "Argument must be greater than or equal to 0.");
        }
    }
}
