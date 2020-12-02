using System;

namespace KRpgLib.Utility.ValueCurves
{
    /// <summary>
    /// Return the interpolated result between a and b by t. A t value of 0 should return a, and t value of 1 should return b. Values less than 0 or greater than 1 should be expected and accounted for.
    /// </summary>
    public delegate T InterpolationDelegate<T>(T a, T b, double t);
    public struct Point<TypeY>
    {
        public double PositionX { get; }
        public TypeY ValueY { get; }
        public InterpolationDelegate<TypeY> InterpolationFunc { get; }

        /// <summary>
        /// Create a known point using threshold interpolation (value remains the same until the next known point).
        /// </summary>
        public Point(double positionX, TypeY valueY) : this(positionX, valueY, ThresholdInterpolation) { }
        /// <summary>
        /// Create a known point using a custom interpolation function.
        /// </summary>
        public Point(double positionX, TypeY valueY, InterpolationDelegate<TypeY> interpolationFunc)
        {
            if (valueY == null)
            {
                // Cannot null-coalesce with generic type.
                throw new ArgumentNullException(nameof(valueY));
            }

            PositionX = positionX;
            ValueY = valueY;
            InterpolationFunc = interpolationFunc ?? throw new ArgumentNullException(nameof(interpolationFunc));
        }

        private static TypeY ThresholdInterpolation(TypeY a, TypeY b, double t)
        {
            return t < 1 ? a : b;
        }
    }
}
