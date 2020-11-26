using System;
using System.Collections.Generic;

namespace KRpgLib.Stats
{
    /// <summary>
    /// A type of change to a stat value, such as addition vs. multiplication.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class StatDeltaType<TValue> where TValue : struct
    {
        /// <summary>
        /// A math operation taking two inputs and returning an output. Input values are passed as the left-hand operand.
        /// </summary>
        /// <param name="leftHand">the input value</param>
        /// <param name="rightHand">the new operand</param>
        /// <returns>result of operation on leftHand by rightHand</returns>
        public delegate TValue DeltaTypeFunc(TValue leftHand, TValue rightHand);

        // This static method exists in each compiler-generated concrete type.
        /// <summary>
        /// Get all registered stat delta types in order of priority.
        /// </summary>
        /// <returns>new list of stat delta types</returns>
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
        /// <summary>
        /// Register a new type of stat delta.
        /// </summary>
        /// <param name="function">the operation applied by the delta type</param>
        /// <param name="baselineValue">the neutral value of the delta (ex. 0 for additive, 1 for multiplicative)</param>
        /// <param name="combineFunction">the operation used to combine deltas of the same type (usually deltas are added together)</param>
        /// <param name="priority">the priority value of the delta type. delta types are evaluated in numerical order of priority (lower is sooner).</param>
        public static void RegisterStatDeltaType(DeltaTypeFunc function, TValue baselineValue, DeltaTypeFunc combineFunction, int priority)
        {
            var newInstance = new StatDeltaType<TValue>(
                function ?? throw new ArgumentNullException(nameof(function)),
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
        /// Apply the effect of the stat delta type, such as adding or multiplying the values.
        /// </summary>
        public TValue Apply(TValue input, TValue operand) => _func(input, operand);
        /// <summary>
        /// Combine the effect of two instances of the stat delta type.
        /// </summary>
        public TValue Combine(TValue deltaValue1, TValue deltaValue2) => _combine(deltaValue1, deltaValue2);
    }
}
