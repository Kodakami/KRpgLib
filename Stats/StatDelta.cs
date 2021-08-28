namespace KRpgLib.Stats
{
    /// <summary>
    /// A change to a stat such as an addition or multiplication. Stat template in question is not included in this object.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public struct StatDelta<TValue> where TValue : struct
    {
        /// <summary>
        /// The value to change the stat by (leave reductions negative).
        /// </summary>
        public TValue Value { get; }

        /// <summary>
        /// The type of change (addition, multiplication, etc...). Use static instances of StatDeltaType.
        /// </summary>
        public StatDeltaType<TValue> Type { get; }

        // ctor
        public StatDelta(StatDeltaType<TValue> type, TValue value)
        {
            Value = value;
            Type = type ?? throw new System.ArgumentNullException(nameof(type));
        }
    }
}
