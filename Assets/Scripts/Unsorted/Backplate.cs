using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Backplate : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public UnityEvent onPointerUp;

    public ItemContainerWorldObject itemWorldObjectPrefab;

    public void HandleItemDropped(ItemContainer container)
    {
        var worldItem = Instantiate(itemWorldObjectPrefab, GameObject.Find("PlayerController").transform.position + (GameObject.Find("PlayerController").transform.forward), Quaternion.identity);
        worldItem.GetComponent<ItemContainerWorldObject>().SetItemContainer(container);
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {

    }
}
