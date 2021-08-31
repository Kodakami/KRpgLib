using System;
using System.Collections.Generic;
using System.Text;

namespace KRpgLib.Stats
{
    /// <summary>
    /// A combination of a stat and a delta for that stat.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public struct StatDelta<TValue> where TValue : struct
    {
        /// <summary>
        /// The affected stat.
        /// </summary>
        public IStat<TValue> Stat { get; }
        /// <summary>
        /// The delta affecting the stat.
        /// </summary>
        public Delta<TValue> Delta { get; }
        /// <summary>
        /// The delta type of the delta affecting the stat. Shorthand for Delta.Type.
        /// </summary>
        public DeltaType<TValue> DeltaType => Delta.Type;
        /// <summary>
        /// The delta value of the delta affecting the stat. Shorthand for Delta.Value.
        /// </summary>
        public TValue DeltaValue => Delta.Value;

        public StatDelta(IStat<TValue> stat, Delta<TValue> delta)
        {
            Stat = stat ?? throw new ArgumentNullException(nameof(stat));
            Delta = delta;
        }
        public StatDelta(IStat<TValue> stat, DeltaType<TValue> deltaType, TValue deltaValue)
            :this(stat, new Delta<TValue>(deltaType, deltaValue))
        { }
    }
}
