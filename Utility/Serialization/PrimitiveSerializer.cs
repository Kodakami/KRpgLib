using System;
using System.Collections.Generic;

namespace KRpgLib.Utility.Serialization
{
    public abstract class PrimitiveSerializer<T> : Serializer<T>
    {
        // Can only be implemented in this assembly.

        internal PrimitiveSerializer() { }

        protected delegate byte[] BitConverterGetBytesDelegate(T obj);
        protected delegate T BitConverterToValueTypeDelegate(byte[] bytes, int startingIndex);

        public abstract int SizeOfValueType();
        protected abstract BitConverterGetBytesDelegate GetBytesDelegate();
        protected abstract BitConverterToValueTypeDelegate ToValueTypeDelegate();

        protected override bool TrySerialize_Internal(T obj, ByteWriter writerInProgress)
        {
            // Serialization is guaranteed.
            SerializeImmediate(obj, writerInProgress);
            return true;
        }
        protected override bool TryDeserialize_Internal(ByteReader readerInProgress, out T obj)
        {
            int sizeOf = SizeOfValueType();

            // If there are enough bytes to read the primitive,
            if (readerInProgress.Remaining >= sizeOf)
            {
                var bytes = readerInProgress.GetNextAndAdvance(sizeOf, true);
                obj = ToValueTypeDelegate().Invoke(bytes, 0);
                return true;
            }

            // Not enough remaining bytes.
            obj = default;
            return false;
        }

        // We can trust that serialization of C# primitives will always succeed, so we can implement direct convenience methods for serialization.
        public void SerializeImmediate(T obj, out IReadOnlyList<byte> bytes)
        {
            bytes = GetBytesDelegate().Invoke(obj);
        }
        public void SerializeImmediate(T obj, ByteWriter writerInProgress)
        {
            SerializeImmediate(obj, out IReadOnlyList<byte> bytes);

            writerInProgress.WriteBytes(bytes, true);
        }
    }
    public sealed class Int32Serializer : PrimitiveSerializer<int>
    {
        public override int SizeOfValueType() => sizeof(int);
        protected override BitConverterGetBytesDelegate GetBytesDelegate() => BitConverter.GetBytes;
        protected override BitConverterToValueTypeDelegate ToValueTypeDelegate() => BitConverter.ToInt32;
    }
    public sealed class Float32Serializer : PrimitiveSerializer<float>
    {
        public override int SizeOfValueType() => sizeof(float);
        protected override BitConverterGetBytesDelegate GetBytesDelegate() => BitConverter.GetBytes;
        protected override BitConverterToValueTypeDelegate ToValueTypeDelegate() => BitConverter.ToSingle;
    }
    public sealed class UInt32Serializer : PrimitiveSerializer<uint>
    {
        public override int SizeOfValueType() => sizeof(uint);
        protected override BitConverterGetBytesDelegate GetBytesDelegate() => BitConverter.GetBytes;
        protected override BitConverterToValueTypeDelegate ToValueTypeDelegate() => BitConverter.ToUInt32;
    }
}
