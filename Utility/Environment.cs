using System;
using System.Collections.Generic;
using System.Text;

namespace KRpgLib.Utility
{
    public sealed class Environment
    {
        private static Environment Self;

        private Random _rng;

        private Environment(Random rng)
        {
            _rng = rng;
        }
        public static void CreateSingleton(Random rng)
        {
            if (Self == null)
            {
                Self = new Environment(rng);
            }
        }
        public static Random Rng => Self._rng;
        public static void SetRng(Random rng) => Self._rng = rng ?? throw new ArgumentNullException(nameof(rng));
    }
}
