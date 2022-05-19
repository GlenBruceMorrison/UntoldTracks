using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks.Inventory;

namespace UntoldTracks.Player
{
    public class PlayerActiveItem : MonoBehaviour
    {
        private PlayerManager _playerMananger;
        public Transform playerHand;

        public PlaceableEntityController _placeableEntityController => GameObject.FindObjectOfType<PlaceableEntityController>();

        public PlayerManager PlayerManager
        {
            get
            {
                if (_playerMananger == null)
                {
                    _playerMananger = FindObjectOfType<PlayerManager>();
                }

                return _playerMananger;
            }
        }

        public Item ActiveItem
        {
            get
            {
                return _playerMananger.inventoryController.ActiveItem?.Item;
            }
        }

        public GameObject activeItemObject;

        private void OnEnable()
        {
            PlayerManager.inventoryController.OnActiveItemChanged += HandleActiveItemChanged;
            HandleActiveItemChanged(PlayerManager, PlayerManager.inventoryController.ActiveItem);
        }

        private void OnDisable()
        {
            _playerMananger.inventoryController.OnActiveItemChanged -= HandleActiveItemChanged;
            EmptyHand();
        }

        public void EmptyHand()
        {
            if (activeItemObject == null)
            {
                return;
            }

            Destroy(activeItemObject);
        }

        private void HandleActiveItemChanged(PlayerManager player, IItemContainer container)
        {
            EmptyHand();

            if (container.IsEmpty())
            {
                return;
            }

            if (container.Item.isTool && container.Item.toolPrefab != null)
            {
                SwitchToTool(container.Item.toolPrefab);
            }
            else if (container.Item.isPlaceable && container.Item.placeablePrefab != null)
            {
                SwitchToPlaceable(container.Item.placeablePrefab);
            }
        }

        public void SwitchToTool(GameObject toolPrefab)
        {
            activeItemObject = Instantiate(toolPrefab, playerHand.transform);
            activeItemObject.transform.localPosition = Vector3.zero;
            activeItemObject.transform.localEulerAngles = Vector3.zero;
        }

        public void SwitchToPlaceable(PlaceableEntity placeablePrefab)
        {
            var obj = Instantiate(placeablePrefab, this.transform);
            activeItemObject = obj.gameObject;
            _placeableEntityController.gameObject.SetActive(true);
            _placeableEntityController.EquipPlaceable(obj);
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
