using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UntoldTracks.Player;
using UntoldTracks.UI;

namespace UntoldTracks.InventorySystem
{
    [System.Serializable]
    public class ItemContainerTemplate
    {
        public Item item;
        public int count;
    }

    public delegate void ActiveItemChanged(PlayerManager player, ItemContainer container);

    public class PlayerInventoryController : PlayerComponent
    {
        #region private
        [SerializeField] private List<ItemContainerTemplate> _initialContents = new List<ItemContainerTemplate>();

        private const int _inventoryBarSize = 9;
        private const int _inventorySize = 25;
        
        private int _activeItemIndex = 0;
        private Inventory _inventory;
        private bool _isOpen = false;
        #endregion

        #region events
        public UnityEvent OnClose, OnOpen;

        public event ActiveItemChanged OnActiveItemChanged;
        #endregion

        #region getters
        public Inventory Inventory
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

        public ItemContainer ActiveItem
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

                Inventory.Give(new ItemContainer(containerTemplate.item, containerTemplate.count));
            }
        }

        private void LinkToInventory()
        {
            _playerManager.PlayerManagerUI.LinkInventory(_inventory, _inventorySize, _inventoryBarSize);
            _activeItemIndex = 0;
        }

        public void Close()
        {
            _playerManager.PlayerManagerUI.CloseInventory();
            _isOpen = false;
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

            OnActiveItemChanged?.Invoke(_playerManager, ActiveItem);

            _playerManager.PlayerManagerUI.SetActiveItemIndex(_activeItemIndex);
        }

        public void Open(Inventory linkedInventory)
        {
            _playerManager.PlayerManagerUI.OpenInventory(linkedInventory);
            _isOpen = true;
        }

        private void HandleItemContainerUpdate(ItemContainer container)
        {
            if (container.Index == _activeItemIndex)
            {
                OnActiveItemChanged?.Invoke(_playerManager, ActiveItem);
            }
        }
        
        #region Player Component
        protected override void Initiate()
        {
            Close();
            
            _inventory = new Inventory(_inventorySize);

            if (_initialContents != null)
            {
                Seed();
            }

            LinkToInventory();
        }

        private void OnEnable()
        {
            _inventory.OnContainerModified += HandleItemContainerUpdate;
        }

        private void OnDisable()
        {
            _inventory.OnContainerModified -= HandleItemContainerUpdate;
        }
        
        protected override void Run(float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (_isOpen)
                {
                    Close();
                }
                else
                {
                    Open(null);
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
        #endregion
    }
}