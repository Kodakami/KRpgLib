namespace KRpgLib.Counters
{
    public struct CuringInfo
    {
        /// <summary>
        /// Number of ticks before this counter can be cured (removed by user effects). Null indicates counter is not curable.
        /// </summary>
        public int? DurationBeforeCurable { get; }

        public CuringInfo(int? durationBeforeCurable)
        {
            DurationBeforeCurable = durationBeforeCurable;
        }

        
    }
}
