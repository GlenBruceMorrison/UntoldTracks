using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Linq;

public class ItemContainerUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public TMP_Text countText;
    public Image displayImage;

    public GameObject contents;

    public IInventoryController InventoryController { get; private set; }
    public int SlotIndex { get; private set; }

    public UnityAction<ItemContainerUI> onDragBegin, onDrag, onDrop;

    public ItemContainer Slot => InventoryController.Inventory.containers[SlotIndex];

    public bool beingDragged;
    public Vector2 preDragPosition;


    public void LinkToInventory(IInventoryController targetInventory, int targetSlotIndex)
    {
        InventoryController = targetInventory;
        SlotIndex = targetSlotIndex;
        Render();
    }

    public void SetItem(Item item, int count)
    {
        Slot.item = item;
        Slot.count = count;
        Render();
    }

    public void SetCount(int count)
    {
        if (Slot.item == null)
        {
            throw new System.Exception("Can't add more to this container because it has no item");
        }

        Slot.count = count;
        Render();
    }

    public void Render()
    {
        if (Slot.item == null)
        {
            countText.text = "";
            displayImage.sprite = null;
            displayImage.enabled = false;
            return;
        }

        countText.text = !Slot.item.stackable ? "" : Slot.count.ToString();
        displayImage.sprite = Slot.item.sprite;
        displayImage.enabled = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Slot.item == null)
        {
            return;
        }

        contents.transform.parent = GameObject.Find("Canvas").transform;
        contents.transform.SetAsLastSibling();
        preDragPosition = contents.transform.position;
        onDragBegin?.Invoke(this);
        beingDragged = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Slot.item == null || !beingDragged)
        {
            return;
        }

        onDrag?.Invoke(this);
        contents.transform.position = new Vector3(eventData.position.x, eventData.position.y, 100);
    }

    public void SwapWithAnotherContainer(ItemContainerUI targetContainer)
    {
        var origionalItem = targetContainer.Slot.item;
        var origionalCount = targetContainer.Slot.count;

        targetContainer.SetItem(Slot.item, Slot.count);
        SetItem(origionalItem, origionalCount);
    }

    public void HandleDropOnContainer(ItemContainerUI container)
    {
        // swap with another item
        if (container.Slot.item != null && container.Slot.item != Slot.item)
        {
            SwapWithAnotherContainer(container);
        }

        // fill another container that has this same item
        else if (container.Slot.item == Slot.item || container.Slot.item == null)
        {
            TransferToAnotherContainer(container);
        }
    }

    public void TransferToAnotherContainer(ItemContainerUI targetContainer)
    {
        var remaining = targetContainer.Slot.AddAndReturnDifference(Slot.item, Slot.count);
        targetContainer.Render();

        if (remaining == 0)
        {
            Empty();
        }
        else if (remaining > 0)
        {
            SetCount(remaining);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!beingDragged)
        {
            return;
        }

        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        foreach(var target in raycastResults)
        {
            if (target.gameObject.TryGetComponent<ItemContainerUI>(out ItemContainerUI container))
            {
                if(container == this)
                {
                    continue;
                }

                HandleDropOnContainer(container);
                break;
            }
        }

        if (raycastResults[0].gameObject.TryGetComponent(out Backplate backplate))
        {
            backplate.HandleItemDropped(Slot);
            Empty();
        }

        contents.transform.parent = this.transform;
        contents.transform.position = preDragPosition;
        beingDragged = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (InventoryController.AccessedBy != null)
            {
                TransferTo(InventoryController.AccessedBy);
                return;
            }

            if (InventoryController.Accessing != null)
            {
                TransferTo(InventoryController.Accessing);
                return;
            }
        }
    }

    public void TransferTo(IInventoryController inventoryController)
    {
        var remaining = inventoryController.Inventory.AddAndReturnRemaining(Slot.item, Slot.count);

        if (remaining == 0)
        {
            Empty();
            return;
        }

        SetCount(remaining);
    }

    public void Empty()
    {
        Slot.Empty();
        Render();
    }
}
