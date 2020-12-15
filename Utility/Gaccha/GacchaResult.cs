using System;
using System.Text;

namespace KRpgLib.Utility.Gaccha
{
    public struct GacchaResult<TCapsule>
    {
        /// <summary>
        /// The item to be returned by the gaccha.
        /// </summary>
        public TCapsule Capsule { get; }
        /// <summary>
        /// The number of instances of this item in the gaccha. This value must be greater than or equal to 0.
        /// </summary>
        public int Count { get; }
        public GacchaResult(TCapsule capsule, int count)
        {
            Capsule = capsule;   // Null is a valid option here.
            Count = count > 0 ? count : 0;
        }
    }
}
