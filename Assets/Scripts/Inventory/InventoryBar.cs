using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Linq;

public class InventoryBar : MonoBehaviour
{
    private IInventoryController InventoryController { get; set; }

    [HideInInspector]
    public PlayerManager playerMananger;

    public List<ItemContainerUI> BarContainers = new List<ItemContainerUI>();

    public int activeIndex;

    public Image selector;

    public ItemContainer ActiveItem
    {
        get
        {
            return BarContainers[activeIndex].Slot;
        }
    }

    public void LinkInventory(IInventoryController targetInventory)
    {
        if (InventoryController != null)
        {
            throw new System.Exception("Trying to link to another inventory when already linked");
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

        for (int i=0; i<BarContainers.Count; i++)
        {
            BarContainers[i].LinkToInventory(InventoryController, i);
        }
    }

    public void RenderSelector()
    {
        var targetContainer = BarContainers[activeIndex];

        selector.transform.parent = targetContainer.transform;
        selector.rectTransform.localPosition = new Vector3(0, 0, 0);
    }

    public void SetActiveIndex(int index)
    {
        Debug.Log(index);
        if (index > (BarContainers.Count - 1))
        {
            activeIndex = 0;
        }
        else if(index < 0)
        {
            activeIndex = BarContainers.Count - 1;
        }
        else
        {
            activeIndex = index;
        }

        RenderSelector();
    }

    public void Update()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            SetActiveIndex(activeIndex+1);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            SetActiveIndex(activeIndex-1);
        }
    }
}
