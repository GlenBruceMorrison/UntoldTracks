using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    public PlayerManager playerManager;

    public Inventory inventory;

    [HideInInspector]
    public InventoryUI inventoryUI;

    public InventoryUI inventoryUIPrefab;

    private void Awake()
    {
        var canvas = GameObject.Find("Main");
        var invent = Instantiate(inventoryUIPrefab, canvas.transform.position, Quaternion.identity);

        invent.transform.parent = canvas.transform;

        inventoryUI = invent.GetComponent<InventoryUI>();
        inventoryUI.LinkInventory(inventory);

        inventoryUI.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        inventoryUI.onInventoryClosed += HideInventory;
    }

    public void HandleItemPickupFromWorld(ItemContainerWorldObject containerWorldObject)
    {
        var remaining = inventory.AddAndReturnRemaining(containerWorldObject.GetContainerValue().item, containerWorldObject.GetContainerValue().count);

        if (remaining > 0)
        {
            GetComponent<Backplate>().HandleItemDropped(new ItemContainer()
            {
                item = containerWorldObject.Container.item,
                count = remaining
            });
        }

        GameObject.Destroy(containerWorldObject.gameObject);
    }

    public void ShowIntenvtory()
    {
        playerManager.controlController.LoseControl();
        playerManager.controlController.FreeViewOn();
        inventoryUI.gameObject.SetActive(true);
    }

    public void HideInventory()
    {
        playerManager.controlController.GainControl();
        playerManager.controlController.FreeViewOff();
        inventoryUI.gameObject.SetActive(false);
        Debug.Log("closed");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (inventoryUI.gameObject.activeSelf)
            {
                HideInventory();
            }
            else
            {
                ShowIntenvtory();
            }
        }
    }
}
