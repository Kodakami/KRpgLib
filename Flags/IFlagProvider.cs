﻿using System.Collections.Generic;

namespace KRpgLib.Flags
{
    public interface IFlagProvider
    {
        /// <summary>
        /// Get provided flags. If no flags are provided under current conditions (dynamic flag providers only), return null.
        /// </summary>
        List<Flag> GetAllFlags();
    }
    public interface IFlagProvider_Dynamic : IFlagProvider
    {
        event System.Action OnFlagsChanged;
    }
}
