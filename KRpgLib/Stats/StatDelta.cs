namespace KRpgLib.Stats
{
    /// <summary>
    /// A change to a stat such as an addition or multiplication. Stat template in question is not included in this object.
    /// </summary>
    public struct StatDelta<TValue> where TValue : struct
    {
        /// <summary>
        /// The value to change the stat by (leave subtraction negative).
        /// </summary>
        public TValue Value { get; }

        /// <summary>
        /// The type of change (addition, multiplication, etc...). Use static instances of StatDeltaType.
        /// </summary>
        public StatDeltaType<TValue> Type { get; }

        // ctor
        public StatDelta(TValue value, StatDeltaType<TValue> type)
        {
            Value = value;
            Type = type;
        }
    }
}
