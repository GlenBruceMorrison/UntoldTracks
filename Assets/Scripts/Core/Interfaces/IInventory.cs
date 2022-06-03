using System.Collections.Generic;
using UntoldTracks.Player;
using UntoldTracks.Models;
using UntoldTracks.Managers;

namespace UntoldTracks.InventorySystem
{
    public delegate void PlayerInteraction(PlayerManager player);
    public delegate void ContainerModified(ItemContainer container);
    public delegate void InventoryModified();

    public interface IInventory
    {
        /// <summary>
        /// IS this inventory currently being accessed
        /// </summary>
        public bool IsOpen { get; }

        /// <summary>
        /// List of containers which hold items
        /// </summary>
        public List<ItemContainer> Containers { get; }

        /// <summary>
        /// How many containers this has
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// 
        /// </summary>
        public event ContainerModified OnContainerAdded;

        /// <summary>
        /// 
        /// </summary>
        public event ContainerModified OnContainerRemoved;

        /// <summary>
        /// 
        /// </summary>
        public event ContainerModified OnContainerModified;

        /// <summary>
        /// Checks whether we can get this amount of an item from this inventory
        /// </summary>
        /// <param name="item">The Item to be checked</param>
        /// <param name="count">The ammount to check</param>
        /// <returns>Return true if the inventory can be filled</returns>
        public bool CanGive(ItemModel item, int count);
        ItemQueryResult Give(ItemContainer container);

        /// <summary>
        /// Checks whether we can get this amount of an item from this inventory
        /// </summary>
        /// <param name="item">The Item to be checked</param>
        /// <param name="count">The ammount to check</param>
        /// <returns>Return true if the invenory contains at least that amount of the given item</returns>
        public bool CanTake(ItemModel item, int count);

        public ItemQueryResult Take(ItemQuery query);

        /// <summary>
        /// Get the container at a given index
        /// </summary>
        /// <param name="index">The index to get</param>
        /// <returns>Returns the inventoryies <see cref="ItemContainer"/> at the given index, will be null if not found</returns>
        public ItemContainer GetContainerAtIndex(int index);

        /// <summary>
        /// Adds a new itemContainer to this inventories container list
        /// </summary>
        /// <returns>Returns the <see cref="ItemContainer"/> added</returns>
        public ItemContainer AppendContainer();

        /// <summary>
        /// Resets the inventory and updates size
        /// </summary>
        /// <param name="size">Target size</param>
        public void SetSize(int size);

        int GetItemCount(ItemModel item);
    }
}
