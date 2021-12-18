using System;
using System.Collections.Generic;

namespace KRpgLib.Investments
{
    public interface IReadOnlyInvestmentCollection<TInvestmentTemplate, TInvestment, TInvestmentValue> : IEnumerable<TInvestment>
        where TInvestmentTemplate : IInvestmentTemplate<TInvestmentValue>
        where TInvestment : AbstractInvestment<TInvestmentTemplate, TInvestmentValue>
        where TInvestmentValue : IInvestmentValue
    {
        bool HasInvestment(TInvestmentTemplate template);
        bool HasInvestmentWhere(Predicate<TInvestment> predicate);
        TInvestmentValue GetCombinedInvestmentValue();
    }
}
