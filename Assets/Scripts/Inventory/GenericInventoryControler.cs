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


    public UnityEvent<ItemContainer> onItemDropped;

    private void Awake()
    {
        var canvas = GameObject.Find("Main");
        var invent = Instantiate(inventoryUIPrefab, canvas.transform.position, Quaternion.identity);

        invent.transform.parent = canvas.transform;

        inventoryUI = invent.GetComponent<InventoryUI>();
        inventoryUI.LinkInventory(inventory);
    }

    public void ShowInventory()
    {
        inventoryUI.gameObject.SetActive(true);
    }

    public void HideInventory()
    {
        inventoryUI.gameObject.SetActive(false);

    }

    private void Update()
    {
        if (Input.GetKeyDown("w"))
        {
            HideInventory();
        }
        if (Input.GetKeyDown("r"))
        {
            ShowInventory();
        }
    }
}
