using System;
using System.Collections.Generic;
using KRpgLib.Flags;

namespace KRpgLib.Counters
{
    public class FlagComponent : CounterComponent, IFlagProvider
    {
        protected FlagCollection FlagCollection { get; }
        public FlagComponent(FlagCollection flagCollection)
        {
            FlagCollection = flagCollection != null ? new FlagCollection() : throw new ArgumentNullException(nameof(flagCollection));
        }
        public FlagCollection GetFlagCollection()
        {
            return FlagCollection;
        }
    }
}
