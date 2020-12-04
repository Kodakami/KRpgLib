using System;
using System.Collections.Generic;
using KRpgLib.Flags;

namespace KRpgLib.Counters
{
    public struct FlagInfo : IFlagProvider
    {
        public List<Flag> Flags { get; }
        public FlagInfo(List<Flag> flags)
        {
            Flags = flags != null ? new List<Flag>(flags) : throw new ArgumentNullException(nameof(flags));
        }
        List<Flag> IFlagProvider.GetAllFlags()
        {
            return Flags;
        }
    }
}
