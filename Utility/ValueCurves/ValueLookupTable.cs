using System.Collections.Generic;

namespace KRpgLib.Utility.ValueCurves
{
    public struct ValueLookupTable<TypeX, TypeY>
    {
        private readonly Dictionary<TypeX, TypeY> _table;

        public KnownPointExtents<TypeX, TypeY> KnownPointExtents { get; }

        public ValueLookupTable(Dictionary<TypeX, TypeY> entries, KnownPointExtents<TypeX, TypeY> extents)
        {
            _table = new Dictionary<TypeX, TypeY>(entries);
            KnownPointExtents = extents;
        }
        public int Count => (_table?.Count) ?? 0;
        public bool TryGetValue(TypeX positionX, out TypeY valueY)
        {
            if (_table == null)
            {
                valueY = default;
                return false;
            }

            return _table.TryGetValue(positionX, out valueY);
        }
    }
}
