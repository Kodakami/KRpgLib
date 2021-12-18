using System;

namespace KRpgLib.Utility
{
    /// <summary>
    /// All the environment variables that need to be accessed by KRpgLib systems. This class is extended by each module.
    /// </summary>
    public class KRpgEnvironment
    {
        // Static singleton management.
        private static KRpgEnvironment _instance;
        public static KRpgEnvironment Instance => _instance ?? new KRpgEnvironment();
        public static void SetInstance(KRpgEnvironment environment) => _instance = environment ?? throw new ArgumentNullException(nameof(environment));

        // Instance members.
        private Random _rng;
        public Random Rng => _rng ?? new Random();   // Lazy. Good.

        // Instance methods.
        public void SetRng(Random rng) => _rng = rng ?? throw new ArgumentNullException(nameof(rng));
    }
}
