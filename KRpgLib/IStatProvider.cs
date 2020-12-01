﻿using System.Collections.Generic;

namespace KRpgLib.Stats
{
    /// <summary>
    /// Interface for an object which provides stat value deltas to a stat manager.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public interface IStatProvider<TValue> where TValue : struct
    {
        StatDeltaCollection<TValue> GetStatDeltaCollection();
    }
}