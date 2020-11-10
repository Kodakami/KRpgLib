using System.Collections.Generic;

namespace KRpgLib.Stats
{
    public interface IStatProvider<TValue> where TValue : struct
    {
        /// <summary>
        /// Returns true if this stat provider provides any deltas for the provided stat template.
        /// </summary>
        /// <param name="stat">an IStatTemplate</param>
        bool HasDeltasForStat(IStatTemplate<TValue> stat);
        /// <summary>
        /// Get a collection of the stat that have deltas.
        /// </summary>
        List<IStatTemplate<TValue>> GetStatsWithDeltas();
        /// <summary>
        /// Get all stat deltas for the provided stat template. Return empty list if there are none.
        /// </summary>
        /// <param name="stat">an IStatTemplate</param>
        /// <returns>new IEnumerable<StatDelta></returns>
        List<StatDelta<TValue>> GetStatDeltasForStat(IStatTemplate<TValue> stat);
    }
    public interface IStatProvider_Dynamic<TValue> : IStatProvider<TValue> where TValue : struct
    {
        /// <summary>
        /// Invokes when a delta provided by this stat provider changes.
        /// </summary>
        event System.Action<IStatProvider_Dynamic<TValue>, IStatTemplate<TValue>> OnStatDeltasChanged;
    }
}
