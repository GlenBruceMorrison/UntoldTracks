﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UntoldTracks.Player;
using UntoldTracks.UI;

namespace UntoldTracks.Inventory
{
    [System.Serializable]
    public class ItemContainerTemplate
    {
        public Item item;
        public int count;
    }

    public delegate void ActiveItemChanged(PlayerManager player, IItemContainer container);

    public class PlayerInventoryController : MonoBehaviour
    {
        #region private
        [SerializeField]
        private List<ItemContainerTemplate> _initialContents = new List<ItemContainerTemplate>();

        [SerializeField]
        private PlayerInventoryUI _ui;

        [SerializeField]
        private PlayerInventoryBarUI _uiInventoryBar;

        private int _inventoryBarSize = 9;
        private int _inventorySize = 25;
        private int _activeItemIndex = 0;
        internal PlayerManager playerManager;
        private IInventory _inventory;
        private bool _isOpen = false;
        #endregion

        #region events
        public UnityEvent OnClose, OnOpen;

        public event ActiveItemChanged OnActiveItemChanged;
        #endregion

        #region getters
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

        public IItemContainer ActiveItem
        {
            get
            {
                return _inventory.Containers[_activeItemIndex];
            }
        }

        public bool HasActiveItem
        {
            get
            {
                return !(ActiveItem == null || ActiveItem.IsEmpty());
            }
        }
        #endregion

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
            _inventory = new Inventory(_inventorySize);

            if (_initialContents != null)
            {
                Seed();
            }

            LinkToInventory();

            _ui.OnClose += Close;
        }

        private void OnEnable()
        {
            _inventory.OnContainerModified += HandleItemContainerUpdate;
        }

        private void OnDisable()
        {
            _inventory.OnContainerModified -= HandleItemContainerUpdate;
        }

        private void LinkToInventory()
        {
            _uiInventoryBar.LinkToInventory(Inventory, 0, _inventoryBarSize);
            _ui.LinkToInventory(Inventory, _inventoryBarSize, _inventorySize);
            _activeItemIndex = 0;
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

        public void SetActiveIndex(int index)
        {
            var newIndex = index;

            if (index > _inventoryBarSize-1)
            {
                newIndex = 0;
            }
            else if (index < 0)
            {
                newIndex = _inventoryBarSize - 1;
            }

            _activeItemIndex = newIndex;

            OnActiveItemChanged?.Invoke(playerManager, ActiveItem);

            _uiInventoryBar.SetActiveIndex(_activeItemIndex);
        }

        public void Open()
        {
            playerManager.FirstPersonController.UnlockPointer();
            _ui.gameObject.SetActive(true);
            _ui.Open();
            _isOpen = true;
            OnOpen?.Invoke();
        }

        private void HandleItemContainerUpdate(IItemContainer container)
        {
            if (container.Index == _activeItemIndex)
            {
                OnActiveItemChanged?.Invoke(playerManager, ActiveItem);
            }
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

            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                SetActiveIndex(_activeItemIndex + 1);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                SetActiveIndex(_activeItemIndex - 1);
            }

            if (Input.GetKeyDown("0")) SetActiveIndex(8);
            else if (Input.GetKeyDown("1")) SetActiveIndex(0);
            else if (Input.GetKeyDown("2")) SetActiveIndex(1);
            else if (Input.GetKeyDown("3")) SetActiveIndex(2);
            else if (Input.GetKeyDown("4")) SetActiveIndex(3);
            else if (Input.GetKeyDown("5")) SetActiveIndex(4);
            else if (Input.GetKeyDown("6")) SetActiveIndex(5);
            else if (Input.GetKeyDown("7")) SetActiveIndex(6);
            else if (Input.GetKeyDown("8")) SetActiveIndex(7);
        }
    }
}