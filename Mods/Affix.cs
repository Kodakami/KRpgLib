using KRpgLib.Utility;
using KRpgLib.Stats;
using KRpgLib.Flags;
using System;
using System.Collections.Generic;
using KRpgLib.Mods.ModTemplates;

namespace KRpgLib.Mods
{
    public struct Affix<TValue> : INamedObject, IFlagProvider, IStatProvider<TValue> where TValue : struct
    {
        // Private instance members.

        // Rolled mod instances that are part of the affix.
        private readonly List<ModFlag> _flagMods;
        private readonly List<ModStatDelta<TValue>> _statDeltaMods;

        // Cached values for the combined results of the mod instances.
        private readonly List<Flag> _flagCache;
        private readonly StatDeltaCollection<TValue> _statDeltaCache;

        // Public properties (and INamedObject compliance).
        public AffixTemplate<TValue> Template { get; }
        public string ExternalName => Template.ExternalName;

        // Constructor.
        public Affix(AffixTemplate<TValue> template, List<ModFlag> flagMods, List<ModStatDelta<TValue>> statDeltaMods)
        {
            Template = template ?? throw new ArgumentNullException(nameof(flagMods));

            _flagMods = flagMods ?? new List<ModFlag>();
            _statDeltaMods = statDeltaMods ?? new List<ModStatDelta<TValue>>();

            _flagCache = CreateFlagCache(flagMods);
            _statDeltaCache = CreateStatDeltaCache(statDeltaMods);
        }

        // Public methods (for viewing the mods themselves while keeping them read-only).
        public List<ModFlag> GetFlagModsCopy() => new List<ModFlag>(_flagMods);
        public List<ModStatDelta<TValue>> GetStatDeltaModsCopy() => new List<ModStatDelta<TValue>>(_statDeltaMods);

        // IFlagProvider compliance.
        public List<Flag> GetAllFlags() => new List<Flag>(_flagCache);

        // IStatProvider<TValue> compliance.
        public List<StatDelta<TValue>> GetDeltasForStat(IStatTemplate<TValue> stat) => _statDeltaCache.GetDeltasForStat(stat);
        public List<IStatTemplate<TValue>> GetStatsWithDeltas() => _statDeltaCache.GetStatsWithDeltas();
        public bool HasDeltasForStat(IStatTemplate<TValue> stat) => _statDeltaCache.HasDeltasForStat(stat);

        // Private static methods (for calculating cached values).
        private static List<Flag> CreateFlagCache(List<ModFlag> flagMods)
        {
            return flagMods.ConvertAll(mod => mod.RolledResult);
        }
        private static StatDeltaCollection<TValue> CreateStatDeltaCache(List<ModStatDelta<TValue>> statDeltaMods)
        {
            var newCollection = new StatDeltaCollection<TValue>();
            foreach (var mod in statDeltaMods)
            {
                newCollection.Add(new StatTemplateAndDelta<TValue>(
                    mod.Template.AffectedStatTemplate,
                    mod.Template.StatDeltaType,
                    mod.RolledResult));
            }
            return newCollection;
        }
    }
}
