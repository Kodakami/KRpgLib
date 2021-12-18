using System;
using System.Linq;
using System.Collections.Generic;
using KRpgLib.Utility.TemplateObject;
using KRpgLib.Utility;

namespace KRpgLib.Affixes
{
    /// <summary>
    /// An instance of an affix (group of connected mods) which will modify values associated with an IModdable. Points to an AffixTemplate and holds mod instances.
    /// </summary>
    public sealed class Affix : ITemplateObject<AffixTemplate>
    {
        private readonly List<Mod> _modInstances;
        private readonly ModEffectCacheHelper _effectCacheHelper;

        public AffixTemplate Template { get; }

        public Affix(AffixTemplate template, IEnumerable<Mod> modInstances)
        {
            Template = template;

            _modInstances = new List<Mod>(modInstances);
            _effectCacheHelper = new ModEffectCacheHelper(this);
        }
        /// <summary>
        /// Roll or reroll all mods in the Affix. Will return true if anything changed.
        /// </summary>
        public bool RerollAllMods(Random rng)
        {
            // We don't use LINQ's Any() method to ensure against short-circuiting behavior.

            bool changeOccurred = false;

            // For each mod instance in the affix.
            foreach (var mod in _modInstances)
            {
                // Tell it to reroll. Will return true if any change occurred.
                var modChanged = mod.RollNewArg(rng);

                // Change occurred if any prior change occurred or one occurred now.
                changeOccurred = changeOccurred || modChanged;
            }

            if (changeOccurred)
            {
                _effectCacheHelper.SetDirty_FromExternal();
            }

            return changeOccurred;
        }
        public ModEffectCollection GetAllModEffects() => _effectCacheHelper.GetCacheCopy();
        public void ForceCacheUpdate() => _effectCacheHelper.ForceCacheUpdate();
        public IReadOnlyList<Mod> GetAllModsForSerialization() => _modInstances;

        private sealed class ModEffectCacheHelper : CachedValueController<ModEffectCollection, Affix>
        {
            // Handle to parent collection.
            private readonly IReadOnlyList<Mod> _parentModInstances;

            public ModEffectCacheHelper(Affix context)
                :base(context)
            {
                _parentModInstances = context._modInstances;
            }

            protected override ModEffectCollection CalculateNewCache()
            {
                // Get the mod effect from each mod instance in the affix.
                return new ModEffectCollection(_parentModInstances.Select(m => m.GetModEffect()));
            }

            protected override ModEffectCollection CreateCacheCopy(ModEffectCollection cache)
            {
                // ModEffectCollection is sealed and not modifiable. Safe to pass by reference.
                return cache;
            }
        }
    }
}
