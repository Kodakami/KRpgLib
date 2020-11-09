namespace KRpgLib.Stats
{
    /// <summary>
    /// A change to a stat such as an addition or multiplication. Stat template in question is not included in this object.
    /// </summary>
    public struct StatDelta
    {
        /// <summary>
        /// The value to change the stat by (leave subtraction negative).
        /// </summary>
        public float Value { get; }

        /// <summary>
        /// The type of change (addition, multiplication, etc...). Use static instances of StatDeltaType.
        /// </summary>
        public StatDeltaType Type { get; }

        // ctor
        public StatDelta(float value, StatDeltaType type)
        {
            Value = value;
            Type = type;
        }
    }
}
