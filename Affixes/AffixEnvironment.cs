using KRpgLib.Utility;
using System;

namespace KRpgLib.Affixes
{
    public sealed class AffixEnvironment
    {
        public static AffixEnvironment Instance { get; private set; }
        public static void SetInstance(AffixEnvironment instance) => Instance = instance ?? throw new System.ArgumentNullException(nameof(instance));

        public IUniqueRepo<ModTemplate> ModTemplateRepo { get; }
        public IUniqueRepo<AffixTemplate> AffixTemplateRepo { get; }

        public AffixEnvironment(IUniqueRepo<ModTemplate> modTemplateRepo, IUniqueRepo<AffixTemplate> affixTemplateRepo)
        {
            ModTemplateRepo = modTemplateRepo ?? throw new ArgumentNullException(nameof(modTemplateRepo));
            AffixTemplateRepo = affixTemplateRepo ?? throw new ArgumentNullException(nameof(affixTemplateRepo));
        }
    }
}
