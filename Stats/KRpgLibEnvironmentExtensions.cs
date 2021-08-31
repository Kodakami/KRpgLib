using KRpgLib.Utility;

namespace KRpgLib.Stats
{
    public static class KRpgLibEnvironmentExtensions
    {
        public static StatEnvironment<TValue> Stats<TValue>(this KRpgEnvironment e) where TValue : struct => StatEnvironment<TValue>.Instance;
        public static void SetStatEnvironmentInstance<TValue>(this KRpgEnvironment e, StatEnvironment<TValue> instance) where TValue : struct
        {
            StatEnvironment<TValue>.SetInstance(instance);
        }
    }
}
