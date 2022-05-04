using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryController : MonoBehaviour, IInventoryController
{
    public PlayerManager playerManager;

    public Inventory inventory;

    [HideInInspector]
    public InventoryUI inventoryUI;

    public InventoryUI inventoryUIPrefab;

    public IInventoryController accessing;

    public Inventory Inventory { get => inventory; }
    public InventoryUI UI { get => inventoryUI; }
    public PlayerInventoryController AccessedBy { get => null; }
    public IInventoryController Accessing { get => accessing; }

    private void Awake()
    {
        var canvas = GameObject.Find("Main");
        var invent = Instantiate(inventoryUIPrefab, canvas.transform.position, Quaternion.identity);

        invent.transform.parent = canvas.transform;

        inventoryUI = invent.GetComponent<InventoryUI>();
        inventoryUI.LinkInventory(this);

        inventoryUI.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        inventoryUI.onInventoryClosed += Hide;
    }

    private void OnDisable()
    {
        inventoryUI.onInventoryClosed -= Hide;
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

    internal void AccessInventory(IInventoryController inventoryController)
    {
        accessing = inventoryController;
        //accessing.Access(playerManager);
        playerManager.inventoryController.Show();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (inventoryUI.gameObject.activeSelf)
            {
                Hide();
                if (accessing != null)
                {
                    accessing.Hide();
                    accessing = null;
                }
            }
            else
            {
                Show();
            }
        }
    }

    public void Hide()
    {
        playerManager.FirstPersonController.Movement.GainControl();
        playerManager.FirstPersonController.Look.LockPointer();
        inventoryUI.gameObject.SetActive(false);
    }

    public void Show()
    {
        playerManager.FirstPersonController.Movement.LoseControl();
        playerManager.FirstPersonController.Look.UnlockPointer();
        inventoryUI.gameObject.SetActive(true);
    }
}
