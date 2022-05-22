using System;
using System.Linq;
using System.Collections.Generic;
using UntoldTracks.Player;

namespace UntoldTracks.Inventory
{
    public class Inventory : IInventory
    {
        #region private
        private PlayerManager _accessedBy;
        private List<ItemContainer> _containers = new List<ItemContainer>();
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

        public List<ItemContainer> Containers
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
        public event InventoryModified OnModified;
        public event ContainerModified OnContainerAdded; 
        public event ContainerModified OnContainerRemoved;
        public event ContainerModified OnContainerModified;
        #endregion

        public Inventory(int size)
        {
            SetSize(size);
        }

        public ItemContainer AppendContainer()
        {
            var container = new ItemContainer(this, _containers.Count);
            _containers.Add(container);
            container.OnModified += HandleContainerModified;
            return container;
        }

        private void HandleContainerModified(ItemContainer newValue)
        {
            OnContainerModified?.Invoke(newValue);
        }

        public ItemQueryResult Take(ItemQuery query)
        {
            var itemQueryResult = new ItemQueryResult(query.item, 0);

            // if item is degradable we need to return item properties
            if (query.item.degradable)
            {
                ItemContainer container = null;

                // find container
                if (query.preferredIndex >= 0)
                {
                    // if we actually have am index there
                    if (Size - 1 > query.preferredIndex)
                    {
                        // get container at index
                        container = _containers[query.preferredIndex];
                    }
                }
                else
                {
                    // find first container that has this item
                    container = Containers.Where(x => x.HasItem(query.item)).First();
                }

                // transfer container 
                if (container != null && container.Count > 0 && container.HasItem(query.item))
                {
                    itemQueryResult.amountAdded = 1;
                    itemQueryResult.durability = container.CurrentDurability;

                    container.Empty();
                    OnContainerModified?.Invoke(container);
                }
            }
            else
            {
                var remaining = TakeAndReturnRemaining(query.item, query.count, query.preferredIndex);

                itemQueryResult.amountAdded = query.count - remaining;
            }

            return itemQueryResult;
        }

        public ItemQueryResult Give(ItemContainer container)
        {
            var itemQueryResult = new ItemQueryResult(container.Item, 0);

            // is is not stackable, then just give to the first empty container
            if (!container.Item.stackable)
            {
                var emptyContainer = _containers.Where(x => x.IsEmpty()).First();
                var result =  emptyContainer.Give(container);
                OnModified?.Invoke();
                return result;
            }
            
            // keep a running track of what's left to give
            var amountLeftToAdd = container.Count;

            // start with items that have this item
            var containersWithItem = _containers.Where(x => x.HasItem(container.Item)).ToList();
            foreach (var containerWithItem in containersWithItem)
            {
                var result = containerWithItem.Give(new ItemContainer(container.Item, amountLeftToAdd));
                itemQueryResult.amountAdded += result.amountAdded;
                amountLeftToAdd -= result.amountAdded;

                if (amountLeftToAdd <= 0)
                {
                    break;
                }
            }

            // still have more to add, so add them to the first empty containers
            if (amountLeftToAdd > 0)
            {
                var emptyContainers = _containers.Where(x => x.IsEmpty()).ToList();
                foreach (var emptyContainer in emptyContainers)
                {
                    var result = emptyContainer.Give(new ItemContainer(container.Item, amountLeftToAdd));
                    itemQueryResult.amountAdded += result.amountAdded;
                    amountLeftToAdd -= result.amountAdded;

                    if (amountLeftToAdd <= 0)
                    {
                        break;
                    }
                }
            }

            OnModified?.Invoke();
            return itemQueryResult;
        }

        public bool CanTake(Item item, int count)
        {
            var containers = _containers.Where(x => x.HasItem(item)).ToList();

            if (containers.Count == 0)
            {
                return false;
            }

            var remainder = count;

            foreach (var container in containers)
            {
                remainder = container.TakeAndReturnRemaining(count, false);
            }

            return !(remainder > 0);
        }

        public bool CanGive(Item item, int count)
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

        private List<ItemContainer> GetNonEmptyContainer()
        {
            return _containers.Where(x => !x.IsEmpty()).ToList();
        }

        public int GetItemCount(Item item)
        {
            return GetNonEmptyContainer().Where(x => x.Item == item).Sum(x => x.Count);
        }

        public ItemContainer GetContainerAtIndex(int index)
        {
            if (_containers == null || _containers.Count < 1)
            {
                return null;
            }

            if (index + 1 >= _containers.Capacity || index < 0)
            {
                //Debug.LogWarning("Trying to access container at non existing index");
                return null;
            }

            return Containers[index];
        }

        public void SetSize(int size)
        {
            _containers = new List<ItemContainer>();

            for (var i=0;i<size; i++)
            {
                AppendContainer();
            }
        }

        /// <summary>
        /// Takes from the inventory as much as can be taken, and returns what couldn't be taken
        /// </summary>
        /// <param name="item">The Item to be taken</param>
        /// <param name="count">The ammount desired</param>
        /// <param name="preferredIndex">The inventory index you would preffered to priotise first</param>
        /// <returns>Returns the count of what couldn't be taken, will be 0 if the desired amount was all taken from this inventory</returns>
        private int TakeAndReturnRemaining(Item item, int count, int preferredIndex = -1)
        {
            var remaining = count;

            // prioritise preferred index first
            if (preferredIndex >= 0)
            {
                var preferredContainer = GetContainerAtIndex(preferredIndex);
                remaining = preferredContainer.TakeAndReturnRemaining(count);
                if (remaining <= 0)
                {
                    OnContainerRemoved?.Invoke(new ItemContainer(item, count - remaining));
                    return 0;
                }
            }

            var toCheck = Containers.Where(x => x.HasItem(item)).Reverse().ToList();

            toCheck.ForEach(x =>
            {
                if (remaining > 0)
                {
                    remaining = x.TakeAndReturnRemaining(remaining);
                }
            });

            OnModified?.Invoke();

            if (remaining != count)
            {
                OnContainerRemoved?.Invoke(new ItemContainer(item, count - remaining));
            }

            return remaining;
        }

        /// <summary>
        /// Fills the inventory with as much of a given item it can hold, and returns whatever it couldn't
        /// </summary>
        /// <param name="item">The Item to be add</param>
        /// <param name="count">The ammount desired</param>
        /// <returns>Returns the count of what couldn't be added, will be 0 if everything was succesfully added</returns>
        private int FillAndReturnRemaining(Item item, int count)
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

            OnModified?.Invoke();

            if (remaining != count)
            {
                OnContainerAdded?.Invoke(new ItemContainer(item, count - remaining));
            }

            return remaining;
        }
    }
}