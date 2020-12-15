using System;
using System.Linq;
using KRpgLib.Flags;
using KRpgLib.Stats;

namespace KRpgLib.Investments
{
    // TODO: Consider refactoring Investment system to use Komponent system.

    public interface IInvestmentTemplate<TValue> : IFlagProvider, IStatProvider<TValue> where TValue : struct { }
    public class Investment<TValue> : IFlagProvider, IStatProvider<TValue> where TValue : struct
    {
        public IInvestmentTemplate<TValue> Template { get; }
        public InvestmentValue<TValue> Value { get; }

        public Investment(IInvestmentTemplate<TValue> template)
        {
            Template = template ?? throw new ArgumentNullException(nameof(template));
            Value = new InvestmentValue<TValue>(Template.GetFlagCollection(), Template.GetStatDeltaCollection());
        }

        public virtual StatDeltaCollection<TValue> GetStatDeltaCollection()
        {
            return Value.GetStatDeltaCollection();
        }
        public FlagCollection GetFlagCollection()
        {
            return Value.GetFlagCollection();
        }
    }
}
