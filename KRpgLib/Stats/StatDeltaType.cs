using System;
using System.Collections.Generic;

namespace KRpgLib.Stats
{
    public sealed class StatDeltaType<TValue> where TValue : struct
    {
        // A binary function signature.
        public delegate TValue DeltaTypeFunc(TValue input, TValue operand);

        //// Static instances (may be registered if desired in project)
        //public static StatDeltaType<int> Addition = new StatDeltaType<int>((input, operand) => input + operand, 0);
        //public static StatDeltaType<float> Multiplication = new StatDeltaType<float>((input, operand) => (int)(input * operand), 1);

        // This static method exists in each compiler-generated concrete type.
        public static List<StatDeltaType<TValue>> GetAllByPriority()
        {
            if (_cacheIsDirty)
            {
                var newList = new List<StatDeltaType<TValue>>();

                var temp = new List<KeyValuePair<StatDeltaType<TValue>, int>>(_registry);
                temp.Sort((kvp1, kvp2) => kvp1.Value - kvp2.Value);

                foreach (var item in temp)
                {
                    newList.Add(item.Key);
                }
                _deltaTypeCacheInPriorityOrder = newList;
                _cacheIsDirty = false;
            }

            return new List<StatDeltaType<TValue>>(_deltaTypeCacheInPriorityOrder);
        }
        private static List<StatDeltaType<TValue>> _deltaTypeCacheInPriorityOrder = new List<StatDeltaType<TValue>>();
        private static bool _cacheIsDirty = true;

        private static readonly Dictionary<StatDeltaType<TValue>, int> _registry = new Dictionary<StatDeltaType<TValue>, int>();

        // This static method without a type parameter is intended.
        public static void RegisterStatDeltaType(DeltaTypeFunc applyFunction, TValue baselineValue, DeltaTypeFunc combineFunction, int priority)
        {
            var newInstance = new StatDeltaType<TValue>(
                applyFunction ?? throw new ArgumentNullException(nameof(applyFunction)),
                baselineValue,
                combineFunction ?? throw new ArgumentNullException(nameof(combineFunction)));

            _registry.Add(newInstance, priority);
            _cacheIsDirty = true;
        }

        // Instance members.

        // The actual mathematical function this delta type performs on a stat.
        private readonly DeltaTypeFunc _func;

        // The function for combining deltas of the type. (addition for numerical types, not sure about others.)
        private readonly DeltaTypeFunc _combine;

        /// <summary>
        /// The neutral delta value (0 for addition, 1 for multiplication).
        /// </summary>
        public TValue BaselineValue { get; }

        private StatDeltaType(DeltaTypeFunc func, TValue baselineValue, DeltaTypeFunc combineFunc)
        {
            _func = func;
            BaselineValue = baselineValue;
            _combine = combineFunc;
        }

        /// <summary>
        /// Apply the effect of the StatDeltaType, such as adding or multiplying the values.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="operand"></param>
        public TValue Apply(TValue input, TValue operand) => _func(input, operand);
        public TValue Combine(TValue deltaValue1, TValue deltaValue2) => _combine(deltaValue1, deltaValue2);
    }
}
