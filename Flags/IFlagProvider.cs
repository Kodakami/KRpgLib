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
    public interface IDynamicFlagProvider : IFlagProvider
    {
        event System.Action OnFlagsChanged;
    }
}
