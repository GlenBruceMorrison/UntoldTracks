using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UntoldTracks.Player;
using UntoldTracks.UI;
using UntoldTracks.Data;
using UntoldTracks.Managers;
using SimpleJSON;

namespace UntoldTracks.InventorySystem
{
    public delegate void ActiveItemChanged(PlayerManager player, ItemContainer container);

    public class PlayerInventoryController : ITokenizable
    {
        public PlayerManager playerManager;

        public const int _inventoryBarSize = 9;
        public int _inventorySize = 29;
        
        private int _activeItemIndex = 0;
        [SerializeField] private Inventory _inventory;
        [SerializeField] private ItemContainerUI containerDragHandler;

        public int SizeBase => _inventorySize;
        public int BarSize => _inventoryBarSize;

        public PlayerInventoryController(PlayerManager manager, SerializableRegistry registry)
        {
            playerManager = manager;
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

        #region Token
        public void Load(JSONNode node)
        {
            _inventory = new Inventory(node);
            _inventorySize = node["size"];
            _inventory.OnContainerModified += HandleItemContainerUpdate;
        }

        public JSONObject Save()
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}