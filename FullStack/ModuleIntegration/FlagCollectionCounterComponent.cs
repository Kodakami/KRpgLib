using System;
using KRpgLib.Flags;

namespace KRpgLib.Counters
{
    public class FlagCollectionCounterComponent : CounterComponent, IFlagProvider
    {
        protected IReadOnlyFlagCollection FlagCollection { get; }
        public FlagCollectionCounterComponent(IReadOnlyFlagCollection flagCollection)
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
