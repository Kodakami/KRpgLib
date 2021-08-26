using System;
using System.Collections.Generic;

namespace KRpgLib.Affixes
{
    /// <summary>
    /// An instance of an affix (group of connected mods) which will modify values associated with an IModdable. Points to an AffixTemplate and holds mod instances.
    /// </summary>
    public class Affix
    {
        private readonly List<ModBase> _modInstances;

        public AffixTemplate Template { get; }

        public Affix(AffixTemplate template, IEnumerable<ModBase> modInstances)
        {
            Template = template;

            _modInstances = new List<ModBase>(modInstances);
        }
        public void RerollAllMods(Random rng) => _modInstances.ForEach(m => m.RollNewValue(rng));
        public void Modify(IModdable moddable) => _modInstances.ForEach(m => m.Modify(moddable));
    }
}
