using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UntoldTracks.Inventory;

namespace UntoldTracks.UI
{
    public class CraftingButton : MonoBehaviour, IPointerClickHandler
    {
        public UnityAction OnClick;

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke();
        }
    }
}
