using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Linq;

public class InventoryBar : MonoBehaviour
{
    private IInventoryController InventoryController { get; set; }

    [HideInInspector]
    public PlayerManager playerMananger;

    public List<ItemContainerUI> BarContainers = new List<ItemContainerUI>();

    public int activeIndex;

    public ItemContainer ActiveItem
    {
        get
        {
            return BarContainers[activeIndex].Slot;
        }
    }

    public void LinkInventory(IInventoryController targetInventory)
    {
        if (InventoryController != null)
        {
            throw new System.Exception("Trying to link to another inventory when already linked");
        }

        InventoryController = targetInventory;
        InventoryController.Inventory.onModified.AddListener(Render);
        Render();
    }

    public void Render()
    {
        if (InventoryController == null)
        {
            throw new System.Exception("Trying to render an inventory that has not been linked yet");
        }

        for (int i=0; i<BarContainers.Count; i++)
        {
            BarContainers[i].LinkToInventory(InventoryController, i);
        }
    }
}
