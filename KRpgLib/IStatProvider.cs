using System.Collections.Generic;

namespace KRpgLib.Stats
{
    /// <summary>
    /// Interface for an object which provides stat value deltas to a stat manager.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public interface IStatProvider<TValue> where TValue : struct
    {
        /// <summary>
        /// Returns true if this stat provider provides any deltas for the provided stat template.
        /// </summary>
        /// <param name="stat">any stat template</param>
        bool HasDeltasForStat(IStatTemplate<TValue> stat);
        /// <summary>
        /// Get a list of the stat templates that have stat value deltas.
        /// </summary>
        List<IStatTemplate<TValue>> GetStatsWithDeltas();
        /// <summary>
        /// Get all stat value deltas for the provided stat template. Return empty list if there are none.
        /// </summary>
        /// <param name="stat">any stat template</param>
        /// <returns>new list of StatDeltas</returns>
        List<StatDelta<TValue>> GetDeltasForStat(IStatTemplate<TValue> stat);
    }
    /// <summary>
    /// Interface for an object which provides stat value deltas to a stat manager. The deltas provided by this object may change during runtime, after which an event will be raised.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public interface IStatProvider_Dynamic<TValue> : IStatProvider<TValue> where TValue : struct
    {
        /// <summary>
        /// Is raised when a stat value delta provided by this stat provider changes. Passes this stat provider and the affected stat template as arguments.
        /// </summary>
        event System.Action<IStatProvider_Dynamic<TValue>, IStatTemplate<TValue>> OnStatDeltasChanged;
    }
}
