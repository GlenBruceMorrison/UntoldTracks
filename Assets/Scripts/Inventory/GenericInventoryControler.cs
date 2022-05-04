using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericInventoryControler : MonoBehaviour
{
    public Inventory inventory;
    [HideInInspector]
    public InventoryUI inventoryUI;
    public InventoryUI inventoryUIPrefab;

    // can only ever be accessing some inventory once at a time
    public PlayerInventoryController accessedBy;

    public UnityEvent<ItemContainer> onItemDropped;

    private void Awake()
    {
        var canvas = GameObject.Find("Main");
        var invent = Instantiate(inventoryUIPrefab, canvas.transform.position, Quaternion.identity);

        invent.transform.parent = canvas.transform;

        inventoryUI = invent.GetComponent<InventoryUI>();
        inventoryUI.LinkInventory(inventory);

        HideInventory();
    }

    public void Access(PlayerInventoryController playerInventory)
    {
        if (accessedBy != null)
        {
            // being accessed by something else...
            return;
        }

        ShowInventory();
        accessedBy = playerInventory;
        accessedBy.inventoryUI.onInventoryClosed += () =>
        {
            HideInventory();
        };
    }

    public void ShowInventory()
    {
        inventoryUI.gameObject.SetActive(true);
    }

    public void HideInventory()
    {
        inventoryUI.gameObject.SetActive(false);
        accessedBy = null;
    }
}
