using System;
using System.Collections.Generic;

namespace KRpgLib.Affixes
{
    /// <summary>
    /// A blueprint for an Affix.
    /// </summary>
    public class AffixTemplate
    {
        private readonly List<ModTemplateBase> _modTemplates;

        public string InternaName { get; }
        public AffixType AffixType { get; }

        public int ModTemplateCount => _modTemplates.Count;

        // TODO: Tags.

        public AffixTemplate(string internalName, AffixType affixType, IEnumerable<ModTemplateBase> modTemplates)
        {
            InternaName = internalName ?? throw new ArgumentNullException(nameof(internalName));
            AffixType = affixType ?? throw new ArgumentNullException(nameof(affixType));
            _modTemplates = new List<ModTemplateBase>(modTemplates ?? throw new ArgumentNullException(nameof(modTemplates)));
        }
        public Affix CreateNewAffixInstance(Random rng)
        {
            var modInstances = _modTemplates.ConvertAll(mt => mt.CreateNewModInstance(rng));
            return new Affix(this, modInstances);
        }
    }
}
