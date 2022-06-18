using System.Collections.Generic;

namespace KRpgLib.Utility.Serialization
{
    public interface ISerializer<T>
    {
        bool TrySerialize(T obj, ByteWriter byteWriter);
        bool TrySerialize(T obj, out IReadOnlyList<byte> bytes, int capacitySuggestion = 0);
        bool TryDeserialize(ByteReader reader, out T obj);
        bool TryDeserialize(IReadOnlyList<byte> bytes, int offset, out T obj);
    }
}