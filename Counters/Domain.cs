namespace KRpgLib.Counters
{
    public enum Domain
    {
        /// <summary>
        /// The counter is representative of a game mechanic. This should only be removable by other game mechanics, and not by curing of any kind.
        /// </summary>
        GAME_MECHANIC = -1,
        /// <summary>
        /// The counter can not be considered beneficial, detrimental, or mixed. It is amoral.
        /// </summary>
        NONE = 0,
        /// <summary>
        /// The counter is primarily a buff, boost, or boon. It was most likely granted by a friendly source.
        /// </summary>
        BENEFICIAL = 1,
        /// <summary>
        /// The counter is primarily a debuff, hindrance, or punishment. It was most likely inflicted by an enemy source.
        /// </summary>
        DETRIMENTAL = 2,
        /// <summary>
        /// The counter is a somewhat a benefit and a detriment. It could be considered good, bad, or both, depending on context.
        /// </summary>
        MIXED = 3,
    }
}
