namespace KRpgLib.Stats
{
    /// <summary>
    /// Interface for a collection of stat values at a single relative moment in time.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public interface IStatSet<TValue> where TValue : struct
    {
        /// <summary>
        /// Get the raw value of a stat. If the stat has no recorded value, returns the stat's default value.
        /// </summary>
        /// <param name="statTemplate">any stat template</param>
        /// <returns>current stat value, accurate to the represented moment in time</returns>
        TValue GetStatValue(IStatTemplate<TValue> statTemplate);
    }
    /// <summary>
    /// Abstract base class for objects which represent a collection of stat values at a single relative moment in time. Contains convenience methods and argument checks.
    /// </summary>
    /// /// <typeparam name="TValue">stat backing type</typeparam>
    public abstract class AbstractStatSet<TValue> : IStatSet<TValue> where TValue : struct
    {
        /// <summary>
        /// Returns the raw stat value. Argument has already been null-checked.
        /// </summary>
        /// <param name="safeStatTemplate">null-checked stat template</param>
        /// <returns>raw stat value</returns>
        protected abstract TValue GetStatValue_Internal(IStatTemplate<TValue> safeStatTemplate);
        public TValue GetStatValue(IStatTemplate<TValue> statTemplate)
        {
            if (statTemplate == null)
            {
                throw new System.ArgumentNullException(nameof(statTemplate));
            }

            return GetStatValue_Internal(statTemplate);
        }
        /// <summary>
        /// Get the legalized value of a stat. If the stat has no recorded value, returns the stat's legalized default value.
        /// </summary>
        /// <param name="statTemplate">any stat template</param>
        /// <returns>current legal stat value, accurate to the represented moment in time</returns>
        public TValue GetStatValueLegalized(IStatTemplate<TValue> statTemplate)
        {
            if (statTemplate == null)
            {
                throw new System.ArgumentNullException(nameof(statTemplate));
            }
            return statTemplate.GetLegalizedValue(GetStatValue_Internal(statTemplate));
        }
    }
}