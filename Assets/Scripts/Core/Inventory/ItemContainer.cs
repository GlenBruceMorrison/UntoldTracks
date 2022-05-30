using System;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections.Generic;

namespace UntoldTracks.InventorySystem
{
    public delegate void ItemContainerModified(ItemContainer newValue);

    [System.Serializable]
    public class ItemContainer
    {
        #region private
        [SerializeField]
        private Item _item;
        [SerializeField]
        private int _count;
        private int _currentDurability;
        private Inventory _inventory;
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

        public Inventory Inventory
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

        public int CurrentDurability
        {
            get
            {
                return _currentDurability;
            }
        }
        #endregion

        #region events
        public event ItemContainerModified OnModified;
        #endregion

        #region constructors
        public ItemContainer(Inventory inventory, int index)
        {
            _inventory = inventory;
            _index = index;
        }
         
        public ItemContainer(Item item, int count, int durability=-1)
        {
            _inventory = null;
            _index = -1;
            _item = item;
            _count = count;
            _currentDurability = durability == -1 ? item.durability : durability;
        }
        #endregion

        /// <summary>
        /// Checks whether this container contains nothing
        /// </summary>
        /// <returns>Returns true if this container contains nothing</returns>
        public bool IsEmpty()
        {
            return _item == null;
        }

        /// <summary>
        /// Checks whether this container contains something
        /// </summary>
        /// <returns>Returns true if this contains something</returns>
        public bool IsNotEmpty()
        {
            return _item != null;
        }

        /// <summary>
        /// Check whether the container contains the given item
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <returns></returns>
        public bool HasItem(Item item)
        {
            return _item == item;
        }

        /// <summary>
        /// Empty this container completely
        /// </summary>
        public void Empty()
        {
            _item = null;
            _count = 0;

            OnModified?.Invoke(this);
        }

        /// <summary>
        /// Swaps the contents of this container with another container
        /// </summary>
        /// <param name="container">The other container to swap with</param>
        public void Swap(ItemContainer container)
        {
            var prev = (ItemContainer)this.MemberwiseClone();

            _item = container.Item;
            _count = container.Count;
            _currentDurability = container.CurrentDurability;

            container.Empty();
            container.Give(prev);

            OnModified?.Invoke(this);
        }

        /// <summary>
        /// Empty this container completely and return the contents
        /// </summary>
        /// <returns>
        /// Returns the contents of this container
        /// </returns>
        public ItemContainer TakeAll()
        {
            var prev = (ItemContainer)this.MemberwiseClone();

            _item = null;
            _count = 0;

            OnModified?.Invoke(this);

            return prev;
        }

        /// <summary>
        /// Attempt to take items from this contianer.
        /// </summary>
        /// <param name="query">
        /// An instance of the ItemQuery class which provides query
        /// information on what you want to take from this container
        /// </param>
        /// <returns>
        /// Returns a ItemQueryResult instance, that 
        /// provides information on the result of this query.
        /// </returns>
        public ItemQueryResult Take(ItemQuery query)
        {
            var itemQueryResult = new ItemQueryResult(query.item, 0);

            // not valid
            if (!HasItem(query.item) || Count <= 0)
            {
                return itemQueryResult;
            }

            // if item is not stackable
            if (!_item.stackable)
            {
                // if it is degradable, tranfer it's degrade value
                if (_item.degradable)
                {
                    itemQueryResult.durability = _currentDurability;
                }

                // non stackables have to be 1, so set this value
                itemQueryResult.amountAdded = 1;
                Empty();
            }
            else
            {
                // get difference between request and count
                var diff = _count - query.count;

                // if we can't give all, return what we couldn't give
                if (diff < 0)
                {
                    _item = null;
                    _count = 0;
                    itemQueryResult.amountAdded = -diff;
                }

                _count -= query.count;

                if (_count <= 0)
                {
                    _item = null;
                    _count = 0;
                }

                OnModified?.Invoke(this);
            }

            return itemQueryResult;
        }

        /// <summary>
        /// Attempt to give this container data. 
        /// </summary>
        /// <param name="container">
        /// The item contianer containing the item and the amount
        /// you want to attempt to add
        /// </param>
        /// <returns>
        /// Returns a ItemQueryResult instance, that 
        /// provides information on the result of this query.
        /// </returns>
        public ItemQueryResult Give(ItemContainer container)
        {
            var itemQueryResult = new ItemQueryResult(container.Item, container.CurrentDurability);

            // have a different item already, no point in continuing
            if (!HasItem(container.Item) && !IsEmpty())
            {
                return itemQueryResult;
            }

            // item is non stackable, so has more unique requirments
            if (!container.Item.stackable)
            {
                // non stackable items can only have one, so has to be empty to continue
                if (IsEmpty())
                {
                    this._item = container.Item;
                    this._count = 1;

                    if (container.Item.degradable)
                    {
                        this._currentDurability = container.CurrentDurability;
                    }

                    OnModified?.Invoke(this);
                }

                return itemQueryResult;
            }

            // if is empty, just fill with what we are provided
            if (IsEmpty())
            {
                this._item = container.Item;

                // sometimes we might pass a container with item count more that max stack size
                var extra = container.Item.stackSize - container.Count;

                // passing in more than the items max stack size, so just add the items max stack size
                if (extra < 0)
                {
                    _count = container.Item.stackSize;
                    itemQueryResult.amountAdded = container.Item.stackSize;
                }
                // passing in less than the max, so give it all
                else
                {
                    _count = container.Count;
                    itemQueryResult.amountAdded = container.Count;
                }

                OnModified?.Invoke(this);

                return itemQueryResult;
            }

            // if here then item is the same as the one adding and is not stackable
            
            // already full
            if (_count >= Item.stackSize)
            {
                return itemQueryResult;
            }

            // difference, minus means that we are trying to add too much
            var diff = container.Item.stackSize - (container.Count + _count);

            // giving too much
            if (diff < 0)
            {
                itemQueryResult.amountAdded = container.Item.stackSize - _count;
                _count = container.Item.stackSize;
            }
            // can take everything we are giving
            else
            {
                _count += container.Count;
                itemQueryResult.amountAdded = container.Count;
            }

            OnModified?.Invoke(this);
            return itemQueryResult;
        }

        /// <summary>
        /// Whether this container contains the given item and amount.
        /// </summary>
        /// <param name="item">
        /// The target item
        /// </param>
        /// <param name="count">
        /// The amount needed
        /// </param>
        /// <returns>
        /// Returns whether this does contain the given values
        /// </returns>
        public bool CanTake(Item item, int count=1)
        {
            // this container is empty, so can't return anything
            if (IsEmpty())
            {
                return false;
            }

            // has the same item
            if (HasItem(item))
            {
                // return whether we gave enough of this or not
                return (_count >= count);
            }

            return false;
        }

        /// <summary>
        /// Whether this container can take the given item and amount
        /// </summary>
        /// <param name="item">
        /// The target item
        /// </param>
        /// <param name="count">
        /// The amount needed
        /// </param>
        /// <returns>
        /// Returns whether this container can take the full amount
        /// </returns>
        public bool CanGive(Item item, int count=1)
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

        public void SetDurability(int value)
        {
            _currentDurability = value;
        }

        public void DecreaseDurability(int amount)
        {
            _currentDurability -= amount;
            if (_currentDurability <= 0)
            {
                Empty();
            }
            else
            {
               OnModified?.Invoke(this);
            }
        }

        internal int FillAndReturnRemaining(Item item, int count, bool modifyContainer = true)
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
            else
            {
                if (!_item.stackable)
                {
                    return count;
                }
            }

            if (!item.stackable)
            {
                if (modifyContainer)
                {
                    _count = 1;
                    OnModified?.Invoke(this);
                }

                return count - 1;
            }

            var diff = _item.stackSize - (_count + count);

            if (diff < 0)
            {
                if (modifyContainer)
                {
                    _count = _item.stackSize;
                    OnModified?.Invoke(this);
                }
                return -diff;
            }

            if (modifyContainer)
            {
                _count += count;
                OnModified?.Invoke(this);
            }

            return 0;
        }

        internal int TakeAndReturnRemaining(int count, bool modifyContainer = true)
        {
            if (IsEmpty())
            {
                throw new Exception("Trying to take items from an empty container");
            }

            var diff = _count - count;

            if (diff < 0)
            {
                if (modifyContainer)
                {
                    _count = 0;
                    _item = null;
                    OnModified?.Invoke(this);
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

                OnModified?.Invoke(this);
            }

            return 0;
        }
    }
}
