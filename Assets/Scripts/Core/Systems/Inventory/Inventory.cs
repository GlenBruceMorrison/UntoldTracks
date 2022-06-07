using System;
using System.Linq;
using System.Collections.Generic;
using UntoldTracks.Player;
using UntoldTracks.Models;
using UntoldTracks.Managers;
using UntoldTracks.Data;
using SimpleJSON;
using UnityEngine;

namespace UntoldTracks.InventorySystem
{
    public class Inventory : IInventory, ITokenizable
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

        #region constructors
        public Inventory(int size)
        {
            SetSize(size);
        }

        public Inventory(JSONNode node)
        {
            Load(node);
        }
        #endregion

        #region eventHandlers
        private void HandleContainerModified(ItemContainer newValue)
        {
            OnContainerModified?.Invoke(newValue);
        }
        #endregion

        /// <summary>
        /// Add another container to this inventory
        /// </summary>
        /// <returns>Returns the container that was added</returns>
        public ItemContainer AppendContainer()
        {
            var container = new ItemContainer(this, _containers.Count);
            _containers.Add(container);
            container.OnModified += HandleContainerModified;
            return container;
        }

        /// <summary>
        /// Attempt to take items from this inventory.
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

        /// <summary>
        /// Attempt to give this inventory data. 
        /// </summary>
        /// <param name="container">
        /// The item container containing the item and the amount
        /// you want to attempt to add to this inventory
        /// </param>
        /// <returns>
        /// Returns a ItemQueryResult instance, that 
        /// provides information on the result of this query.
        /// </returns>
        public ItemQueryResult Give(ItemContainer container)
        {
            var itemQueryResult = new ItemQueryResult(container.Item, container.Item.durability);

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

        #region Token
        public void Load(JSONNode node)
        {
            _containers.Clear();

            var result = new InventoryData()
            {
                size = node["size"]
            };

            var itemsNode = node["items"];
            for (var i=0; i<result.size; i++)
            {
                var added = false;
                foreach (var item in itemsNode.Children)
                {
                    if (item["inventoryIndex"] == i)
                    {
                        var targetItem = GameManager.Instance.Registry.FindByGUID<ItemModel>(item["itemGUID"]);

                        if (targetItem != null)
                        {
                            _containers.Add(new ItemContainer(this, item["inventoryIndex"], targetItem, item["amount"], item["durability"]));
                            added = true;
                            continue;
                        }
                    }
                }

                if (!added)
                {
                    _containers.Add(new ItemContainer(this, i));
                }
            }
        }

        public JSONObject Save()
        {
            var inventoryJSON = new JSONObject();

            inventoryJSON.Add("size", Size);

            var itemsJSON = new JSONArray();

            foreach (var container in _containers)
            {
                if (container?.Item == null)
                {
                    continue;
                }

                var item = new JSONObject();

                item.Add("itemGUID", container.Item.Guid);
                item.Add("inventoryIndex", container.Index);
                item.Add("amount", container.Count);
                item.Add("durability", container.CurrentDurability);

                itemsJSON.Add(item);
            }

            inventoryJSON.Add("items", itemsJSON);

            return inventoryJSON;
        }
        #endregion

        /// <summary>
        /// Whether this inventory contains the given item and amount.
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
        public bool CanTake(ItemModel item, int count)
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

        /// <summary>
        /// Whether this inventory can take the given item and amount
        /// </summary>
        /// <param name="item">
        /// The target item
        /// </param>
        /// <param name="count">
        /// The amount needed
        /// </param>
        /// <returns>
        /// Returns whether this inventory can take the full amount
        /// </returns>
        public bool CanGive(ItemModel item, int count)
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

        /// <summary>
        /// Checks how much of a given item is present in this inventory
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <returns>Returns an integer representing the amount present</returns>
        public int GetItemCount(ItemModel item)
        {
            return GetNonEmptyContainer().Where(x => x.Item == item).Sum(x => x.Count);
        }

        /// <summary>
        /// Reurns the container present at the given index
        /// </summary>
        /// <param name="index">The index to check</param>
        /// <returns>
        /// Returns the container at the index, will be null if the index does 
        /// not exist
        /// </returns>
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

        /// <summary>
        /// Sets the size of the inventory
        /// </summary>
        /// <param name="size">The size value to target</param>
        public void SetSize(int size)
        {
            _containers = new List<ItemContainer>();

            for (var i=0;i<size; i++)
            {
                AppendContainer();
            }
        }

        private List<ItemContainer> GetNonEmptyContainer()
        {
            return _containers.Where(x => !x.IsEmpty()).ToList();
        }

        /// <summary>
        /// Takes from the inventory as much as can be taken, and returns what couldn't be taken
        /// </summary>
        /// <param name="item">The Item to be taken</param>
        /// <param name="count">The ammount desired</param>
        /// <param name="preferredIndex">The inventory index you would preffered to priotise first</param>
        /// <returns>Returns the count of what couldn't be taken, will be 0 if the desired amount was all taken from this inventory</returns>
        private int TakeAndReturnRemaining(ItemModel item, int count, int preferredIndex = -1)
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
        private int FillAndReturnRemaining(ItemModel item, int count)
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