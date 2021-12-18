using System;
using System.Collections.Generic;
using KRpgLib.Utility;
using KRpgLib.Utility.TemplateObject;
using KRpgLib.Affixes.AffixTypes;

namespace KRpgLib.Affixes
{
    /// <summary>
    /// A blueprint for an Affix.
    /// </summary>
    public sealed class AffixTemplate : ITemplate
    {
        private readonly List<ModTemplate> _modTemplates;

        public AffixType AffixType { get; }

        public int ModTemplateCount => _modTemplates.Count;

        // TODO: Tags.

        public AffixTemplate(AffixType affixType, IEnumerable<ModTemplate> modTemplates)
        {
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
