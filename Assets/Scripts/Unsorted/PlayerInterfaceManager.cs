using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInterfaceManager : MonoBehaviour
{
    public Inventory masterInventory;
    public InventoryUI masterInventoryUI;

    public ItemContainer mouseInventory;

    private void Start()
    {
        masterInventoryUI.containers.ForEach(x =>
        {
            x.onDragBegin += (containerUI) =>
            {
                if (mouseInventory != null)
                {
                    return;
                }

                mouseInventory = containerUI.Slot;
                containerUI.Inventory.RemoveAtSlot(containerUI.SlotIndex);
            };
        });
    }
}
