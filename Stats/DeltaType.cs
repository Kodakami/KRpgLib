using System;
using System.Collections.Generic;
using KRpgLib.Utility;

namespace KRpgLib.Stats
{
    /// <summary>
    /// A type of change to a stat value, such as addition or multiplication.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class DeltaType<TValue> where TValue : struct
    {
        /// <summary>
        /// A math operation taking two inputs and returning an output. Input values are passed as the left-hand operand.
        /// </summary>
        /// <param name="leftHand">the input value</param>
        /// <param name="rightHand">the new operand</param>
        /// <returns>result of operation on leftHand by rightHand</returns>
        public delegate TValue DeltaTypeFunc(TValue leftHand, TValue rightHand);

        // Instance members.

        // The actual mathematical function this delta type performs on a stat.
        private readonly DeltaTypeFunc _func;

        // The function for combining deltas of the type. (addition for numerical types, not sure about others.)
        private readonly DeltaTypeFunc _combine;

        /// <summary>
        /// The neutral delta value (0 for addition, 1 for multiplication).
        /// </summary>
        public TValue BaselineValue { get; }

        public DeltaType(DeltaTypeFunc func, TValue baselineValue, DeltaTypeFunc combineFunc)
        {
            _func = func ?? throw new ArgumentNullException(nameof(func));
            _combine = combineFunc ?? throw new ArgumentNullException(nameof(combineFunc));

            BaselineValue = baselineValue;
        }

        /// <summary>
        /// Apply the effect of the delta type onto a value, such as adding or multiplying the values.
        /// </summary>
        public TValue Apply(TValue input, TValue operand) => _func(input, operand);
        /// <summary>
        /// Combine the effects of two instances of the delta type. This is nearly always simple addition.
        /// </summary>
        public TValue Combine(TValue deltaValue1, TValue deltaValue2) => _combine(deltaValue1, deltaValue2);
    }
}
