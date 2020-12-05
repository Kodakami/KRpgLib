using System;
using System.Collections.Generic;
using KRpgLib.Flags;

namespace KRpgLib.Counters
{
    public class FlagComponent : CounterComponent, IFlagProvider
    {
        public List<Flag> Flags { get; }
        public FlagComponent(List<Flag> flags)
        {
            Flags = flags != null ? new List<Flag>(flags) : throw new ArgumentNullException(nameof(flags));
        }
        public List<Flag> GetAllFlags()
        {
            return Flags;
        }
    }
}
