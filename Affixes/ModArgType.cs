using KRpgLib.Utility.Serialization;
using System;
using System.Collections.Generic;

namespace KRpgLib.Affixes
{
    public abstract class ModArgType : ISerializer<object>
    {
        public abstract bool TrySerialize(object obj, ByteWriter byteWriter);
        public abstract bool TrySerialize(object obj, out IReadOnlyList<byte> bytes, int capacitySuggestion = 0);
        public abstract bool TryDeserialize(ByteReader reader, out object obj);
        public abstract bool TryDeserialize(IReadOnlyList<byte> bytes, int offset, out object obj);
    }
    public class ModArgType<T> : ModArgType, ISerializer<T>
    {
        // Strongly-typed serializer.
        private readonly Serializer<T> _serializer;

        // Ctor.
        public ModArgType(Serializer<T> serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        // Serialization for weakly-typed arg (casts and passes).
        public override bool TrySerialize(object weakValue, out IReadOnlyList<byte> bytes, int capacitySuggestion = 0)
        {
            // Type check.
            if (weakValue is T strongValue)
            {
                return TrySerialize(strongValue, out bytes, capacitySuggestion);
            }

            bytes = default;
            return false;
        }
        public override bool TrySerialize(object weakValue, ByteWriter byteWriter)
        {
            // Type check.
            return weakValue is T strongValue && TrySerialize(strongValue, byteWriter);
        }
        public override bool TryDeserialize(IReadOnlyList<byte> bytes, int offset, out object weakValue)
        {
            // Type check.
            if (TryDeserialize(bytes, offset, out T strongValue))
            {
                weakValue = strongValue;
                return true;
            }

            weakValue = default;
            return false;
        }
        public override bool TryDeserialize(ByteReader reader, out object weakValue)
        {
            // Type check.
            if (TryDeserialize(reader, out T strongValue))
            {
                weakValue = strongValue;
                return true;
            }

            weakValue = default;
            return false;
        }

        // Serialization for strongly-typed arg (passes to serializer).
        public bool TrySerialize(T obj, ByteWriter byteWriter) => _serializer.TrySerialize(obj, byteWriter);
        public bool TrySerialize(T obj, out IReadOnlyList<byte> bytes, int capacitySuggestion = 0) => _serializer.TrySerialize(obj, out bytes, capacitySuggestion);
        public bool TryDeserialize(ByteReader reader, out T obj) => _serializer.TryDeserialize(reader, out obj);
        public bool TryDeserialize(IReadOnlyList<byte> bytes, int offset, out T obj) => _serializer.TryDeserialize(bytes, offset, out obj);
    }
}