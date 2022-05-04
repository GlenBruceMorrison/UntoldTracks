using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class InventoryUI : MonoBehaviour
{
    private IInventoryController InventoryController { get; set; }
    public ContainerUI containerPrefab;
    public GameObject containerWindow;
    public List<ContainerUI> containers = new List<ContainerUI>();

    public UnityAction onInventoryClosed;

    public Button onClose;
    public TMP_Text inventoryName;

    private void Start()
    {
        onClose.onClick.AddListener(HandleInventoryClosed);
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

        inventoryName.text = targetInventory.Inventory.name;

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

        foreach(Transform child in containerWindow.transform)
        {
            Destroy(child.gameObject);
        }

        containers = new List<ContainerUI>();

        for (int i=0; i< InventoryController.Inventory.containers.Count; i++)
        {
            var newContainer = Instantiate(containerPrefab, transform.position, Quaternion.identity);

            newContainer.LinkToInventory(InventoryController, i);
            newContainer.transform.parent = containerWindow.transform;
            containers.Add(newContainer);
        }
    }
}
