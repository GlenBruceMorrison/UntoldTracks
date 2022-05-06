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

    [SerializeField]
    private int _activeIndex;

    [SerializeField]
    private Image _selector;

    public ItemContainer ActiveItem
    {
        get
        {
            if (BarContainers[_activeIndex] == null)
            {
                return new ItemContainer()
                {
                    item = null,
                    count = 0
                };
            }

            return BarContainers[_activeIndex].Slot;
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

    private void RenderSelector()
    {
        var targetContainer = BarContainers[_activeIndex];

        _selector.transform.parent = targetContainer.transform;
        _selector.rectTransform.localPosition = new Vector3(0, 0, 0);
    }

    public void SetActiveIndex(int index)
    {
        if (index > (BarContainers.Count - 1))
        {
            _activeIndex = 0;
        }
        else if(index < 0)
        {
            _activeIndex = BarContainers.Count - 1;
        }
        else
        {
            _activeIndex = index;
        }

        Debug.Log(playerMananger.gameObject);
        ActiveItemChangeEvent.BroadcastEvent(new ActiveItemChangeEvent()
        {
            player = playerMananger,
            item = ActiveItem
        });

        RenderSelector();
    }

    public void Update()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            SetActiveIndex(_activeIndex + 1);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            SetActiveIndex(_activeIndex - 1);
        }
    }
}
