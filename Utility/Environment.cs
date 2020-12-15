using System;
using System.Collections.Generic;
using System.Text;

namespace KRpgLib.Utility
{
    public static class Environment
    {
        private static Random _rng;
        public static Random Rng => _rng ?? new Random();   // Lazy. Good.

        public static void SetRng(Random rng) => _rng = rng ?? throw new ArgumentNullException(nameof(rng));
    }
}
