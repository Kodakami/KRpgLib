using KRpgLib.Utility;

namespace KRpgLib.Flags
{
    public sealed class FlagEnvironmentBuilder
    {
        private IUniqueRepo<IFlagTemplate> _flagTemplateRepo;

        /// <summary>
        /// Register a unique repo of flag templates with the flag environment. The unique repo acts as a lookup table and fetches a unique ID value for each flag template. The unique ID values returned by the repo are mostly used for serialization, but the means by which they are determined is left to developer implementation.
        /// </summary>
        public void RegisterFlagTemplateRepo(IUniqueRepo<IFlagTemplate> flagTemplateRepo)
        {
            _flagTemplateRepo = flagTemplateRepo ?? throw new System.ArgumentNullException(nameof(flagTemplateRepo));
        }

        /// <summary>
        /// Build the custom flag environment. Optionally, set the new flag environment as the singleton instance.
        /// </summary>
        public FlagEnvironment Build(bool andRegisterAsSingletonInstance)
        {
            var newInstance = new FlagEnvironment(_flagTemplateRepo);

            if (andRegisterAsSingletonInstance)
            {
                FlagEnvironment.SetInstance(newInstance);
            }

            return newInstance;
        }
    }
}
