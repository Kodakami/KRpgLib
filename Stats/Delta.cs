namespace KRpgLib.Stats
{
    /// <summary>
    /// A change to a stat value such as an addition or multiplication. The exact stat to which the delta applies is out of scope.
    /// </summary>
    public struct Delta
    {
        /// <summary>
        /// The value to change the stat by (leave reductions negative).
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// The type of change (addition, multiplication, etc...).
        /// </summary>
        public DeltaType Type { get; }

        // ctor
        public Delta(DeltaType type, int value)
        {
            Value = value;
            Type = type ?? throw new System.ArgumentNullException(nameof(type));
        }
    }
}
