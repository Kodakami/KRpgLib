using System;
using System.Collections.Generic;
using System.Text;

namespace KRpgLib.Stats.Compound.AlgoBuilder
{
    // TODO: Remove class and reset to library.
    public static class Program
    {
        public static void Main(string[] args)
        {
            var statSet = new StatSnapshot<int>(new Dictionary<IStatTemplate<int>, int>() { { Stat.Strength, 5 }, { Stat.Wisdom, -10} });
            var ab = new AlgoBuilder_Int();
            ab.RegisterStat("str", Stat.Strength);
            ab.RegisterStat("wis", Stat.Wisdom);

            foreach (var script in new List<string>()
            {
                "(if (leq (max 25 15) 22) 0 (add (mul 2 3 (pow 2 4)) 1 4 4))",
                "(if (leq (max 25 15) 22) 0 (add (mul 2 3 (pow 2 4)) 1 4 4)",
                "(if (leq (max 25 15) 22) 0 (add (mul 2 3 (pow 2 4)) 1 4 4)))",
                "(if (leq (max 25 15) 22) 0 (add (mul 2 3 (pow 2 4)) 1 4 4)>",
                "if (leq (max 25 15) 22) 0 (add (mul 2 3 (pow 2 4)) 1 4 4))",
                "3",
                "*wis",
                "$str",
                "(add *str *wis)",
                "(add *str **wis)",
                "*agi",
                "(bsl 0 1)",
                "(add 3 mul 2 5)",
            })
            {
                ProcessAndPrintResult(ab, statSet, script);
            }
        }
        public static void ProcessAndPrintResult<TValue>(AlgoBuilder<TValue> ab, IStatSet<TValue> statSet, string script) where TValue : struct
        {
            if (!ab.TryBuild(script, out CompoundStatAlgorithm<TValue> csa))
            {
                Console.WriteLine($"Script: \"{script}\"\n\tFailed: {ab.StatusMessage}");
                return;
            }
            var result = csa.CalculateValue(statSet);
            Console.WriteLine($"Script: \"{script}\"\n\tResult: {result}");
        }
    }
    public class Stat : AbstractStatTemplate_Int
    {
        public static readonly Stat Strength = new Stat(0, null, null, 0);
        public static readonly Stat Wisdom = new Stat(1, 99, null, 1);

        public Stat(int? min, int? max, int? precision, int defaultValue)
            :base(min, max, precision, defaultValue) { }
    }
}
