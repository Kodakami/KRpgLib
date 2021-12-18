using KRpgLib.Utility;

namespace KRpgLib.Stats
{
    public static class KRpgLibEnvironmentExtensions
    {
        public static StatEnvironment Stats(this KRpgEnvironment e) => StatEnvironment.Instance;
        public static void SetStatEnvironmentInstance(this KRpgEnvironment e, StatEnvironment instance)
        {
            StatEnvironment.SetInstance(instance);
        }
    }
}
