using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class PlayerInventoryUI : MonoBehaviour
{
    private List<ItemContainerUI> _containers = new List<ItemContainerUI>();

    public List<ItemContainerUI> InventoryContainers
    {
        get
        {
            return _containers;
        }
    }

    public List<ItemContainerUI> BulkInventoryContainers
    {
        get
        {
            return _containers.Skip(10).ToList();
        }
    }

    public List<ItemContainerUI> InventoryBarContainers
    {
        get
        {
            return _containers.Take(10).ToList();
        }
    }

    private IInventoryController InventoryController
    {
        get;
        set;
    }

    public UnityAction onInventoryClosed;

    public Button onClose;
    public TMP_Text inventoryName;
    public ItemContainerUI containerPrefab;
    public GameObject containerWindow;

    public void LinkInventory(IInventoryController targetInventory)
    {
        if (InventoryController != null)
        {
            throw new System.Exception("Trying to link to another inventory when already linked");
        }

        inventoryName.text = targetInventory.Inventory.name;

        InventoryController = targetInventory;
        InventoryController.Inventory.onModified.AddListener(Render);
        Render();
    }

    public void Render()
    {
        for (int i = 0; i < InventoryController.Inventory.containers.Count; i++)
        {
            _containers[i].LinkToInventory(InventoryController, i);
        }
    }
}
