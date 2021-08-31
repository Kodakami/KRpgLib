namespace KRpgLib.Stats
{
    /// <summary>
    /// A change to a stat value such as an addition or multiplication. The exact stat to which the delta applies is out of scope.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public struct Delta<TValue> where TValue : struct
    {
        /// <summary>
        /// The value to change the stat by (leave reductions negative).
        /// </summary>
        public TValue Value { get; }

        /// <summary>
        /// The type of change (addition, multiplication, etc...).
        /// </summary>
        public DeltaType<TValue> Type { get; }

        // ctor
        public Delta(DeltaType<TValue> type, TValue value)
        {
            Value = value;
            Type = type ?? throw new System.ArgumentNullException(nameof(type));
        }
    }
}
