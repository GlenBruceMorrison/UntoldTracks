using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Linq;

public class ContainerUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public TMP_Text countText;
    public Image displayImage;

    public GameObject contents;

    public Inventory Inventory { get; private set; }
    public int SlotIndex { get; private set; }

    public UnityAction<ContainerUI> onDragBegin, onDrag, onDrop;

    public ItemContainer Slot => Inventory.containers[SlotIndex];

    public bool beingDragged;
    public Vector2 preDragPosition;


    public void LinkToInventory(Inventory targetInventory, int targetSlotIndex)
    {
        Inventory = targetInventory;
        SlotIndex = targetSlotIndex;
        Render();
    }

    public void SetItem(Item item, int count)
    {
        Slot.item = item;
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

        countText.text = Slot.count.ToString();
        displayImage.sprite = Slot.item.sprite;
        displayImage.enabled = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Slot.item == null)
        {
            return;
        }

        contents.transform.parent = this.transform.parent.parent;
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

    public void HandleDropOnContainer(ContainerUI container)
    {
        // swap with another item
        if (container.Slot.item != null && container.Slot.item != Slot.item)
        {
            var origionalItem = container.Slot.item;
            var origionalCount = container.Slot.count;

            container.SetItem(Slot.item, Slot.count);
            SetItem(origionalItem, origionalCount);
        }

        // fill another container that has this same item
        else if (container.Slot.item == Slot.item || container.Slot.item == null)
        {
            var remaining = container.Slot.AddAndReturnDifference(Slot.item, Slot.count);
            container.Render();

            if (remaining == 0)
            {
                Empty();
            }
            else if (remaining > 0)
            {
                Slot.count = remaining;
                Render();
            }
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
            if (target.gameObject.TryGetComponent<ContainerUI>(out ContainerUI container))
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

    }

    public void Empty()
    {
        Slot.Empty();
        Render();
    }
}
