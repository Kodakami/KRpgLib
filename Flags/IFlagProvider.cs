using System.Collections.Generic;

namespace KRpgLib.Flags
{
    public interface IFlagProvider
    {
        /// <summary>
        /// Get provided flags.
        /// </summary>
        IReadOnlyFlagCollection GetFlagCollection();
    }
    public interface IFlagProvider_Dynamic : IFlagProvider
    {
        event System.Action OnFlagsChanged;
    }
}
