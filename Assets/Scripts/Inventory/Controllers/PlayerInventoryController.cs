using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace UntoldTracks.Inventory
{
    [System.Serializable]
    public class ItemContainerTemplate
    {
        public Item item;
        public int count;
    }

    public class PlayerInventoryController : MonoBehaviour
    {
        [SerializeField]
        private List<ItemContainerTemplate> _initialContents = new List<ItemContainerTemplate>();

        [SerializeField]
        private PlayerInventoryUI _ui;

        [SerializeField]
        private PlayerInventoryBarUI _uiInventoryBar;

        internal PlayerManager playerManager;
        private IInventory _inventory;
        private bool _isOpen = false;

        public UnityEvent OnClose, OnOpen;

        public IInventory Inventory
        {
            get
            {
                return _inventory;
            }
        }

        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
        }

        public void Seed()
        {
            foreach (var containerTemplate in _initialContents)
            {
                if (containerTemplate.item == null || containerTemplate.count < 1)
                {
                    continue;
                }

                Inventory.FillAndReturnRemaining(containerTemplate.item, containerTemplate.count);
            }
        }

        public void Awake()
        {
            _inventory = new Inventory(25);

            if (_initialContents != null)
            {
                Seed();
            }

            _uiInventoryBar.LinkToInventory(Inventory, 0, 9);
            _ui.LinkToInventory(Inventory, 9, 25);

            _ui.OnClose += Close;
        }

        private void Start()
        {
            Close();
        }

        public void OpenOtherInventory(IInventory inventory)
        {
            Open();
            _ui.Open(inventory);
        }

        public void Close()
        {
            playerManager.FirstPersonController.LockPointer();
            _ui.gameObject.SetActive(false);
            _ui.Close();
            _isOpen = false;
            OnClose?.Invoke();
        }

        public void Open()
        {
            playerManager.FirstPersonController.UnlockPointer();
            _ui.gameObject.SetActive(true);
            _ui.Open();
            _isOpen = true;
            OnOpen?.Invoke();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (_isOpen)
                {
                    Close();
                }
                else
                {
                    Open();
                }
            }
        }
    }
}