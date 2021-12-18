﻿using System;
using System.Collections.Generic;
using System.Text;
using KRpgLib.Stats;
using KRpgLib.Stats.Compound;

namespace KRpgLibTests.Stats.Compound
{
    public class FakeLogicExpression : LogicExpression
    {
        private readonly bool _literal;
        public FakeLogicExpression(bool literal)
        {
            _literal = literal;
        }

        protected override bool Evaluate_Internal(IStatSet safeStatSet)
        {
            return _literal;
        }
    }
}
