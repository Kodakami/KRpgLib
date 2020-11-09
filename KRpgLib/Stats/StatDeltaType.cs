using System;
using System.Collections.Generic;

namespace KRpgLib.Stats
{
    /// <summary>
    /// How will the stat be affected by the delta value?
    /// </summary>
    public sealed class StatDeltaType
    {
        // A binary math function signature.
        private delegate float DeltaTypeFunc(float x, float y);

        // Static instances.
        public static StatDeltaType Addition = new StatDeltaType((x, y) => x + y, 0);
        public static StatDeltaType Multiplication = new StatDeltaType((x, y) => x * y, 1);

        // All stat delta types by priority. (This could be done more elegantly, but it's not worth the effort.)
        public static List<StatDeltaType> AllByPriority { get; } = new List<StatDeltaType>()
        {
            Addition,
            Multiplication,
        };

        // The actual mathematical function this delta type performs on a stat.
        private readonly DeltaTypeFunc _func;

        /// <summary>
        /// The neutral delta value (0 for addition, 1 for multiplication).
        /// </summary>
        public float BaselineValue { get; }

        // ctor
        private StatDeltaType(DeltaTypeFunc func, float baselineValue)
        {
            _func = func;
            BaselineValue = baselineValue;
        }

        /// <summary>
        /// Apply the effect of the StatDeltaType, such as adding or multiplying the values.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public float Apply(float x, float y) => _func(x, y);
    }
}
