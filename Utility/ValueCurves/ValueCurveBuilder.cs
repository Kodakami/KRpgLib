using System.Collections.Generic;

namespace KRpgLib.Utility.ValueCurves
{
    public sealed class ValueCurveBuilder<TypeY>
    {
        private readonly SortedDictionary<double, Point<TypeY>> _sortedDict = new SortedDictionary<double, Point<TypeY>>();
        public bool TryAddKnownPoint(double positionX, TypeY valueY, InterpolationDelegate<TypeY> interpolationFunc)
        {
            if (!_sortedDict.ContainsKey(positionX))
            {
                // If interpolation function is null, use threshold interpolation ctor.
                var newPoint = interpolationFunc != null ? new Point<TypeY>(positionX, valueY, interpolationFunc) : new Point<TypeY>(positionX, valueY);

                _sortedDict.Add(positionX, newPoint);
                return true;
            }
            return false;
        }
        public bool TryBuildValueCurve(out ValueCurve<TypeY> valueCurve)
        {
            if (_sortedDict.Count >= 2)
            {
                valueCurve = new ValueCurve<TypeY>(_sortedDict);
                return true;
            }
            valueCurve = default;
            return false;
        }
        public bool TryBuildValueLookupTable(double startingPositionX, int count, double skipDistance, out ValueLookupTable<double, TypeY> valueLookupTable)
        {
            if (TryBuildValueCurve(out ValueCurve<TypeY> curve))
            {
                valueLookupTable = curve.BuildValueLookupTable(startingPositionX, count, skipDistance);
                return true;
            }
            valueLookupTable = default;
            return false;
        }
        public bool TryBuildValueLookupTable(ICollection<double> positionXSet, out ValueLookupTable<double, TypeY> valueLookupTable)
        {
            if (positionXSet != null && TryBuildValueCurve(out ValueCurve<TypeY> curve))
            {
                valueLookupTable = curve.BuildValueLookupTable(positionXSet);
                return true;
            }

            valueLookupTable = default;
            return false;
        }
        public bool TryBuildValueLookupTable(int startingPositionX, int count, int skipDistance, out ValueLookupTable<int, TypeY> valueLookupTable)
        {
            if (TryBuildValueCurve(out ValueCurve<TypeY> curve))
            {
                valueLookupTable = curve.BuildValueLookupTable(startingPositionX, count, skipDistance);
                return true;
            }
            valueLookupTable = default;
            return false;
        }
        public bool TryBuildValueLookupTable(ICollection<int> positionXSet, out ValueLookupTable<int, TypeY> valueLookupTable)
        {
            if (positionXSet != null && TryBuildValueCurve(out ValueCurve<TypeY> curve))
            {
                valueLookupTable = curve.BuildValueLookupTable(positionXSet);
                return true;
            }

            valueLookupTable = default;
            return false;
        }
    }
}
