using System;
using System.Collections.Generic;
using System.Text;
using KRpgLib.Stats;
using KRpgLib.Stats.Compound;

namespace KRpgLib.UnitTests.StatsTests.Compound
{
    public class FakeLogicExpression : LogicExpression<int>
    {
        private readonly bool _literal;
        public FakeLogicExpression(bool literal)
        {
            _literal = literal;
        }

        protected override bool Evaluate_Internal(IStatSet<int> safeStatSet)
        {
            return _literal;
        }
    }
}
