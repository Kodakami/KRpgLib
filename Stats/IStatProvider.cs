using System.Collections.Generic;

namespace KRpgLib.Stats
{
    /// <summary>
    /// Interface for an object which provides a delta collection to a stat manager.
    /// </summary>
    public interface IStatProvider
    {
        DeltaCollection GetDeltaCollection();
    }
    public interface IDynamicStatProvider : IStatProvider
    {
        event System.Action OnStatsChanged;
    }
}
