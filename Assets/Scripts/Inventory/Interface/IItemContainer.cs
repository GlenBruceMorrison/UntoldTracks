using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Tracks.Inventory
{
    public delegate void ItemContainerModified(IItemContainer oldValue, IItemContainer newValue);

    public interface IItemContainer
    {
        public Item Item { get; }
        public int Count { get; }
        public IInventory Inventory { get; }
        public int Index { get; }

        public event ItemContainerModified OnModified;

        public bool IsFull();
        public int RemainingSpace();
        public bool IsEmpty();
        public bool IsNotEmpty();
        public bool HasItem(Item item);
        public void Swap(IItemContainer container);
        public int FillAndReturnRemaining(Item item, int count, bool modifyContainer = true);
        public int TakeAndReturnRemaining(int count, bool modifyContainer = true);
        public bool CanFill(Item item, int count);
        public IItemContainer TakeAll();
        public void Empty();
    }
}