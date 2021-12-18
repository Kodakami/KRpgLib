using KRpgLib.Utility;

namespace KRpgLib.Flags
{
    public sealed class FlagEnvironment
    {
        public static FlagEnvironment Instance { get; private set; }
        public static void SetInstance(FlagEnvironment instance) => Instance = instance ?? throw new System.ArgumentException(nameof(instance));

        public IUniqueRepo<IFlagTemplate> FlagTemplateRepo { get; }

        public FlagEnvironment(IUniqueRepo<IFlagTemplate> flagTemplateRepo)
        {
            FlagTemplateRepo = flagTemplateRepo ?? throw new System.ArgumentNullException(nameof(flagTemplateRepo));
        }
    }
}
