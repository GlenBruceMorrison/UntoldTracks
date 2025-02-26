using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UntoldTracks.InventorySystem;

namespace UntoldTracks.UI
{
    public class CraftingButton : MonoBehaviour, IPointerClickHandler
    {
        public UnityEvent OnClick;

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke();
        }
    }
}
