using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericInventoryController : MonoBehaviour, IInteractable, IInventoryController
{
    public InventoryUI inventoryUIPrefab;

    [SerializeField]
    private Inventory _inventory;

    [HideInInspector]
    private InventoryUI _inventoryUI;

    // can only ever be accessing some inventory once at a time
    private PlayerInventoryController _accessedBy;

    public UnityEvent<ItemContainer> onItemDropped;

    public Inventory Inventory => _inventory;

    public InventoryUI UI => _inventoryUI;

    public PlayerInventoryController AccessedBy => _accessedBy;

    public IInventoryController Accessing => throw new System.Exception("This cannot access any other iventory as it is not controlled by the player");



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
        AccessedBy.inventoryUI.onInventoryClosed += Hide;
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
            AccessedBy.inventoryUI.onInventoryClosed -= Hide;
        }

        _accessedBy = null;
    }

    public void Show()
    {
        UI.gameObject.SetActive(true);
    }
}
