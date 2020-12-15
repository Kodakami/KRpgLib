using System;

namespace KRpgLib.Items
{
    public class StackingComponent : ItemComponent
    {
        private const string EXCEPTION_MESSAGE = "Argument must be greater than zero.";

        public int DefaultMaxStackSize { get; }
        public StackingComponent(int defaultMaxStackSize)
        {
            ClearArgOrThrow(defaultMaxStackSize, nameof(defaultMaxStackSize));

            DefaultMaxStackSize = defaultMaxStackSize;
        }
        // There used to be more args. Now this seems like silly overengineering. Still, what if I add more args again?
        private void ClearArgOrThrow(int argValue, string argName)
        {
            if (argValue < 1)
            {
                throw new ArgumentOutOfRangeException(argName, EXCEPTION_MESSAGE);
            }
        }
    }
}
