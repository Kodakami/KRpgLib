using System;
using System.Collections.Generic;
using KRpgLib.Utility;

namespace KRpgLib.Stats
{
    /// <summary>
    /// A type of change to a stat value, such as addition or multiplication.
    /// </summary>
    public sealed class DeltaType
    {
        /// <summary>
        /// A math operation taking two inputs and returning an output. Input values are passed as the left-hand operand.
        /// </summary>
        /// <param name="leftHand">the input value</param>
        /// <param name="rightHand">the new operand</param>
        /// <returns>result of operation on leftHand by rightHand</returns>
        public delegate int DeltaTypeFunc(int leftHand, int rightHand);

        // Instance members.

        // The actual mathematical function this delta type performs on a stat.
        private readonly DeltaTypeFunc _func;

        /// <summary>
        /// The neutral delta value (0 for addition, 1 for multiplication).
        /// </summary>
        public int BaselineValue { get; }

        public DeltaType(DeltaTypeFunc func, int baselineValue)
        {
            _func = func ?? throw new ArgumentNullException(nameof(func));
            BaselineValue = baselineValue;
        }

        /// <summary>
        /// Apply the effect of the delta type onto a value, such as adding or multiplying the values.
        /// </summary>
        public int Apply(int input, int operand) => _func(input, operand);
    }
}
