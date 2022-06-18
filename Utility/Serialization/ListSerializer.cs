using System;
using System.Collections.Generic;

namespace KRpgLib.Utility.Serialization
{
    public abstract class ListSerializer<T> : Serializer<IReadOnlyList<T>>
    {
        // Layout:
        // Int32 List Count                 (0 ~ 3)
        // 1~N T Values                     (4 ~ X)     // X = ((N * sizeof(T)) - 1) + 4

        protected override bool TrySerialize_Internal(IReadOnlyList<T> values, ByteWriter writerInProgress)
        {
            if (values != null)
            {
                // First, write the list count.
                Int32Serializer.Singleton.SerializeImmediate(values.Count, writerInProgress);

                // Then, write all the values in the list.
                foreach (var value in values)
                {
                    // Try to serialize the individual value. Will write if successful.
                    if (!TrySerializeSingleValue(value, writerInProgress))
                    {
                        // If failure, then the whole thing is a bust.
                        return false;
                    }
                }

                // All values written.
                return true;
            }

            // Values argument was NULL.
            return false;
        }
        protected abstract bool TrySerializeSingleValue(T value, ByteWriter writerInProgress);

        protected override bool TryDeserialize_Internal(ByteReader readerInProgress, out IReadOnlyList<T> values)
        {
            var valueList = new List<T>();
            values = valueList;

            // Try and deserialize a list count. If it works and the value is not negative,
            if (Int32Serializer.Singleton.TryDeserialize(readerInProgress, out int listCount) && listCount >= 0)
            {
                // Unable to determine if enough bytes remain at this time (values may have variable byte sizes).

                // Read the correct number of values.
                for (int i = 0; i < listCount; i++)
                {
                    // Try to deserialize next individual value.
                    if (!TryDeserializeSingleValue(readerInProgress, out T nextValue))
                    {
                        // Value failed to deserialize. The whole thing is a bust.
                        return false;
                    }

                    valueList.Add(nextValue);
                }

                // All values read.
                return true;
            }

            // Not enough bytes remain to read the list count, or list count was negative.
            return false;
        }
        protected abstract bool TryDeserializeSingleValue(ByteReader readerInProgress, out T singleValue);
    }
    public sealed class UniformListSerializer<T> : ListSerializer<T>
    {
        private readonly Serializer<T> _valueSerializer;

        public UniformListSerializer(Serializer<T> singleValueSerializer)
        {
            _valueSerializer = singleValueSerializer ?? throw new ArgumentNullException(nameof(singleValueSerializer));
        }

        protected override bool TryDeserializeSingleValue(ByteReader readerInProgress, out T singleValue)
        {
            return _valueSerializer.TryDeserialize(readerInProgress, out singleValue);
        }

        protected override bool TrySerializeSingleValue(T value, ByteWriter writerInProgress)
        {
            return _valueSerializer.TrySerialize(value, writerInProgress);
        }
    }
}
