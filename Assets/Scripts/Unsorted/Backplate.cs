using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Backplate : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public UnityEvent onPointerUp;

    public void HandleItemDropped(ItemContainer container)
    {
        Debug.Log($"Dropping {container.count} {container.item.name}(s)");
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }


    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {

    }
}
