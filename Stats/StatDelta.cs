using System;
using System.Collections.Generic;
using System.Text;

namespace KRpgLib.Stats
{
    /// <summary>
    /// A combination of a stat and a delta for that stat.
    /// </summary>
    public struct StatDelta
    {
        /// <summary>
        /// The affected stat.
        /// </summary>
        public Stat Stat { get; }
        /// <summary>
        /// The delta affecting the stat.
        /// </summary>
        public Delta Delta { get; }
        /// <summary>
        /// The delta type of the delta affecting the stat. Shorthand for Delta.Type.
        /// </summary>
        public DeltaType DeltaType => Delta.Type;
        /// <summary>
        /// The delta value of the delta affecting the stat. Shorthand for Delta.Value.
        /// </summary>
        public int DeltaValue => Delta.Value;

        public StatDelta(Stat stat, Delta delta)
        {
            Stat = stat ?? throw new ArgumentNullException(nameof(stat));
            Delta = delta;
        }
        public StatDelta(Stat stat, DeltaType deltaType, int deltaValue)
            :this(stat, new Delta(deltaType, deltaValue))
        { }
    }
}
