using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UntoldTracks.Inventory;

namespace UntoldTracks.UI
{

    public class PlayerInventoryUI : InventoryUI
    {
        [SerializeField]
        private InventoryUI _linkedInventory;

        [SerializeField]
        //private Button _btnClose;

        public UnityAction OnClose;

        private void OnEnable()
        {
            //_btnClose.onClick.AddListener(OnClose);
            RenderContainers();
        }

        private void OnDisable()
        {
            //_btnClose.onClick.RemoveListener(OnClose);
        }

        public void Open(IInventory inventory=null)
        {
            if (inventory != null)
            {
                _linkedInventory.gameObject.SetActive(true);
                _linkedInventory.LinkToInventory(inventory);
            }
        }

        public void Close()
        {
            _linkedInventory.gameObject.SetActive(false);
        }
    }
}