using KRpgLib.Stats.Compound;

namespace KRpgLib.Stats
{
    /// <summary>
    /// Interface for a collection of stat values. Implementing this is a guarantee that all returned values are accurate to the represented relative moment in time.
    /// </summary>
    public interface IStatSet<TValue> where TValue : struct
    {
        /// <summary>
        /// Get the raw value of a stat. If the stat has no recorded value, return the stat's default value.
        /// </summary>
        /// <param name="statTemplate">an IStatTemplate</param>
        /// <returns>current stat value, accurate to the represented moment in time</returns>
        TValue GetStatValue(IStatTemplate<TValue> statTemplate);
    }
    public interface ICompoundStatSet<TValue> where TValue : struct
    {
        /// <summary>
        /// Get the raw value of a stat. If the stat has no recorded value, return the stat's default value.
        /// </summary>
        /// <param name="statTemplate">an IStatTemplate</param>
        /// <returns>current stat value, accurate to the represented moment in time</returns>
        TValue GetCompoundStatValue(ICompoundStatTemplate<TValue> compoundStatTemplate);
    }
    /// <summary>
    /// Abstract implementation of IStatSet and ICompoundStatSet. Contains extra convenience methods.
    /// </summary>
    public abstract class AbstractStatSet<TValue> : IStatSet<TValue>, ICompoundStatSet<TValue> where TValue : struct
    {
        protected abstract TValue GetStatValue_Internal(IStatTemplate<TValue> safeStatTemplate);
        public TValue GetStatValue(IStatTemplate<TValue> statTemplate)
        {
            if (statTemplate == null)
            {
                throw new System.ArgumentNullException(nameof(statTemplate));
            }

            return GetStatValue_Internal(statTemplate);
        }
        public TValue GetStatValueLegalized(IStatTemplate<TValue> statTemplate)
        {
            if (statTemplate == null)
            {
                throw new System.ArgumentNullException(nameof(statTemplate));
            }
            return statTemplate.GetLegalizedValue(GetStatValue_Internal(statTemplate));
        }
        protected abstract TValue GetCompoundStatValue_Internal(ICompoundStatTemplate<TValue> safeCompoundStatTemplate);
        public TValue GetCompoundStatValue(ICompoundStatTemplate<TValue> compoundStatTemplate)
        {
            if (compoundStatTemplate == null)
            {
                throw new System.ArgumentNullException(nameof(compoundStatTemplate));
            }

            return compoundStatTemplate.CalculateValue(this);
        }
        public TValue GetCompoundStatValueLegalized(ICompoundStatTemplate<TValue> compoundStatTemplate)
        {
            if (compoundStatTemplate == null)
            {
                throw new System.ArgumentNullException(nameof(compoundStatTemplate));
            }

            return compoundStatTemplate.GetLegalizedValue(compoundStatTemplate.CalculateValue(this));
        }
    }
}