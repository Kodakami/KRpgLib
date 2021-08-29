using System;
using System.Collections.Generic;
using KRpgLib.Utility.TemplateObject;

namespace KRpgLib.Affixes
{
    /// <summary>
    /// A blueprint for an Affix.
    /// </summary>
    public class AffixTemplate : ITemplate
    {
        private readonly List<ModTemplate> _modTemplates;

        public string InternalName { get; }
        public AffixType AffixType { get; }

        public int ModTemplateCount => _modTemplates.Count;

        // TODO: Tags.

        public AffixTemplate(string internalName, AffixType affixType, IEnumerable<ModTemplate> modTemplates)
        {
            InternalName = internalName ?? throw new ArgumentNullException(nameof(internalName));
            AffixType = affixType ?? throw new ArgumentNullException(nameof(affixType));
            _modTemplates = new List<ModTemplate>(modTemplates ?? throw new ArgumentNullException(nameof(modTemplates)));
        }
        public Affix CreateNewAffixInstance(Random rng)
        {
            var modInstances = _modTemplates.ConvertAll(mt => mt.CreateNewModInstance(rng));
            return new Affix(this, modInstances);
        }
    }
}
