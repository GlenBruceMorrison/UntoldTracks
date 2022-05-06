using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tracks.Inventory
{
    #region delegates
    public delegate void ItemContainerModified(ItemContainer oldValue, ItemContainer newValue);
    #endregion

    public interface IItemContainer
    {
        public Item Item { get; }
        public int Count { get; }

        public event ItemContainerModified OnModified;

        public bool IsFull();
        public int RemainingSpace();
        public bool IsEmpty();
        public bool IsNotEmpty();
        public bool HasItem(Item item);
        public int FillAndReturnRemaining(Item item, int count, bool modifyContainer = true);
        public int TakeAndReturnRemaining(int count, bool modifyContainer = true);
        public bool CanFill(Item item, int count);
    }

    [System.Serializable]
    public class ItemContainer : IItemContainer
    {

        #region private
        private Item _item;
        private int _count;
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
        #endregion

        #region events
        public event ItemContainerModified OnModified;
        #endregion

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
                throw new ArgumentException("Must be more than 0");
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

            var origional = this;

            var diff = _item.stackSize - (count + count);

            if (diff < 0)
            {
                if (modifyContainer)
                {
                    _count = _item.stackSize;
                    OnModified?.Invoke(origional, this);
                }
                return -diff;
            }

            if (modifyContainer)
            {
                _count += count;
                OnModified?.Invoke(origional, this);
            }

            return 0;
        }

        public int TakeAndReturnRemaining(int count, bool modifyContainer = true)
        {
            if (IsEmpty())
            {
                throw new Exception("Trying to take items from an empty container");
            }

            var origional = this;

            var diff = _count - count;

            if (diff < 0)
            {
                if (modifyContainer)
                {
                    _count = 0;
                    _item = null;
                    OnModified?.Invoke(origional, this);
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

                OnModified?.Invoke(origional, this);
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
    }

    public delegate void PlayerInteraction(PlayerManager player);
    public delegate void InventoryModified();

    public interface IInventory
    {
        /// <summary>
        /// IS this inventory currently being accessed
        /// </summary>
        public bool IsOpen { get; }

        /// <summary>
        /// The player who is currently accessing this, returns null if no-one is
        /// </summary>
        public PlayerManager AccessedBy { get; }

        /// <summary>
        /// List of containers which hold items
        /// </summary>
        public List<IItemContainer> Containers { get; }

        /// <summary>
        /// How many containers this has
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// Event fired when some player entity accessed this inventory
        /// </summary>
        public event PlayerInteraction OnOpened;

        /// <summary>
        /// Event fired when some player entity stops accessing this inventory
        /// </summary>
        public event PlayerInteraction OnClosed;

        /// <summary>
        /// Event fired when any containers are modifieds
        /// </summary>
        public event InventoryModified OnModified;

        /// <summary>
        /// Sets this inventory to open by player
        /// </summary>
        /// <param name="player">The player who is accesing this inventory</param>
        public void Open(PlayerManager player);

        /// <summary>
        /// Sets the inventory to closed by player
        /// </summary>
        /// <param name="player">The player who is closing this inventory</param>
        public void Close(PlayerManager player);


        /// <summary>
        /// Checks whether we can get this amount of an item from this inventory
        /// </summary>
        /// <param name="item">The Item to be checked</param>
        /// <param name="count">The ammount to check</param>
        /// <returns>Return true if the inventory can be filled</returns>
        public bool CanFill(Item item, int count);

        /// <summary>
        /// Checks whether we can get this amount of an item from this inventory
        /// </summary>
        /// <param name="item">The Item to be checked</param>
        /// <param name="count">The ammount to check</param>
        /// <returns>Return true if the invenory contains at least that amount of the given item</returns>
        public bool HasItem(Item item, int count);

        /// <summary>
        /// Fills the inventory with as much of a given item it can hold, and returns whatever it couldn't
        /// </summary>
        /// <param name="item">The Item to be add</param>
        /// <param name="count">The ammount desired</param>
        /// <returns>Returns the count of what couldn't be added, will be 0 if everything was succesfully added</returns>
        public int FillAndReturnRemaining(Item item, int count);

        /// <summary>
        /// Takes from the inventory as much as can be taken, and returns what couldn't be taken
        /// </summary>
        /// <param name="item">The Item to be taken</param>
        /// <param name="count">The ammount desired</param>
        /// <returns>Returns the count of what couldn't be taken, will be 0 if the desired amount was all taken from this inventory</returns>
        public int TakeAndReturnRemaining(Item item, int count);

        /// <summary>
        /// Get the container at a given index
        /// </summary>
        /// <param name="index">The index to get</param>
        /// <returns>Returns the inventoryies <see cref="ItemContainer"/> at the given index, will be null if not found</returns>
        public IItemContainer GetContainerAtIndex(int index);
    }

    [System.Serializable]
    public class Inventory : IInventory
    {
        #region private
        private PlayerManager _accessedBy;

        [SerializeField]
        private List<IItemContainer> _containers = new List<IItemContainer>();
        #endregion

        #region getters
        public bool IsOpen
        {
            get
            {
                return _accessedBy != null;
            }
        }

        public PlayerManager AccessedBy
        {
            get
            {
                return _accessedBy;
            }
        }

        public List<IItemContainer> Containers
        {
            get
            {
                return _containers;
            }
        }

        public int Size
        {
            get
            {
                return _containers.Count;
            }
        }
        #endregion

        #region events
        public event PlayerInteraction OnOpened;
        public event PlayerInteraction OnClosed;
        public event InventoryModified OnModified;
        #endregion

        public bool CanFill(Item item, int count)
        {
            var remainder = count;

            var containers = _containers.Where(x => x.HasItem(item) || x.IsEmpty()).ToList();

            if (containers.Count == 0)
            {
                return false;
            }

            foreach (var container in containers)
            {
                remainder = container.FillAndReturnRemaining(item, count, false);
            }

            return !(remainder > 0);
        }

        public int FillAndReturnRemaining(Item item, int count)
        {
            var remaining = count;

            _containers.Where(x => x.HasItem(item)).ToList().ForEach(x =>
            {
                if (remaining > 0)
                {
                    remaining = x.FillAndReturnRemaining(item, remaining);
                }
            });

            if (remaining > 0)
            {
                _containers.Where(x => x.IsEmpty()).ToList().ForEach(x =>
                {
                    if (remaining > 0)
                    {
                        remaining = x.FillAndReturnRemaining(item, remaining);
                    }
                });
            }

            OnModified.Invoke();
            return remaining;
        }

        public int TakeAndReturnRemaining(Item item, int count)
        {
            var remaining = count;

            Containers.Where(x => x.HasItem(item)).Reverse().ToList().ForEach(x =>
            {
                if (remaining > 0)
                {
                    remaining = x.TakeAndReturnRemaining(remaining);
                }
            });

            OnModified.Invoke();
            return remaining;
        }

        public bool HasItem(Item item, int count)
        {
            var remainder = count;

            var containers = _containers.Where(x => x.HasItem(item)).ToList();

            if (containers.Count == 0)
            {
                return false;
            }

            foreach (var container in containers)
            {
                remainder = container.TakeAndReturnRemaining(count, false);
            }

            return !(remainder > 0);
        }

        public void Open(PlayerManager player)
        {
            if (IsOpen)
            {
                return;
            }

            _accessedBy = player;
            OnOpened.Invoke(player);
        }

        public void Close(PlayerManager player)
        {
            if (!IsOpen)
            {
                Debug.LogWarning("This inventory was closed, but it was not open in the first place");
                return;
            }

            _accessedBy = null;
            OnClosed.Invoke(player);
        }

        public IItemContainer GetContainerAtIndex(int index)
        {
            if (_containers == null || _containers.Count < 1)
            {
                return null;
            }

            if (index + 1 >= _containers.Capacity || index < 0)
            {
                Debug.LogWarning("Trying to acces container at non existing index");
                return null;
            }

            return Containers[index];
        }
    }

    public class PlayerInventory : Inventory
    {
        private PlayerManager _player;
        public IInventory _accessing;

        public IInventory Accessing
        {
            get
            {
                return _accessing;
            }
        }

        public bool IsAccessing
        {
            get
            {
                return _accessing != null;
            }
        }

        public void AccessInventory(IInventory target)
        {
            if (IsAccessing)
            {
                return;
            }

            if (target.IsOpen)
            {
                return;
            }

            _accessing = target;
            target.Open(_player);
        }

        public void StopAccessingInventory(IInventory target)
        {
            if (!IsAccessing)
            {
                return;
            }

            if (Accessing != target)
            {
                return;
            }

            if (!target.IsOpen)
            {
                return;
            }

            _accessing = null;
            target.Close(_player);
        }
    }
}



/*
 * 
 * FNZ
 */
