using KRpgLib.Stats.Compound;

namespace KRpgLib.Stats
{
    /// <summary>
    /// Interface for a collection of stat values. Implementing this is a guarantee that all returned values are accurate to the represented relative moment in time.
    /// </summary>
    public interface IStatSet
    {
        /// <summary>
        /// Get the raw value of a stat. If the stat has no recorded value, return the stat's default value.
        /// </summary>
        /// <param name="statTemplate">an IStatTemplate</param>
        /// <returns>current stat value, accurate to the represented moment in time</returns>
        float GetStatValue(IStatTemplate statTemplate);
    }
    public interface ICompoundStatSet
    {
        /// <summary>
        /// Get the raw value of a stat. If the stat has no recorded value, return the stat's default value.
        /// </summary>
        /// <param name="statTemplate">an IStatTemplate</param>
        /// <returns>current stat value, accurate to the represented moment in time</returns>
        float GetCompoundStatValue(ICompoundStatTemplate compoundStatTemplate);
    }
    /// <summary>
    /// Abstract implementation of IStatSet and ICompoundStatSet. Contains extra convenience methods.
    /// </summary>
    public abstract class AbstractStatSet : IStatSet, ICompoundStatSet
    {
        protected abstract float GetStatValue_Internal(IStatTemplate safeStatTemplate);
        public float GetStatValue(IStatTemplate stat, bool useLegalizedValue = true)
        {
            if (stat == null)
            {
                throw new System.ArgumentNullException(nameof(stat));
            }

            float raw = GetStatValue_Internal(stat);
            return useLegalizedValue ? stat.GetLegalizedValue(raw) : raw;
        }
        protected abstract float GetCompoundStatValue_Internal(ICompoundStatTemplate safeCompoundStatTemplate);
        public float GetCompoundStatValue(ICompoundStatTemplate compoundStat, bool useLegalizedValue = true)
        {
            if (compoundStat == null)
            {
                throw new System.ArgumentNullException(nameof(compoundStat));
            }
            if (compoundStat.Algorithm == null)
            {
                throw new System.ArgumentNullException(nameof(compoundStat.Algorithm));
            }

            float raw = compoundStat.Algorithm.CalculateValue(this);
            return useLegalizedValue ? compoundStat.GetLegalizedValue(raw) : raw;
        }

        float IStatSet.GetStatValue(IStatTemplate statTemplate) => GetStatValue(statTemplate);
        float ICompoundStatSet.GetCompoundStatValue(ICompoundStatTemplate compoundStatTemplate) => GetCompoundStatValue(compoundStatTemplate);
    }
}