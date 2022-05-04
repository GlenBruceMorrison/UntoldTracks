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

    public GenericInventoryController accessing;

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

    internal void AccessInventory(GenericInventoryController inventoryController)
    {
        accessing = inventoryController;
        //accessing.Access(playerManager);
        playerManager.inventoryController.ShowInventory();
    }

    public void ShowInventory()
    {
        playerManager.FirstPersonController.Movement.LoseControl();
        playerManager.FirstPersonController.Look.UnlockPointer();
        inventoryUI.gameObject.SetActive(true);
    }

    public void HideInventory()
    {
        playerManager.FirstPersonController.Movement.GainControl();
        playerManager.FirstPersonController.Look.LockPointer();
        inventoryUI.gameObject.SetActive(false);

        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (inventoryUI.gameObject.activeSelf)
            {
                HideInventory();
                if (accessing != null)
                {
                    accessing.HideInventory();
                    accessing = null;
                }
            }
            else
            {
                ShowInventory();
            }
        }
    }
}
