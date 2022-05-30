using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UntoldTracks.Player;
using UntoldTracks.UI;

namespace UntoldTracks.InventorySystem
{

    public delegate void ActiveItemChanged(PlayerManager player, ItemContainer container);

    public class PlayerInventoryController : MonoBehaviour, IPlayerComponent
    {
        #region private
        private PlayerManager _playerManager;

        private const int _inventoryBarSize = 9;
        private const int _inventorySize = 25;
        
        private int _activeItemIndex = 0;
        private Inventory _inventory;
        private bool _isOpen = false;
        #endregion

        public InventorySeed inventorySeed;
        
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
        
        private void LinkToInventory()
        {
            _playerManager.PlayerManagerUI.LinkInventory(_inventory, _inventorySize, _inventoryBarSize);
            _activeItemIndex = 0;
        }

        public void Close()
        {
            _playerManager.PlayerManagerUI.CloseMainWindow();
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
            _playerManager.PlayerManagerUI.OpenMainWindow(linkedInventory);
            _isOpen = true;
        }

        private void HandleItemContainerUpdate(ItemContainer container)
        {
            if (container.Index == _activeItemIndex)
            {
                OnActiveItemChanged?.Invoke(_playerManager, ActiveItem);
            }
        }

        #region Life Cycle
        public void Init(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        private void Awake()
        {
            Close();
            
            _inventory = new Inventory(_inventorySize);

            if (inventorySeed != null)
            {
                inventorySeed.Seed(_inventory);
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

            if (Input.GetKeyDown(KeyCode.Alpha0)) SetActiveIndex(8);
            else if (Input.GetKeyDown(KeyCode.Alpha1)) SetActiveIndex(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2)) SetActiveIndex(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3)) SetActiveIndex(2);
            else if (Input.GetKeyDown(KeyCode.Alpha4)) SetActiveIndex(3);
            else if (Input.GetKeyDown(KeyCode.Alpha5)) SetActiveIndex(4);
            else if (Input.GetKeyDown(KeyCode.Alpha6)) SetActiveIndex(5);
            else if (Input.GetKeyDown(KeyCode.Alpha7)) SetActiveIndex(6);
            else if (Input.GetKeyDown(KeyCode.Alpha8)) SetActiveIndex(7);
        }
        #endregion
    }
}