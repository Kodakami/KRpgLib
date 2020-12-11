using System;
using System.Collections.Generic;

namespace KRpgLib.Items
{
    /// <summary>
    /// A collection of items in the form of a limited number of slots that can be filled with stacks of any subclass of a kind of item.
    /// </summary>
    public abstract class ItemPocket<TItemStack, TItem>
        where TItemStack : ItemStack<TItem>
        where TItem : Item
    {
        // Slots may be (and start out as) null.
        private readonly List<TItemStack> _slotList;

        /// <summary>
        /// If null, uses default max stack size for each item stack.
        /// </summary>
        protected int? CustomMaxStackSize { get; }

        /// <summary>
        /// The number of slots (empty or full) in the collection.
        /// </summary>
        public int SlotCount { get; }
        /// <summary>
        /// The number of item stacks in the collection. In other words, the number of slots that have at least one item instance in them.
        /// </summary>
        public int ItemStackCount => _slotList.FindAll(s => s != null).Count;
        /// <summary>
        /// True if the collection contains no items.
        /// </summary>
        public bool IsEmpty => _slotList.TrueForAll(s => s == null);
        /// <summary>
        /// True if every slot has at least one item in it (though that stack may or may not be full). There is no space for new stacks of items.
        /// </summary>
        public bool HasMaxStacks => _slotList.TrueForAll(s => s != null);
        /// <summary>
        /// True if every slot has a full stack of items in it. There is no remaining space for items of any kind.
        /// </summary>
        public bool IsCompletelyFull => _slotList.TrueForAll(s => s?.IsFull == true);

        private ItemPocket(int slotCount, int? customMaxStackSize)
        {
            SlotCount = slotCount > 1 ? slotCount : 1;
            _slotList = new List<TItemStack>();

            // Populate with null (empty) slots.
            for (int i = 0; i < SlotCount; i++)
            {
                _slotList.Add(null);
            }

            CustomMaxStackSize = customMaxStackSize;
        }
        /// <summary>
        /// Create an item pocket which holds up to a given number of stacks of items. If the given value is less than 1, it will be set to 1 instead. This constructor makes an item pocket with a custom max stack size, which overrides the default one for each item stack in it.
        /// </summary>
        protected ItemPocket(int slotCount, int customMaxStackSize)
            :this(slotCount, (int?)customMaxStackSize) { }
        /// <summary>
        /// Create an item pocket which holds up to a given number of stacks of items. If the given value is less than 1, it will be set to 1 instead. This constructor makes an item pocket which lets each type of item determine its own max stack size (or 1 if it does not have a stacking component).
        /// </summary>
        protected ItemPocket(int slotCount)
            :this(slotCount, null) { }

        /// <summary>
        /// True if the collection contains at least one instance of the provided item.
        /// </summary>
        public bool HasItem(TItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!IsItemValidForPocket(item))
            {
                return false;
            }

            return _slotList.Find(s => s != null && s.Item == item) != null;
        }
        /// <summary>
        /// Counts the number of item instances in the collection which match the provided predicate.
        /// </summary>
        public int GetItemCount(Predicate<TItem> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            int totalCount = 0;
            foreach (var slot in _slotList.FindAll(s => s != null && predicate(s.Item)))
            {
                totalCount += slot.Count;
            }
            return totalCount;
        }
        /// <summary>
        /// Counts the number of instances of the provided item in the collection.
        /// </summary>
        public int GetItemCount(TItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return GetItemCount(i => i == item);
        }
        /// <summary>
        /// Counts the total number of item instances in the collection.
        /// </summary>
        public int GetItemCount()
        {
            int totalCount = 0;
            foreach (var slot in _slotList.FindAll(s => s != null))
            {
                totalCount += slot.Count;
            }
            return totalCount;
        }

        /// <summary>
        /// True if there is enough room in the collection to fit the provided amount of the provided item. This method accounts for non-full stacks of the item as well as empty slots to be filled.
        /// </summary>
        public bool HasSpaceForItems(TItem item, int addedCount)
        {
            // TODO: This method could benefit from a short-circuiting behavior.

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!IsItemValidForPocket(item))
            {
                return false;
            }

            return CountAvailableSpaceForItem(item) >= addedCount;
        }
        /// <summary>
        /// Counts the amount of space available for the provided item. This method accounts for non-full stacks of the item as well as empty slots to be filled.
        /// </summary>
        public int CountAvailableSpaceForItem(TItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!IsItemValidForPocket(item))
            {
                return 0;
            }

            // Get a handle for max stack size ofr this item (in the context of this container).
            int maxStackSize;

            // If there is no custom max stack size,
            if (CustomMaxStackSize == null)
            {
                // Try to get a stacking component.
                var stacking = item.GetKomponent<StackingComponent>();

                // If there is one, then use the default max stack size for the item.
                if (stacking != null)
                {
                    maxStackSize = stacking.DefaultMaxStackSize;
                }
                // Otherwise, max stack size is 1.
                else
                {
                    maxStackSize = 1;
                }
            }
            // Otherwise,
            else
            {
                // Use the custom max stack size.
                maxStackSize = CustomMaxStackSize.Value;
            }

            // Keep track of space remaining.
            int totalSpaceRemaining = 0;

            // For each slot in this container.
            foreach (var slot in _slotList)
            {
                // If the slot is null (empty),
                if (slot == null)
                {
                    // Add the max stack size to the total (we could put an entire stack here).
                    totalSpaceRemaining += maxStackSize;
                }
                // Otherwise, if the slot is for this type of item and it is not full,
                else if (slot.Item == item && !slot.IsFull)
                {
                    // Add the remaining space to the total.
                    totalSpaceRemaining += slot.MaxStackSize - slot.Count;
                }
            }
            return totalSpaceRemaining;
        }

        /// <summary>
        /// Add items to the collection. The amount that could not fit is returned in the out parameter.
        /// </summary>
        public void AddItems(TItem item, int addedCount, out int overflow)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            overflow = addedCount;

            if (!IsItemValidForPocket(item))
            {
                return;
            }

            // While there are still items to put away,
            while (overflow > 0)
            {
                // Try to find an existing stack that isn't full.
                var stack = _slotList.Find(s => s != null && s.Item == item && !s.IsFull);

                // If there is no existing non-full stack,
                if (stack == null)
                {
                    // If there is no room for more stacks,
                    if (HasMaxStacks)
                    {
                        // No more room. Leave method with current overflow value.
                        break;
                    }

                    // Otherwise, create a new stack.
                    if (CustomMaxStackSize == null)
                    {
                        stack = CreateNewStackInternal_DefaultMaxStackSize(item);
                    }
                    else
                    {
                        stack = CreateNewStackInternal_CustomMaxStackSize(item, CustomMaxStackSize.Value);
                    }

                    // And put it in the first empty slot.
                    int emptyIndex = _slotList.FindIndex(s => s == null);
                    _slotList[emptyIndex] = stack;
                }

                // Regardless of where the stack came from, add the instances and update the overflow value.
                stack.AddInstances(overflow, out overflow);
            }
        }
        protected abstract TItemStack CreateNewStackInternal_DefaultMaxStackSize(TItem item);
        protected abstract TItemStack CreateNewStackInternal_CustomMaxStackSize(TItem item, int maxStackSize);

        /// <summary>
        /// Remove items from the collection. The amount that could not be removed is returned in the out parameter.
        /// </summary>
        public void RemoveItems(TItem item, int removedCount, out int underflow)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            underflow = removedCount;

            if (!IsItemValidForPocket(item))
            {
                return;
            }

            // While there are still items to remove,
            while (underflow > 0)
            {
                // Try to find an existing stack.
                var stack = _slotList.Find(s => s != null && s.Item == item);

                // If there is no existing stack,
                if (stack == null)
                {
                    // No more items. Leave method with current underflow value.
                    break;
                }

                // Remove the instances and update the underflow value.
                stack.RemoveInstances(underflow, out underflow);

                // If this process left the stack empty,
                if (stack.IsEmpty)
                {
                    // Get the slot index it was in.
                    int emptyStackIndex = _slotList.IndexOf(stack);

                    // And set that to null.
                    _slotList[emptyStackIndex] = null;
                }
            }
        }
        /// <summary>
        /// Try to get an item stack from the collection at the specified index. Returns false if the slot was empty.
        /// </summary>
        public bool TryGetItemStackAtIndex(int index, out TItemStack itemStack)
        {
            if (!IsSlotIndexInRange(index))
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            itemStack = _slotList[index];
            return itemStack != null;
        }
        /// <summary>
        /// Take the contents of two slots (whether or not they have any items), and swap them.
        /// </summary>
        public void SwapItemStackIndexes(int indexA, int indexB)    // Same as "MoveStack".
        {
            // checks?
            if (!IsSlotIndexInRange(indexA))
            {
                throw new ArgumentOutOfRangeException(nameof(indexA));
            }
            if (!IsSlotIndexInRange(indexB))
            {
                throw new ArgumentOutOfRangeException(nameof(indexB));
            }

            if (indexA == indexB)
            {
                return;
            }

            var oldStackA = _slotList[indexA];
            _slotList[indexA] = _slotList[indexB];
            _slotList[indexB] = oldStackA;
        }
        /// <summary>
        /// Returns true if the slot index represents a valid slot in the collection (zero-based index).
        /// </summary>
        protected bool IsSlotIndexInRange(int slotIndex)
        {
            return slotIndex >= 0 && slotIndex < SlotCount;
        }
        /// <summary>
        /// This method is called before counting, adding, or removing items from the collection. Override to define custom item validation. Default value is always true.
        /// </summary>
        protected virtual bool IsItemValidForPocket(TItem item) => true;
    }
    public class BasicItemPocket : ItemPocket<ItemStack<Item>, Item>
    {
        public BasicItemPocket(int slotCount, int customMaxStackSize)
            :base(slotCount, customMaxStackSize) { }

        public BasicItemPocket(int slotCount)
            :base(slotCount) { }

        protected override ItemStack<Item> CreateNewStackInternal_CustomMaxStackSize(Item item, int maxStackSize)
        {
            return new ItemStack<Item>(item, maxStackSize);
        }

        protected override ItemStack<Item> CreateNewStackInternal_DefaultMaxStackSize(Item item)
        {
            return new ItemStack<Item>(item);
        }
    }
}
