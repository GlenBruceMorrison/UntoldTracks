using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks.InventorySystem;

namespace UntoldTracks.Player
{
    public interface IPlayerActiveItem
    {
        //GameObject ActiveItemContainer { get; }
        bool TryGetTool(out EquipableEntity tool);
    }

    public class PlayerActiveItemController : MonoBehaviour, IPlayerActiveItem, IPlayerComponent
    {
        private PlayerManager _playerManager;

        public Transform playerHand;
        public GameObject activeItemObject;

        #region Life Cycle
        public void Init(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        private void OnEnable()
        {
            _playerManager.InventoryController.OnActiveItemChanged += HandleActiveItemChanged;
            HandleActiveItemChanged(_playerManager, _playerManager.InventoryController.ActiveItemContainer);
        }

        private void OnDisable()
        {
            _playerManager.InventoryController.OnActiveItemChanged -= HandleActiveItemChanged;
            EmptyHand();
        }
        #endregion

        /// <summary>
        /// Destroys what the player is currently holding
        /// </summary>
        private void EmptyHand()
        {
            if (activeItemObject == null)
            {
                return;
            }

            Destroy(activeItemObject);
        }

        /// <summary>
        /// Called when the player changes the active item via the hotbar
        /// </summary>
        /// <param name="player">The PlayerManager script that the change originates from</param>
        /// <param name="container">The item container that is now the active item</param>
        private void HandleActiveItemChanged(PlayerManager player, ItemContainer container)
        {
            EmptyHand();
            
            if (container.IsEmpty())
            {
                return;
            }

            if (container.Item.isEquipable && container.Item.equipablePrefab != null)
            {
                SwitchToTool(container);
            }
            else if (container.Item.isPlaceable && container.Item.placeablePrefab != null)
            {
                SwitchToPlaceable(container);
            }
        }

        /// <summary>
        /// If the item is a tool this is called along with the relevant data
        /// </summary>
        /// <param name="toolData">The data for the container which is now active</param>
        private void SwitchToTool(ItemContainer toolData)
        {
            var tool = Instantiate(toolData.Item.equipablePrefab, playerHand.transform);
            activeItemObject = tool.gameObject;

            tool.toolData = toolData;
            activeItemObject.transform.localPosition = Vector3.zero;
            activeItemObject.transform.localEulerAngles = Vector3.zero;

            tool.EquipedByPlayer(_playerManager);
        }

        /// <summary>
        /// If the item is a placeable this is called along with the relevant data
        /// </summary>
        /// <param name="toolData">The data for the container which is now active</param>
        private void SwitchToPlaceable(ItemContainer placaeableData)
        {
            var placeable = Instantiate(placaeableData.Item.placeablePrefab, this.transform);
            activeItemObject = placeable.gameObject;
            _playerManager.PlaceableEntityController.SetTargetPlacable(placeable);
        }

        /// <summary>
        /// Attempts to get the tool the player is currently holding
        /// </summary>
        /// <param name="tool">The ToolEntity object to populate with the active tool</param>
        /// <returns>Returns false if the player is not holding a tool</returns>
        public bool TryGetTool(out EquipableEntity tool)
        {
            if (playerHand.GetComponentInChildren<EquipableEntity>() == null)
            {
                tool = null;
                return false;
            }

            tool = playerHand.GetComponentInChildren<EquipableEntity>();
            return true;
        }
    }
}
