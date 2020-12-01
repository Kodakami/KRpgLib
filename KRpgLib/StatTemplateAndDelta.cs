using System;
using System.Collections.Generic;
using System.Text;

namespace KRpgLib.Stats
{
    /// <summary>
    /// A combination of a stat template and a delta for that stat template. This is more clear than "System.KeyValuePair<IStatTemplate<TValue>, StatDelta<TValue>>".
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public struct StatTemplateAndDelta<TValue> where TValue : struct
    {
        public IStatTemplate<TValue> Template { get; }
        public StatDelta<TValue> Delta { get; }
        public StatDeltaType<TValue> DeltaType => Delta.Type;
        public TValue DeltaValue => Delta.Value;

        public StatTemplateAndDelta(IStatTemplate<TValue> template, StatDelta<TValue> delta)
        {
            Template = template ?? throw new ArgumentNullException(nameof(template));
            Delta = delta;
        }
        public StatTemplateAndDelta(IStatTemplate<TValue> template, StatDeltaType<TValue> deltaType, TValue deltaValue)
            :this(template, new StatDelta<TValue>(deltaType, deltaValue))
        { }
    }
}
