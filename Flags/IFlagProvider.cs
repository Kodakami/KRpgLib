using System.Collections.Generic;

namespace KRpgLib.Flags
{
    public interface IFlagProvider
    {
        /// <summary>
        /// Get provided flags.
        /// </summary>
        FlagCollection GetFlagCollection();
    }
    public interface IFlagProvider_Dynamic : IFlagProvider
    {
        event System.Action OnFlagsChanged;
    }
}
