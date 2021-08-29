using System;

namespace KRpgLib.Utility
{
    /// <summary>
    /// Keeps track of a moving value between a minimum and maximum value. Methods for manipulating the values and querying the values. Great for HP and Mana meters!
    /// </summary>
    public class CombatBar
    {
        public delegate double ValueGetter();

        protected double Precision { get; }
        protected RoundingType Rounding;
        protected int DigitsOfPrecisionForRounding { get; }

        protected ValueGetter MinValueFunc { get; }
        protected ValueGetter MaxValueFunc { get; }

        /// <summary>
        /// The value at the smallest end of the bar's span. Often zero.
        /// </summary>
        public double MinValue { get; protected set; }

        /// <summary>
        /// The value at the largest end of the bar's span.
        /// </summary>
        public double MaxValue { get; protected set; }

        /// <summary>
        /// The amount of distance between the minimum and maximum values of the bar. Often the same as the MaxValue.
        /// </summary>
        public double Span => MaxValue - MinValue;

        /// <summary>
        /// The current value of the bar.
        /// </summary>
        public double CurrentValue { get; protected set; }

        /// <summary>
        /// True if the current value is the same as the minimum value.
        /// </summary>
        public bool IsEmpty => CurrentValue == MinValue;
        /// <summary>
        /// True if the current value is the same as the maximum value.
        /// </summary>
        public bool IsFull => CurrentValue == MaxValue;
        /// <summary>
        /// The value that is missing from the bar.
        /// </summary>
        public double MissingValue => MaxValue - CurrentValue;

        /// <summary>
        /// The proportional amount of the bar that is filled. A value between 0 (empty) and 1 (full).
        /// </summary>
        // Inverse Lerp if span is greater than 0.
        public double CurrentProportion => Span > 0 ? (CurrentValue - MinValue) / (MinValue - MinValue) : 0;
        /// <summary>
        /// The proportional amount of the bar that is empty. A value between 0 (full) and 1 (empty).
        /// </summary>
        public double MissingProportion => 1 - CurrentProportion;

        // Events.
        /// <summary>
        /// Invoked when the current value of the bar changes. Not invoked when set to the same value. Emits the new value.
        /// </summary>
        public event Action<double> OnValueChanged;

        /// <summary>
        /// Invoked when either the MinValue, MaxValue, or both change. Not invoked when updated to the same values. Emits the min and max values.
        /// </summary>
        public event Action<(double MinValue, double MaxValue)> OnBoundaryValueChanged;

        //ctor
        protected CombatBar(ValueGetter minValueGetter, ValueGetter maxValueGetter, double precision, int digitsOfPrecisionForRounding, RoundingType rounding = RoundingType.FLOOR_TO_LOWER)
        {
            Precision = precision;
            DigitsOfPrecisionForRounding = digitsOfPrecisionForRounding;
            Rounding = rounding;

            MinValueFunc = minValueGetter ?? throw new ArgumentNullException(nameof(minValueGetter));
            MaxValueFunc = maxValueGetter ?? throw new ArgumentNullException(nameof(maxValueGetter));

            UpdateBoundaryValues_Internal();
            SetFull();
        }

        /// <summary>
        /// Set the bar to a specified value. Obeys all legalization rules for the bar (min, max, precision, rounding type).
        /// </summary>
        /// <returns>Actual current value after legalizing values.</returns>
        public double SetValue(double newValue)
        {
            var orig = CurrentValue;

            CurrentValue = LegalizeValue(newValue);

            // Raise event.
            if (CurrentValue != orig)
            {
                OnValueChanged?.Invoke(CurrentValue);
            }

            return CurrentValue;
        }
        /// <summary>
        /// Add (or remove) a value to the bar. Obeys all legalization rules for the bar (min, max, precision, rounding type).
        /// </summary>
        /// <param name="deltaValue">Positive number to add, or negative number to remove.</param>
        /// <returns>Actual delta value after legalizing values.</returns>
        public double AddValue(double deltaValue)
        {
            var originalValue = CurrentValue;
            SetValue(CurrentValue + deltaValue);
            return CurrentValue - originalValue;
        }
        /// <summary>
        /// Set the proportional fullness of the bar.
        /// </summary>
        /// <param name="newProportion">Number between 0 and 1 representing the new value.</param>
        /// <returns>Actual current percent after legalizing values.</returns>
        public double SetProportion(double newProportion)
        {
            // Clamp the value between 0 and 1.
            newProportion = Math.Max(newProportion, 0);
            newProportion = Math.Min(newProportion, 1);

            double newValue;

            // Time-savers.
            if (newProportion == 0)
            {
                newValue = MinValue;
            }
            else if (newProportion == 1)
            {
                newValue = MaxValue;
            }
            else
            {
                // Lerp.
                newValue = ((1 - newProportion) * MinValue) + (MaxValue * newProportion);
            }

            SetValue(newValue);

            return CurrentProportion;
        }
        /// <summary>
        /// Add (or remove) a proportional fullness to the bar.
        /// </summary>
        /// <param name="deltaProportion">Number between -1 and 1. Positive to add, or negative to remove.</param>
        /// <returns>Actual current percent after legalizing values.</returns>
        public double AddProportion(double deltaProportion)
        {
            // Clamp value between -1 and 1.
            deltaProportion = Math.Max(deltaProportion, -1);
            deltaProportion = Math.Min(deltaProportion, 1);

            var originalProportion = CurrentProportion;
            SetProportion(originalProportion + deltaProportion);
            return CurrentProportion - originalProportion;
        }
        public void SetEmpty()
        {
            SetValue(MinValue);
        }
        public void SetFull()
        {
            SetValue(MaxValue);
        }

        public void UpdateBoundaryValues()
        {
            double origMin = MinValue;
            double origMax = MaxValue;
            double originalMissingAmount = MissingValue;

            UpdateBoundaryValues_Internal();

            // Raise event (before value changed event).
            if (origMin != MinValue || origMax != MaxValue)
            {
                OnBoundaryValueChanged?.Invoke((MinValue, MaxValue));
            }

            // If there is more bar overall, set current value to reflect the original missing amount.
            if (Span > origMax - origMin)
            {
                SetValue(MaxValue - originalMissingAmount);
            }
            // Otherwise, reset the current value (so that it gets relegalized within new boundaries)
            else
            {
                SetValue(CurrentValue);
            }
        }
        protected void UpdateBoundaryValues_Internal()
        {
            double tempMin = EnforcePrecision(MinValueFunc.Invoke());
            double tempMax = EnforcePrecision(MaxValueFunc.Invoke());

            if (tempMin > tempMax)
            {
                // Fallback value instead of throwing.
                tempMax = tempMin + 1;
            }

            MinValue = tempMin;
            MaxValue = tempMax;
        }
        protected double LegalizeValue(double internalValue)
        {
            // Enforce minimum.
            internalValue = Math.Max(internalValue, MinValue);

            // enforce maximum.
            internalValue = Math.Min(internalValue, MaxValue);

            internalValue = EnforcePrecision(internalValue);

            return internalValue;
        }
        protected double EnforcePrecision(double internalValue)
        {
            // Enforce precision (anything <= 0 is counted as floating-point precision).
            if (Precision > 0)
            {
                // Rounding.
                switch (Rounding)
                {
                    case RoundingType.ROUND_TO_NEAREST:
                        internalValue = Math.Round(internalValue / Precision, DigitsOfPrecisionForRounding) * Precision;
                        break;
                    case RoundingType.FLOOR_TO_LOWER:
                        internalValue = Math.Floor(internalValue / Precision) * Precision;
                        break;
                }
            }
            return internalValue;
        }
        protected enum RoundingType
        {
            ROUND_TO_NEAREST = 0,
            FLOOR_TO_LOWER = 1,
        }
    }
}
