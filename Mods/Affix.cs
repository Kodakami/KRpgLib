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

        // Cached values for the combined results of the mod instances. No need for cached value controllers, values are static.
        private readonly List<Flag> _flagCache;
        private readonly StatDeltaCollection<TValue> _statDeltaCollectionCache;

        // Public properties (and INamedObject compliance).
        public AffixTemplate<TValue> Template { get; }
        public string ExternalName => Template.ExternalName;

        // Constructor.
        private Affix(AffixTemplate<TValue> template, List<ModFlag> flagMods, List<ModStatDelta<TValue>> statDeltaMods)
        {
            Template = template;
            _flagMods = flagMods;
            _statDeltaMods = statDeltaMods;

            // Create flag cache.
            _flagCache = _flagMods.ConvertAll(mod => mod.RolledResult);

            // Create stat delta collection cache.
            _statDeltaCollectionCache = new StatDeltaCollection<TValue>(
                _statDeltaMods.ConvertAll(mod => new StatTemplateAndDelta<TValue>(
                    mod.Template.AffectedStatTemplate,
                    mod.Template.StatDeltaType,
                    mod.RolledResult)
                ));
        }

        // Public methods (for viewing the mods themselves while keeping them read-only).
        public bool HasFlagMods => _flagMods.Count > 0;
        public List<ModFlag> GetFlagModsCopy() => new List<ModFlag>(_flagMods);
        public bool HasStatDeltaMods => _statDeltaMods.Count > 0;
        public List<ModStatDelta<TValue>> GetStatDeltaModsCopy() => new List<ModStatDelta<TValue>>(_statDeltaMods);

        // IFlagProvider compliance.
        public List<Flag> GetAllFlags() => new List<Flag>(_flagCache);

        // IStatProvider<TValue> compliance.
        public StatDeltaCollection<TValue> GetStatDeltaCollection() => _statDeltaCollectionCache;

        // Static create method.
        public static Affix<TValue> Create(AffixTemplate<TValue> template, List<ModFlag> flagMods, List<ModStatDelta<TValue>> statDeltaMods)
        {
            return new Affix<TValue>(
                template ?? throw new ArgumentNullException(nameof(flagMods)),
                flagMods ?? new List<ModFlag>(),
                statDeltaMods ?? new List<ModStatDelta<TValue>>()
                );
        }
    }
}
