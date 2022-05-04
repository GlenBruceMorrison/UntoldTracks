using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryController : MonoBehaviour, IInteractable, IInventoryController
{
    public InventoryUI inventoryUIPrefab;

    [SerializeField]
    private Inventory _inventory;

    private InventoryUI _inventoryUI;

    private PlayerInventoryController _accessedBy;

    // Data object
    public Inventory Inventory
    {
        get
        {
            return _inventory;
        }
    }

    // The user interface representation of this inventory
    public InventoryUI UI
    {
        get
        {
            return _inventoryUI;
        }
    }

    // What other inventory is accessing this one. if it is open, null if not
    public PlayerInventoryController AccessedBy
    {
        get
        {
            return _accessedBy;
        }
    }

    // What inventory this is accessing
    public IInventoryController Accessing
    {
        get
        {
            throw new System.Exception("This cannot access any other iventory as it is not controlled by the player");
        }
    }

    private void Awake()
    {
        var canvas = GameObject.Find("Main");
        var invent = Instantiate(inventoryUIPrefab, canvas.transform.position, Quaternion.identity);

        invent.transform.parent = canvas.transform;

        _inventoryUI = invent.GetComponent<InventoryUI>();
        UI.LinkInventory(this);

        Hide();
    }

    public void Access(PlayerManager player)
    {
        if (AccessedBy != null)
        {
            // being accessed by something else...
            return;
        }
        player.inventoryController.AccessInventory(this);
        Show();
        _accessedBy = player.inventoryController;
        AccessedBy.UI.onInventoryClosed += Hide;
    }

    public void HandleInteraction(PlayerManager player)
    {
        Access(player);
    }

    public void Hide()
    {
        UI.gameObject.SetActive(false);

        if (AccessedBy != null)
        {
            AccessedBy.UI.onInventoryClosed -= Hide;
        }

        _accessedBy = null;
    }

    public void Show()
    {
        UI.gameObject.SetActive(true);
    }
}
