using System;
using System.Linq;

namespace Tracks.Inventory
{

    public class PlayerInventoryUI : InventoryUI
    {
        public int inventoryBarSize = 9;
        public PlayerInventory Inventory { get; private set; }
        public InventoryUI _linkedInventory;

        public void Open(IInventory inventory=null)
        {
            if (inventory != null)
            {
                _linkedInventory.gameObject.SetActive(true);
                _linkedInventory.LinkToInventory(inventory);
            }
        }

        public void Close()
        {
            _linkedInventory.gameObject.SetActive(false);
        }

        internal void Initiate(PlayerInventory playerInventory)
        {
            Inventory = playerInventory;
        }
    }
}