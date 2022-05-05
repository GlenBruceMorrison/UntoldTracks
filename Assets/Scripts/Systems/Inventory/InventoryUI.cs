using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Linq;

public class InventoryUI : MonoBehaviour
{
    private IInventoryController InventoryController { get; set; }
    public ItemContainerUI containerPrefab;
    public GameObject containerWindow;
    public List<ItemContainerUI> containers = new List<ItemContainerUI>();

    public UnityAction onInventoryClosed;

    public Button onClose;
    public TMP_Text inventoryName;

    private bool _generateInventory = true;

    public int inventoryStartFromIndex = 0;
    public int inventoryEndOnIndex = 0;

    private void Awake()
    {
        if (containerWindow.transform.childCount > 0)
        {
            _generateInventory = true;
        }

        if (!_generateInventory)
        {
            containers = containerWindow.GetComponentsInChildren<ItemContainerUI>().ToList();
        }

        if (onClose != null)
        {
            onClose.onClick.AddListener(HandleInventoryClosed);
        }
    }

    public void HandleInventoryClosed()
    {
        gameObject.SetActive(false);
        onInventoryClosed?.Invoke();
    }

    public void LinkInventory(IInventoryController targetInventory)
    {
        if (InventoryController != null)
        {
            throw new System.Exception("Trying to link to another inventory when already linked");
        }

        if (inventoryName != null)
        {
            inventoryName.text = targetInventory.Inventory.name;
        }

        InventoryController = targetInventory;
        InventoryController.Inventory.onModified.AddListener(Render);
        Render();
    }

    public void Render()
    {
        if (InventoryController == null)
        {
            throw new System.Exception("Trying to render an inventory that has not been linked yet");
        }

        var endIndex = InventoryController.Inventory.containers.Count;

        if (inventoryEndOnIndex > 0)
        {
            endIndex = inventoryEndOnIndex;
        }

        if (_generateInventory)
        {
            foreach (Transform child in containerWindow.transform)
            {
                Destroy(child.gameObject);
            }

            containers = new List<ItemContainerUI>();

            for (int i = inventoryStartFromIndex; i < endIndex; i++)
            {
                var newContainer = Instantiate(containerPrefab, transform.position, Quaternion.identity);

                newContainer.LinkToInventory(InventoryController, i);
                newContainer.transform.parent = containerWindow.transform;
                containers.Add(newContainer);
            }

            return;
        }

        for (int i = inventoryStartFromIndex; i < endIndex; i++)
        {
            containers[i].LinkToInventory(InventoryController, i);
        }
    }
}
