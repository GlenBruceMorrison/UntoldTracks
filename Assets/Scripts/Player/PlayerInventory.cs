using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Inventory inventory;

    [HideInInspector]
    public InventoryUI inventoryUI;

    public InventoryUI inventoryUIPrefab;

    private void Start()
    {
        var canvas = GameObject.Find("Canvas");
        var invent = Instantiate(inventoryUIPrefab, canvas.transform.position, Quaternion.identity);

        invent.transform.parent = canvas.transform;

        inventoryUI = invent.GetComponent<InventoryUI>();
        inventoryUI.LinkInventory(inventory);
    }

    private void Update()
    {
        if (Input.GetKeyDown("r"))
        {
            inventoryUI.enabled = !inventoryUI.enabled;
        }
    }
}
