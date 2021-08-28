using System;
using System.Collections.Generic;
using KRpgLib.Utility;

namespace KRpgLib.Stats
{
    /// <summary>
    /// A type of change to a stat value, such as addition vs. multiplication. Static members provide functions for registering stat delta types, a step required for correctly calculating stat values.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class StatDeltaType<TValue> where TValue : struct
    {
        // Delegate.

        /// <summary>
        /// A math operation taking two inputs and returning an output. Input values are passed as the left-hand operand.
        /// </summary>
        /// <param name="leftHand">the input value</param>
        /// <param name="rightHand">the new operand</param>
        /// <returns>result of operation on leftHand by rightHand</returns>
        public delegate TValue DeltaTypeFunc(TValue leftHand, TValue rightHand);

        // Static members (a distinct copy exists in each compiler-generated type).

        private static readonly PriorityRegistry<StatDeltaType<TValue>> _registry = new PriorityRegistry<StatDeltaType<TValue>>();

        /// <summary>
        /// Get all registered stat delta types in order of priority.
        /// </summary>
        /// <returns>new list of stat delta types</returns>
        public static List<StatDeltaType<TValue>> GetAllByPriority()
        {
            return _registry.GetAllByPriority();
        }

        /// <summary>
        /// Register a new type of stat delta. This step is necessary for a stat delta type to be considered when performing stat calculations. Stat delta types are evaluated in numerical order of priority (lower priority values are evaluated before higher ones). Each type of backing value for a stat has a separate registry.
        /// </summary>
        /// <param name="function">the operation applied by the delta type</param>
        /// <param name="baselineValue">the neutral value of the delta (ex. 0 for additive, 1 for multiplicative)</param>
        /// <param name="combineFunction">the operation used to combine deltas of the same type (usually deltas are added together)</param>
        /// <param name="priority">the priority value of the delta type</param>
        public static void RegisterStatDeltaType(DeltaTypeFunc function, TValue baselineValue, DeltaTypeFunc combineFunction, int priority)
        {
            var newInstance = new StatDeltaType<TValue>(
                function ?? throw new ArgumentNullException(nameof(function)),
                baselineValue,
                combineFunction ?? throw new ArgumentNullException(nameof(combineFunction)));

            _registry.RegisterItem(newInstance, priority);
        }
        /// <summary>
        /// In the event that you might want to unregister a stat delta type, the function exists and works. The delta type will no longer be considered when performing stat calculations.
        /// </summary>
        /// <param name="statDeltaType">the stat delta type to unregister</param>
        public static void UnregisterStatDeltaType(StatDeltaType<TValue> statDeltaType)
        {
            _registry.UnregisterItem(statDeltaType);
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
