using System;
using System.Collections.Generic;
using System.Text;
using KRpgLib.Stats;
using KRpgLib.Stats.Compound;
using KRpgLib.Flags;

namespace KRpgLibTestEnvironment
{
    public class TestEnvironment
    {
        public StatManager _sm = new StatManager();
        public TestEnvironment()
        {
            PrintAllCoreStatValues();
        }
        public void PrintAllCoreStatValues()
        {
            new List<Stat>
            {
                Stat.Strength,
                Stat.Stamina,
                Stat.Dexterity,
                Stat.Agility,
                Stat.Intellect,
                Stat.Awareness,
                Stat.Fighting,
                Stat.Presence
            }.ForEach(x => PrintStatValue(x));

            new List<CompoundStat>
            {
                CompoundStat.Dodge,
                CompoundStat.Fortitude,
                CompoundStat.Parry,
                CompoundStat.Toughness,
                CompoundStat.Will,
                CompoundStat.Reach,
            }.ForEach(x => PrintCompoundStatValue(x));
        }
        public void PrintStatValue(Stat stat)
        {
            Console.WriteLine($"{stat.ExternalName}: {_sm.GetStatValue(stat)} (raw: {_sm.GetStatValue(stat, false)})");
        }
        public void PrintCompoundStatValue(CompoundStat compoundStat)
        {
            Console.WriteLine($"{compoundStat.ExternalName}: {_sm.GetCompoundStatValue(compoundStat)} (raw: {_sm.GetCompoundStatValue(compoundStat, false)})");
        }
    }
    public class CompoundStat : AbstractCompoundStatTemplate
    {
        public static CompoundStat Toughness = new CompoundStat("Toughness", -5, null, XplusY(Stat.Stamina, Stat.AdditionalToughness));
        public static CompoundStat Dodge = new CompoundStat("Dodge", -5, null, XplusY(Stat.Agility, Stat.AdditionalDodge));
        public static CompoundStat Fortitude = new CompoundStat("Fortitude", -5, null, XplusY(Stat.Stamina, Stat.AdditionalFortitude));
        public static CompoundStat Parry = new CompoundStat("Parry", -5, null, XplusY(Stat.Fighting, Stat.AdditionalParry));
        public static CompoundStat Will = new CompoundStat("Will", -5, null, XplusY(Stat.Awareness, Stat.AdditionalWill));

        public static CompoundStat Reach = new CompoundStat("Reach", null, null, XplusY(Stat.Size, Stat.AdditionalReach));

        // Instance members.
        public string ExternalName { get; }
        protected CompoundStat(string externalName, float? min, float? max, CompoundStatAlgorithm algorithm)
            :base(min, max, 1, algorithm)
        {
            ExternalName = externalName;
        }
        private static CompoundStatAlgorithm XplusY(Stat baseStat, Stat addedStat)
        {
            AlgoBuilder ab = new AlgoBuilder();
            ab.SetTo(baseStat);
            ab.Add(addedStat);
            return ab.Build();
        }
    }
    public sealed class StatProvider : IStatProvider
    {
        private class StatDeltaSpec : System.Tuple<IStatTemplate, int> {
            public StatDeltaSpec(IStatTemplate stat, int deltaValue) : base(stat, deltaValue) { }
        }

        // TODO: Add StatProviders.



        // Instance members.
        public event Action<IStatProvider, IStatTemplate> OnStatDeltasChanged;

        private readonly Dictionary<IStatTemplate, List<StatDelta>> _statsProvided;

        private StatProvider(params StatDeltaSpec[] deltas)
        {
            _statsProvided = new Dictionary<IStatTemplate, List<StatDelta>>();

            foreach (var delta in deltas)
            {
                AddDelta(delta.Item1, delta.Item2);
            }
        }

        private void AddDelta(IStatTemplate stat, int delta)
        {
            if (!_statsProvided.ContainsKey(stat))
            {
                _statsProvided.Add(stat, new List<StatDelta>());
            }

            _statsProvided[stat].Add(new StatDelta(delta, StatDeltaType.Addition));
        }

        public List<StatDelta> GetStatDeltasForStat(IStatTemplate stat)
        {
            if (_statsProvided.ContainsKey(stat))
            {
                return _statsProvided[stat];
            }
            return new List<StatDelta>();
        }

        public List<IStatTemplate> GetStatsWithDeltas()
        {
            return new List<IStatTemplate>(_statsProvided.Keys);
        }

        public bool HasDeltasForStat(IStatTemplate stat)
        {
            return _statsProvided.ContainsKey(stat);
        }
    }
}
