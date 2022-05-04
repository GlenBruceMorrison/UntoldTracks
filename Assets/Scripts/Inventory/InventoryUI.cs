using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class InventoryUI : MonoBehaviour
{
    private Inventory _inventory;
    public ContainerUI containerPrefab;
    public GameObject containerWindow;
    public List<ContainerUI> containers = new List<ContainerUI>();

    public UnityAction onInventoryClosed;

    public Button onClose;

    private void Start()
    {
        onClose.onClick.AddListener(HandleInventoryClosed);
    }

    public void HandleInventoryClosed()
    {
        this.gameObject.SetActive(false);
        onInventoryClosed?.Invoke();
    }

    public void LinkInventory(Inventory targetInventory)
    {
        if (_inventory != null)
        {
            throw new System.Exception("Trying to link to another inventory when already linked");
        }

        _inventory = targetInventory;
        _inventory.onModified.AddListener(Render);
        Render();
    }

    public void Render()
    {
        if (_inventory == null)
        {
            throw new System.Exception("Trying to render an inventory that has not been linked yet");
        }

        foreach(Transform child in containerWindow.transform)
        {
            Destroy(child.gameObject);
        }

        containers = new List<ContainerUI>();

        for (int i=0; i< _inventory.containers.Count; i++)
        {
            var newContainer = Instantiate(containerPrefab, transform.position, Quaternion.identity);

            newContainer.LinkToInventory(_inventory, i);
            newContainer.transform.parent = containerWindow.transform;
            containers.Add(newContainer);
        }
    }
}
