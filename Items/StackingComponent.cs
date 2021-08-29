using System;

namespace KRpgLib.Items
{
    public class StackingComponent : IItemComponent
    {
        private const int MINIMUM_MAX_STACK_SIZE = 1;

        public int DefaultMaxStackSize { get; }
        public StackingComponent(int defaultMaxStackSize)
        {
            DefaultMaxStackSize = Math.Max(defaultMaxStackSize, MINIMUM_MAX_STACK_SIZE);
        }
    }
}
