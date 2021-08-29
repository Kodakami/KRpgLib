using System;
using System.Collections.Generic;
using KRpgLib.Flags;

namespace KRpgLib.Counters
{
    public class FlagComponent : CounterComponent, IFlagProvider
    {
        protected IReadOnlyFlagCollection FlagCollection { get; }
        public FlagComponent(IReadOnlyFlagCollection flagCollection)
        {
            // Might need to make a new collection, currently unsure.
            FlagCollection = flagCollection ?? throw new ArgumentNullException(nameof(flagCollection));
        }
        public IReadOnlyFlagCollection GetFlagCollection()
        {
            return FlagCollection;
        }
    }
}
