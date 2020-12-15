using KRpgLib.Flags;
using KRpgLib.Stats;
using System;

namespace KRpgLib.Investments
{
    public struct InvestmentValue<TValue> : IFlagProvider, IStatProvider<TValue> where TValue : struct
    {
        private readonly FlagCollection _flags;
        private readonly StatDeltaCollection<TValue> _statDeltas;

        public InvestmentValue(FlagCollection flags, StatDeltaCollection<TValue> statDeltas)
        {
            _flags = flags ?? throw new ArgumentNullException(nameof(flags));
            _statDeltas = statDeltas ?? throw new ArgumentNullException(nameof(statDeltas));
        }

        public FlagCollection GetFlagCollection()
        {
            return _flags;
        }
        public StatDeltaCollection<TValue> GetStatDeltaCollection()
        {
            return _statDeltas;
        }
    }
}
