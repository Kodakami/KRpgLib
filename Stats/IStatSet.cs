namespace KRpgLib.Stats
{
    /// <summary>
    /// Interface for a collection of stat values at a single relative moment in time.
    /// </summary>
    public interface IStatSet
    {
        /// <summary>
        /// Get the raw value of a stat. If the stat has no recorded value, returns the stat's default value.
        /// </summary>
        /// <param name="stat">any stat</param>
        /// <returns>current stat value, accurate to the represented moment in time</returns>
        int GetStatValue(Stat stat);
    }
}