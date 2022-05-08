using System;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections.Generic;

namespace UntoldTracks.Inventory
{
    [System.Serializable]
    public class ItemContainer : IItemContainer
    {
        #region private
        [SerializeField]
        private Item _item;
        [SerializeField]
        private int _count;
        private IInventory _inventory;
        private int _index;
        #endregion

        #region getters
        public Item Item
        {
            get
            {
                return _item;
            }
        }

        public int Count
        {
            get
            {
                return _count;
            }
        }

        public IInventory Inventory
        {
            get
            {
                return _inventory;
            }
        }

        public int Index
        {
            get
            {
                return _index;
            }
        }
        #endregion

        #region events
        public event ItemContainerModified OnModified;
        #endregion

        public ItemContainer(IInventory inventory, int index)
        {
            _inventory = inventory;
            _index = index;
        }

        public bool IsFull()
        {
            if (IsEmpty())
            {
                return false;
            }

            if (!_item.stackable)
            {
                return true;
            }

            return _count >= _item.stackSize;
        }

        public int RemainingSpace()
        {
            // cannot tell how much space is remaining if we don't have a max stack size to reference
            if (IsEmpty())
            {
                return -1;
            }

            return _item.stackSize - _count;
        }

        public bool IsEmpty()
        {
            return _item == null;
        }

        public bool IsNotEmpty()
        {
            return _item != null;
        }

        public bool HasItem(Item item)
        {
            return _item == item;
        }

        public int FillAndReturnRemaining(Item item, int count, bool modifyContainer = true)
        {
            if (IsNotEmpty() && !HasItem(item))
            {
                throw new ArgumentException($"This container contains {item.name}, trying to add {_item.name}");
            }

            if (count < 1)
            {
                return count;
            }

            if (IsEmpty())
            {
                if (modifyContainer)
                {
                    _item = item;
                }
                else
                {
                    if (!item.stackable)
                    {
                        return 0;
                    }

                    if (count > item.stackSize)
                    {
                        return count - item.stackSize;
                    }

                    return 0;
                }
            }

            if (!item.stackable)
            {
                return count;
            }

            var prev = (IItemContainer)this.MemberwiseClone();

            var diff = _item.stackSize - (_count + count);

            if (diff < 0)
            {
                if (modifyContainer)
                {
                    _count = _item.stackSize;
                    OnModified?.Invoke(prev, this);
                }
                return -diff;
            }

            if (modifyContainer)
            {
                _count += count;
                OnModified?.Invoke(prev, this);
            }

            return 0;
        }

        public int TakeAndReturnRemaining(int count, bool modifyContainer = true)
        {
            if (IsEmpty())
            {
                throw new Exception("Trying to take items from an empty container");
            }

            var prev = (IItemContainer)this.MemberwiseClone();

            var diff = _count - count;

            if (diff < 0)
            {
                if (modifyContainer)
                {
                    _count = 0;
                    _item = null;
                    OnModified?.Invoke(prev, this);
                }

                return -diff;
            }

            if (modifyContainer)
            {
                _count -= count;

                if (_count == 0)
                {
                    _item = null;
                }

                OnModified?.Invoke(prev, this);
            }

            return 0;
        }

        public bool CanFill(Item item, int count)
        {
            if (IsEmpty())
            {
                return true;
            }

            if (!HasItem(item))
            {
                return false;
            }

            if (!_item.stackable)
            {
                return false;
            }

            var proposedTotal = _count + count;

            if (proposedTotal > _item.stackSize)
            {
                return false;
            }

            return true;
        }

        public void Empty()
        {
            var prev = (IItemContainer)this.MemberwiseClone();

            _item = null;
            _count = 0;

            OnModified?.Invoke(prev, this);
        }

        public IItemContainer TakeAll()
        {
            var prev = (IItemContainer)this.MemberwiseClone();

            _item = null;
            _count = 0;

            OnModified?.Invoke(prev, this);

            return prev;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }

        public void Swap(IItemContainer container)
        {
            var prev = (IItemContainer)this.MemberwiseClone();

            _item = container.Item;
            _count = container.Count;

            container.Empty();
            container.FillAndReturnRemaining(prev.Item, prev.Count);

            OnModified(prev, this);
        }
    }
}
