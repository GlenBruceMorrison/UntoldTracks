using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UntoldTracks.Player;
using UntoldTracks.UI;
using UntoldTracks.Data;
using UntoldTracks.Managers;

namespace UntoldTracks.InventorySystem
{
    public delegate void ActiveItemChanged(PlayerManager player, ItemContainer container);

    public class PlayerInventoryController
    {
        public PlayerManager playerManager;

        public const int _inventoryBarSize = 9;
        public const int _inventorySize = 25;
        
        private int _activeItemIndex = 0;
        [SerializeField] private Inventory _inventory;
        [SerializeField] ItemRegistry _itemRegistry;
        [SerializeField] private ItemContainerUI containerDragHandler;

        public int SizeBase => _inventorySize;
        public int BarSize => _inventoryBarSize;

        public PlayerInventoryController(PlayerManager manager, ItemRegistry itemRegistry)
        {
            playerManager = manager;
            _itemRegistry = itemRegistry;
        }

        public void Init(InventoryData data)
        {
            _inventory = new Inventory(data, _itemRegistry);
            _inventory.OnContainerModified += HandleItemContainerUpdate;
        }

        public event ActiveItemChanged OnActiveItemChanged;

        public Inventory Inventory
        {
            get
            {
                return _inventory;
            }
        }

        public ItemContainer ActiveItemContainer
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
                return !(ActiveItemContainer == null || ActiveItemContainer.IsEmpty());
            }
        }

        public void IncreaseActiveIndex()
        {
            SetActiveIndex(_activeItemIndex + 1);
        }

        public void DecreaseActiveIndex()
        {
            SetActiveIndex(_activeItemIndex - 1);
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

            OnActiveItemChanged?.Invoke(playerManager, ActiveItemContainer);
        }

        private void HandleItemContainerUpdate(ItemContainer container)
        {
            if (container.Index == _activeItemIndex)
            {
                OnActiveItemChanged?.Invoke(playerManager, ActiveItemContainer);
            }
        }
    }
}