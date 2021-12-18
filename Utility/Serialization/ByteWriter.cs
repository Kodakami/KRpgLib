using System;
using System.Collections.Generic;
using System.Linq;

namespace KRpgLib.Utility.Serialization
{
    public sealed class ByteWriter
    {
        private readonly List<byte> _bytes;

        public ByteWriter(int capacitySuggestion = 0)
        {
            _bytes = new List<byte>(capacitySuggestion);
        }

        public void WriteBytes(IEnumerable<byte> bytes, bool reverseIfBigEndian)
        {
            byte[] input = bytes.ToArray();

            if (reverseIfBigEndian && !BitConverter.IsLittleEndian)
            {
                Array.Reverse(input);
            }

            _bytes.AddRange(input);
        }
        public byte[] GetBytes() => _bytes.ToArray();
    }
}
