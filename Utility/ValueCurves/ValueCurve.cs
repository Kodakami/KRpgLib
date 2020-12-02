using System;
using System.Collections.Generic;
using System.Linq;

namespace KRpgLib.Utility.ValueCurves
{
    public struct ValueCurve<TypeY>
    {
        private readonly SortedDictionary<double, Point<TypeY>> _orderedKnownPoints;

        public KnownPointExtents<double, TypeY> KnownPointExtents { get; }
        public double FirstPositionX => KnownPointExtents.FirstPositionX;
        public TypeY FirstValueY => KnownPointExtents.FirstValueY;
        public double LastPositionX => KnownPointExtents.LastPositionX;
        public TypeY LastValueY => KnownPointExtents.LastValueY;

        public ValueCurve(List<Point<TypeY>> knownPoints)
        {
            if (knownPoints == null)
            {
                throw new ArgumentNullException(nameof(knownPoints));
            }
            if (knownPoints.Count == 0)
            {
                throw new ArgumentException("Collection must contain at least one item.", nameof(knownPoints));
            }
            // If there are any two points with the same PositionX value,
            if (knownPoints.GroupBy(p => p.PositionX).Any(g => g.Count() > 1))
            {
                throw new ArgumentException("Collection may not contain any points with duplicate PositionX values.", nameof(knownPoints));
            }

            // Build a sorted dictionary with PosiitonX value being the key.
            _orderedKnownPoints = new SortedDictionary<double, Point<TypeY>>();
            foreach (var point in knownPoints)
            {
                _orderedKnownPoints.Add(point.PositionX, point);
            }

            var firstPoint = _orderedKnownPoints.First();
            var lastPoint = _orderedKnownPoints.Last();
            KnownPointExtents = new KnownPointExtents<double, TypeY>(firstPoint.Key, firstPoint.Value.ValueY, lastPoint.Key, lastPoint.Value.ValueY);
        }
        public ValueCurve(SortedDictionary<double, Point<TypeY>> knownPointDict)
        {
            if (knownPointDict == null)
            {
                throw new ArgumentNullException(nameof(knownPointDict));
            }
            if (knownPointDict.Count == 0)
            {
                throw new ArgumentException("Collection must contain at least one item.", nameof(knownPointDict));
            }

            _orderedKnownPoints = new SortedDictionary<double, Point<TypeY>>(knownPointDict);

            var firstPoint = _orderedKnownPoints.First();
            var lastPoint = _orderedKnownPoints.Last();
            KnownPointExtents = new KnownPointExtents<double, TypeY>(firstPoint.Key, firstPoint.Value.ValueY, lastPoint.Key, lastPoint.Value.ValueY);
        }
        public TypeY GetY(double x)
        {
            // Point is known.
            if (_orderedKnownPoints.TryGetValue(x, out Point<TypeY> found))
            {
                return found.ValueY;
            }
            // PositionX is before first known point.
            if (x < _orderedKnownPoints.ElementAt(0).Key)
            {
                // Unclamped interpolate by first two points.
                return InterpolateWithPointIndexes(0, 1, x);
            }
            // PositionX is after last known point.
            if (x > _orderedKnownPoints.Last().Key)
            {
                // Unclamped interpolate by last two points.
                return InterpolateWithPointIndexes(_orderedKnownPoints.Count - 2, _orderedKnownPoints.Count - 1, x);
            }

            // PositionX is between two known points.
            Point<TypeY> a = _orderedKnownPoints.Last(p => p.Key < x).Value;   // Last point with PositionX < provided X.
            Point<TypeY> b = _orderedKnownPoints.First(p => p.Key > x).Value;  // First point with PositionX > provided X.

            // Clamped interpolate between neighbor points.
            return InterpolateWithPoints(a, b, x);
        }
        private TypeY InterpolateWithPointIndexes(int indexOfA, int indexOfB, double posX)
        {
            Point<TypeY> a = _orderedKnownPoints.ElementAt(indexOfA).Value;
            Point<TypeY> b = _orderedKnownPoints.ElementAt(indexOfB).Value;

            return InterpolateWithPoints(a, b, posX);
        }
        private TypeY InterpolateWithPoints(Point<TypeY> a, Point<TypeY> b, double posX)
        {
            // Get interpolator.
            double t = GetT(a.PositionX, b.PositionX, posX);

            // Choose an interpolation to use. Use a's interpolation if we are not yet at b.
            var interpolationToUse = t < 1 ? a.InterpolationFunc : b.InterpolationFunc;

            return interpolationToUse(a.ValueY, b.ValueY, t);
        }
        private static double GetT(double a, double b, double value) => (value - b) / (a - b);

        /// <summary>
        /// Build a lookup table made up of pre-calculated results (you will not be able to get unspecified values).
        /// </summary>
        /// <param name="startingPositionX">the starting value</param>
        /// <param name="count">the number of entries that will be in the final table</param>
        /// <param name="skipDistance">the distance to skip before getting the next value (this may be negative, though the result will not be special)</param>
        /// <returns>new lookup table populated with specified entries</returns>
        public ValueLookupTable<double, TypeY> BuildValueLookupTable(double startingPositionX, int count, double skipDistance)
        {
            var newDict = new Dictionary<double, TypeY>();

            double firstX = 0, lastX = 0;
            TypeY firstY = default, lastY = default;

            double posX = startingPositionX;
            for (int i = 0; i < count; i++)
            {
                var valY = GetY(posX);

                if (i == 0)
                {
                    firstX = posX; firstY = valY;
                }
                if (i == count - 1)
                {
                    lastX = posX; lastY = valY;
                }

                newDict.Add(posX, valY);
                posX += skipDistance;
            }

            KnownPointExtents<double, TypeY> extents = new KnownPointExtents<double, TypeY>(firstX, firstY, lastX, lastY);

            return new ValueLookupTable<double, TypeY>(newDict, extents);
        }
        /// <summary>
        /// Build a lookup table made up of pre-calculated results (you will not be able to get unspecified values).
        /// </summary>
        /// <param name="positionXSet">collection of all x values to pre-calculate</param>
        /// <returns>new lookup table populated with specified entries</returns>
        public ValueLookupTable<double, TypeY> BuildValueLookupTable(ICollection<double> positionXSet)
        {
            var newDict = new Dictionary<double, TypeY>();

            double firstX = 0, lastX = 0;
            TypeY firstY = default, lastY = default;

            for (int i = 0; i < positionXSet.Count; i++)
            {
                double x = positionXSet.ElementAt(i);
                if (!newDict.ContainsKey(x))
                {
                    var valY = GetY(x);
                    if (i == 0)
                    {
                        firstX = x; firstY = valY;
                    }
                    if (i == positionXSet.Count - 1)
                    {
                        lastX = x; firstY = valY;
                    }

                    newDict.Add(x, valY);
                }
            }

            KnownPointExtents<double, TypeY> extents = new KnownPointExtents<double, TypeY>(firstX, firstY, lastX, lastY);

            return new ValueLookupTable<double, TypeY>(newDict, extents);
        }
        /// <summary>
        /// Build a lookup table made up of pre-calculated results (you will not be able to get unspecified values).
        /// </summary>
        /// <param name="startingPositionX">the starting value</param>
        /// <param name="count">the number of entries that will be in the final table</param>
        /// <param name="skipDistance">the distance to skip before getting the next value (this may be negative, though the result will not be special)</param>
        /// <returns>new lookup table populated with specified entries</returns>
        public ValueLookupTable<int, TypeY> BuildValueLookupTable(int startingPositionX, int count, int skipDistance)
        {
            var newDict = new Dictionary<int, TypeY>();

            int firstX = 0, lastX = 0;
            TypeY firstY = default, lastY = default;

            int posX = startingPositionX;
            for (int i = 0; i < count; i++)
            {
                var valY = GetY(posX);

                if (i == 0)
                {
                    firstX = posX; firstY = valY;
                }
                if (i == count - 1)
                {
                    lastX = posX; lastY = valY;
                }

                newDict.Add(posX, valY);
                posX += skipDistance;
            }

            KnownPointExtents<int, TypeY> extents = new KnownPointExtents<int, TypeY>(firstX, firstY, lastX, lastY);

            return new ValueLookupTable<int, TypeY>(newDict, extents);
        }
        /// <summary>
        /// Build a lookup table made up of pre-calculated results (you will not be able to get unspecified values).
        /// </summary>
        /// <param name="positionXSet">collection of all x values to pre-calculate</param>
        /// <returns>new lookup table populated with specified entries</returns>
        public ValueLookupTable<int, TypeY> BuildValueLookupTable(ICollection<int> positionXSet)
        {
            var newDict = new Dictionary<int, TypeY>();

            int firstX = 0, lastX = 0;
            TypeY firstY = default, lastY = default;

            for (int i = 0; i < positionXSet.Count; i++)
            {
                int x = positionXSet.ElementAt(i);
                if (!newDict.ContainsKey(x))
                {
                    var valY = GetY(x);
                    if (i == 0)
                    {
                        firstX = x; firstY = valY;
                    }
                    if (i == positionXSet.Count - 1)
                    {
                        lastX = x; firstY = valY;
                    }

                    newDict.Add(x, valY);
                }
            }

            KnownPointExtents<int, TypeY> extents = new KnownPointExtents<int, TypeY>(firstX, firstY, lastX, lastY);

            return new ValueLookupTable<int, TypeY>(newDict, extents);
        }
    }
}
