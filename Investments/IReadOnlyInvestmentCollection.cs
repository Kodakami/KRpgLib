using System;
using System.Collections.Generic;

namespace KRpgLib.Investments
{
    // Interface for passing an investment collection and leaving the client code unable to modify its contents.
    public interface IReadOnlyInvestmentCollection<TInvestment, TValue> : IReadOnlyCollection<TInvestment>
        where TInvestment : AbstractInvestment<TValue>
        // where TValue is anything
    {
        // Is there an investment where predicate is true?
        bool HasInvestmentWhere(Predicate<TInvestment> predicate);

        // Get the cached combined value of all investments in the collection.
        TValue GetCombinedInvestmentValue();
    }
}
