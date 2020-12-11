namespace KRpgLib.Items
{
    public class ItemStack<TItem>  where TItem : Item
    {
        public TItem Item { get; }

        public int MaxStackSize { get; }

        private int _count;
        public int Count
        {
            get
            {
                return _count;
            }
            protected set
            {
                _count = System.Math.Max(0, System.Math.Min(MaxStackSize, value));
            }
        }
        public bool IsFull => Count == MaxStackSize;
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// Create an item stack for items which have a stacking component.
        /// </summary>
        /// <param name="item"></param>
        public ItemStack(TItem item)
        {
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }

            var stacking = item.GetKomponent<StackingComponent>();
            if (stacking == null)
            {
                throw new System.ArgumentException("Item must have a stacking component.", nameof(item));
            }

            Item = item;
            MaxStackSize = stacking.DefaultMaxStackSize;
        }
        /// <summary>
        /// Create an item stack with provided max stack size.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="maxStackSize"></param>
        public ItemStack(TItem item, int maxStackSize)
        {
            Item = item ?? throw new System.ArgumentNullException(nameof(item));
            MaxStackSize = maxStackSize > 1 ? maxStackSize : 1;
        }

        public void AddInstances(int addedCount, out int overflow)
        {
            if (addedCount < 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(addedCount));
            }

            if (addedCount == 0)
            {
                overflow = 0;
                return;
            }

            int oldCount = Count;
            Count += addedCount;
            overflow = addedCount - (Count - oldCount);
        }
        public void RemoveInstances(int removedCount, out int underflow)
        {
            if (removedCount < 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(removedCount));
            }

            if (removedCount == 0)
            {
                underflow = 0;
                return;
            }

            int oldCount = Count;
            Count -= removedCount;
            underflow = removedCount - (oldCount - Count);
        }
    }
}
