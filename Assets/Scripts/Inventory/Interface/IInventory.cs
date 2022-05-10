using System.Collections.Generic;
using UntoldTracks.Player;

namespace UntoldTracks.Inventory
{
    public delegate void PlayerInteraction(PlayerManager player);
    public delegate void ContainerAdded(ItemContainer container);
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
        //public PlayerManager AccessedBy { get; }

        /// <summary>
        /// List of containers which hold items
        /// </summary>
        public List<IItemContainer> Containers { get; }

        /// <summary>
        /// How many containers this has
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// 
        /// </summary>
        public event ContainerAdded OnContainerAdded;

        /// <summary>
        /// 
        /// </summary>
        public event ContainerAdded OnContainerRemoved;

        /// <summary>
        /// Event fired when some player entity accessed this inventory
        /// </summary>
        //public event PlayerInteraction OnOpened;

        /// <summary>
        /// Event fired when some player entity stops accessing this inventory
        /// </summary>
        //public event PlayerInteraction OnClosed;

        /// <summary>
        /// Event fired when any containers are modifieds
        /// </summary>
        //public event InventoryModified OnModified;

        /// <summary>
        /// Sets this inventory to open by player
        /// </summary>
        /// <param name="player">The player who is accesing this inventory</param>
        //public void Open(PlayerManager player);

        /// <summary>
        /// Sets the inventory to closed by player
        /// </summary>
        /// <param name="player">The player who is closing this inventory</param>
        //public void Close(PlayerManager player);

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
        /// <returns>Returns the inventoryies <see cref="IItemContainer"/> at the given index, will be null if not found</returns>
        public IItemContainer GetContainerAtIndex(int index);

        /// <summary>
        /// Adds a new itemContainer to this inventories container list
        /// </summary>
        /// <returns>Returns the <see cref="IItemContainer"/> added</returns>
        public IItemContainer AppendContainer();

        /// <summary>
        /// Resets the inventory and updates size
        /// </summary>
        /// <param name="size">Target size</param>
        public void SetSize(int size);
    }
}
