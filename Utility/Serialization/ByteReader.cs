using System;
using System.Linq;
using System.Collections.Generic;

namespace KRpgLib.Utility.Serialization
{
    public sealed class ByteReader
    {
        private readonly byte[] _byteArray;
        private int _pointer;   // <-- points to the most recent value given.

        public int Remaining => _byteArray.Length - _pointer;

        public ByteReader(IReadOnlyList<byte> bytes, int offset)
        {
            _byteArray = bytes.Skip(offset).ToArray();
            _pointer = -1;
        }
        public byte[] GetNextButDoNotAdvance(int count, bool reverseIfBigEndian)
        {
            return GetNext_Internal(count, reverseIfBigEndian, false);
        }
        public byte[] GetNextAndAdvance(int count, bool reverseIfBigEndian)
        {
            return GetNext_Internal(count, reverseIfBigEndian, true);
        }
        public byte[] GetNext_Internal(int count, bool reverseIfBigEndian, bool advance)
        {
            if (count > 0 && count <= Remaining)
            {
                byte[] output = new byte[count];

                for (int i = 0; i < count; i++)
                {
                    if (advance)
                    {
                        output[i] = _byteArray[++_pointer]; // <-- Increment, then check value.
                    }
                    else
                    {
                        output[i] = _byteArray[_pointer + i];
                    }
                }

                // Reverse byte order if requested and relevant.
                if (reverseIfBigEndian && !BitConverter.IsLittleEndian)
                {
                    Array.Reverse(output);
                }

                return output;
            }

            throw new ArgumentOutOfRangeException(nameof(count));
        }
    }
}
