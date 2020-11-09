using System.Collections.Generic;

namespace KRpgLib.Stats
{
    public interface IStatProvider
    {
        /// <summary>
        /// Returns true if this stat provider provides any deltas for the provided stat template.
        /// </summary>
        /// <param name="stat">an IStatTemplate</param>
        bool HasDeltasForStat(IStatTemplate stat);
        /// <summary>
        /// Get a collection of the stat that have deltas.
        /// </summary>
        List<IStatTemplate> GetStatsWithDeltas();
        /// <summary>
        /// Get all stat deltas for the provided stat template. Return empty list if there are none.
        /// </summary>
        /// <param name="stat">an IStatTemplate</param>
        /// <returns>new IEnumerable<StatDelta></returns>
        List<StatDelta> GetStatDeltasForStat(IStatTemplate stat);

        /// <summary>
        /// Invokes when a delta provided by this stat provider changes. This would be used when this is a dynamic stat provider.
        /// </summary>
        event System.Action<IStatProvider, IStatTemplate> OnStatDeltasChanged;
    }
}
