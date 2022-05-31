using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks.InventorySystem;

namespace UntoldTracks.Player
{
    public interface IPlayerActiveItem
    {
        //GameObject ActiveItem { get; }
        bool TryGetTool(out ToolEntity tool);
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
            HandleActiveItemChanged(_playerManager, _playerManager.InventoryController.ActiveItem);
        }

        private void OnDisable()
        {
            _playerManager.InventoryController.OnActiveItemChanged -= HandleActiveItemChanged;
            EmptyHand();
        }
        #endregion

        private void EmptyHand()
        {
            if (activeItemObject == null)
            {
                return;
            }

            Destroy(activeItemObject);
        }

        private void HandleActiveItemChanged(PlayerManager player, ItemContainer container)
        {
            EmptyHand();
            
            if (container.IsEmpty())
            {
                return;
            }

            if (container.Item.isTool && container.Item.toolPrefab != null)
            {
                SwitchToTool(container);
            }
            else if (container.Item.isPlaceable && container.Item.placeablePrefab != null)
            {
                SwitchToPlaceable(container.Item.placeablePrefab);
            }
        }

        private void SwitchToTool(ItemContainer container)
        {
            activeItemObject = Instantiate(container.Item.toolPrefab.gameObject, playerHand.transform);
            activeItemObject.GetComponent<ToolEntity>().container = container;
            activeItemObject.transform.localPosition = Vector3.zero;
            activeItemObject.transform.localEulerAngles = Vector3.zero;
        }

        private void SwitchToPlaceable(PlaceableEntity placeablePrefab)
        {
            var obj = Instantiate(placeablePrefab, this.transform);
            activeItemObject = obj.gameObject;
            //_playerMananger.PlaceableEntityController.gameObject.SetActive(true);
            _playerManager.PlaceableEntityController.SetTargetPlacable(obj);
        }

        public bool TryGetTool(out ToolEntity tool)
        {
            if (playerHand.GetComponentInChildren<ToolEntity>() == null)
            {
                tool = null;
                return false;
            }

            tool = playerHand.GetComponentInChildren<ToolEntity>();
            return true;
        }
    }
}
