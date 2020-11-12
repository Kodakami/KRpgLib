using System.Collections.Generic;

namespace KRpgLib.Stats.Compound
{
    public sealed class CompoundStatAlgorithm<TValue> where TValue : struct
    {
        private readonly IExpression<TValue> _expression;

        public CompoundStatAlgorithm(IExpression<TValue> expression)
        {
            _expression = expression ?? throw new System.ArgumentNullException(nameof(expression));
        }
        public TValue CalculateValue(IStatSet<TValue> statSet)
        {
            if (statSet == null)
            {
                throw new System.ArgumentNullException(nameof(statSet));
            }

            return _expression.Evaluate(statSet);
        }
    }

    // AlgoBuilder v2 design
    /*
    ab.RegisterStat("WIS", MyStatTemplate.Wisdom");
    ab.RegisterStat("STR", MyStatTemplate.Strength");
    ab.RegisterStat("AGI", MyStatTemplate.Agility");
    ab.RegisterStat("DEF", MyStatTemplate.Defense");
    ab.RegisterStat("MDEF", MyStatTemplate.MagicDefense");

    ab.DefineValue("myPiExp", "3.1416")
        would be
        new Literal<float>(3.1416);
    ab.DefineValue("highestMainStat", "(max *STR *WIS *AGI)")
        



    // TODO: Code the new AlgoBuilder.



    ab.DefineBool("defenseIs3TimesMdef", "(eq *DEF (mul 3 *MDEF))")
            
    var returnCode = ab.TryParse("(mul myPiExp (? (neq highestMainStat 69) highestMainStat *AGI))", out CompoundStatAlgorithm algo);
    if (returnCode == AlgoBuilder.ReturnCode.PARSE_OK)
    {
        // Do stuff with algo object.
    }
    // throw exception or handle based on return code.

    //expression handles for float and int would be:
    [numeric literal] (starting with just a number, no decimal)
    $statName [literal raw stat value], *statName [literal legalized stat value]
    add, sub, mul, min, max [2 or more value expression params, return value] (associative)
    div, pow, mod [2 value expression params, return value] (not associative)
    neg [1 value expression param, return value]
    eq, neq [2 or more value expression params, return bool] (all are equal, not all are equal)
    gt, lt, gteq, lteq [2 value expression params, return bool]
    ? [1 logical expression param, 2 value expression params, return value] (conditional, ternary, if-then-else)
    and, or, xor [2 or more logical expression params, return bool] (all AND, all OR, all XOR)
    not [1 logical expression param, return bool]
    //anything else would be a defined macro name or an error
    //a macro name must start with a letter and must not contain spaces. is case sensitive.
    */
}
