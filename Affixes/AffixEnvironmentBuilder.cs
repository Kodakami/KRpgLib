using KRpgLib.Utility;

namespace KRpgLib.Affixes
{
    public sealed class AffixEnvironmentBuilder
    {
        private IUniqueRepo<ModTemplate> _modTemplateRepo;
        private IUniqueRepo<AffixTemplate> _affixTemplateRepo;

        /// <summary>
        /// Register a unique repo of mod templates with the affix environment. The unique repo acts as a lookup table and fetches a unique ID value for each mod template. The unique ID values returned by the repo are mostly used for serialization, but the means by which they are determined is left to developer implementation.
        /// </summary>
        public void RegisterModTemplateRepo(IUniqueRepo<ModTemplate> modTemplateRepo)
        {
            _modTemplateRepo = modTemplateRepo ?? throw new System.ArgumentNullException(nameof(modTemplateRepo));
        }
        /// <summary>
        /// Register a unique repo of affix templates with the affix environment. The unique repo acts as a lookup table and fetches a unique ID value for each affix template. The unique ID values returned by the repo are mostly used for serialization, but the means by which they are determined is left to developer implementation.
        /// </summary>
        public void RegisterAffixTemplateRepo(IUniqueRepo<AffixTemplate> affixTemplateRepo)
        {
            _affixTemplateRepo = affixTemplateRepo ?? throw new System.ArgumentNullException(nameof(affixTemplateRepo));
        }

        /// <summary>
        /// Build the custom flag environment. Optionally, set the new flag environment as the singleton instance.
        /// </summary>
        public AffixEnvironment Build(bool andRegisterAsSingletonInstance)
        {
            var newInstance = new AffixEnvironment(_modTemplateRepo, _affixTemplateRepo);

            if (andRegisterAsSingletonInstance)
            {
                AffixEnvironment.SetInstance(newInstance);
            }

            return newInstance;
        }
    }
}
