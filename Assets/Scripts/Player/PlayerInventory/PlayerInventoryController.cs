using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventoryController : MonoBehaviour, IInventoryController
{
    public static readonly int INVENTORY_BAR_SIZE = 8;

    [HideInInspector]
    public PlayerManager playerManager;

    [SerializeField]
    private Inventory _inventory;

    private InventoryUI _inventoryUI;

    private IInventoryController _accessing;

    public InventoryUI inventoryUIPrefab;

    [SerializeField]
    private InventoryBar _inventoryBar;

    public UnityEvent onActiveItemChanged;

    public InventoryBar InventoryBar
    {
        get
        {
            return _inventoryBar;
        }
    }

    public Inventory Inventory
    {
        get
        {
            return _inventory;
        }
    }

    public InventoryUI UI
    {
        get
        {
            return _inventoryUI;
        }
    }

    public PlayerInventoryController AccessedBy
    {
        get
        {
            // player cannot be accessed by any other
            return null;
        }
    }

    public IInventoryController Accessing
    {
        get
        {
            return _accessing;
        }
    } 

    private void Awake()
    {
        var canvas = GameObject.Find("Main");
        var invent = Instantiate(inventoryUIPrefab, canvas.transform.position, Quaternion.identity);

        invent.inventoryStartFromIndex = INVENTORY_BAR_SIZE;

        invent.transform.parent = canvas.transform;

        _inventoryUI = invent.GetComponent<InventoryUI>();
        _inventoryUI.LinkInventory(this);
        _inventoryBar.LinkInventory(this);

        _inventoryUI.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _inventoryUI.onInventoryClosed += Hide;
    }

    private void OnDisable()
    {
        _inventoryUI.onInventoryClosed -= Hide;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (_inventoryUI.gameObject.activeSelf)
            {
                Hide();
                if (_accessing != null)
                {
                    _accessing.Hide();
                    _accessing = null;
                }
            }
            else
            {
                Show();
            }
        }
    }

    public void AccessInventory(IInventoryController inventoryController)
    {
        _accessing = inventoryController;
        playerManager.inventoryController.Show();
    }

    public void HandleItemPickupFromWorld(ItemContainerWorldObject containerWorldObject)
    {
        var remaining = _inventory.AddAndReturnRemaining(containerWorldObject.GetContainerValue().item, containerWorldObject.GetContainerValue().count);

        if (remaining > 0)
        {
            GetComponent<Backplate>().HandleItemDropped(new ItemContainer()
            {
                item = containerWorldObject.Container.item,
                count = remaining
            });
        }

        Destroy(containerWorldObject.gameObject);
    }

    public void Hide()
    {
        playerManager.FirstPersonController.Movement.GainControl();
        playerManager.FirstPersonController.Look.LockPointer();
        _inventoryUI.gameObject.SetActive(false);
    }

    public void Show()
    {
        playerManager.FirstPersonController.Movement.LoseControl();
        playerManager.FirstPersonController.Look.UnlockPointer();
        _inventoryUI.gameObject.SetActive(true);
    }
}
