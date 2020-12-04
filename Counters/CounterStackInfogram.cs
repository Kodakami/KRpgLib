using System;
using System.Collections.Generic;

namespace KRpgLib.Counters
{
    public struct CounterStackInfogram
    {
        public CounterStackInfogram(CounterTemplate template, int count, int duration)
        {
            Template = template;
            Count = count;
            Duration = duration;
        }

        public CounterTemplate Template { get; }
        public int Count { get; }
        public int Duration { get; }
    }
}
