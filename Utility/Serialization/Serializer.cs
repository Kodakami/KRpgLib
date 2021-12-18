using System.Collections.Generic;

namespace KRpgLib.Utility.Serialization
{
    // AT SOME POINT: Serializer could be further genericized to Serializer<TObject, TOutput>.

    public abstract class Serializer<T> : ISerializer<T>
    {
        public bool TrySerialize(T obj, out IReadOnlyList<byte> bytes, int capacitySuggestion = 0)
        {
            var writerInProgress = new ByteWriter(capacitySuggestion);

            if (TrySerialize_Internal(obj, writerInProgress))
            {
                bytes = writerInProgress.GetBytes();
                return true;
            }

            bytes = default;
            return false;
        }
        public bool TrySerialize(T obj, ByteWriter byteWriter)
        {
            if (byteWriter != null)
            {
                return TrySerialize_Internal(obj, byteWriter);
            }

            return false;
        }
        protected abstract bool TrySerialize_Internal(T obj, ByteWriter writerInProgress);

        public bool TryDeserialize(IReadOnlyList<byte> bytes, int offset, out T obj)
        {
            var newReader = new ByteReader(bytes, offset);

            return TryDeserialize_Internal(newReader, out obj);
        }
        public bool TryDeserialize(ByteReader reader, out T obj)
        {
            if (reader != null)
            {
                return TryDeserialize_Internal(reader, out obj);
            }

            obj = default;
            return false;
        }
        protected abstract bool TryDeserialize_Internal(ByteReader readerInProgress, out T obj);
    }
}
