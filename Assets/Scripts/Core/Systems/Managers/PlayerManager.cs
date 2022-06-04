using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UntoldTracks.InventorySystem;
using UnityEngine;
using UntoldTracks.CharacterController;
using UntoldTracks.UI;
using UntoldTracks.Data;
using UntoldTracks.Player;
using UntoldTracks.Managers;
using UntoldTracks.Models;

namespace UntoldTracks.Managers
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private PlayerInteractionController _interactionController;
        [SerializeField] private PlaceableEntityController _placeableEntityController;
        [SerializeField] private PlayerCharacterController _characterController;
        [SerializeField] private PlayerInventoryController _inventoryController;
        [SerializeField] private PlayerHand _playerActiveItemController;
        [SerializeField] private PlayerManagerUI _playerManagerUI;

        [SerializeField] private ItemRegistry _itemRegistry;
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private Transform _playerHand;

        public PlayerInventoryController InventoryController
        {
            get
            {
                return _inventoryController;
            }
        }

        public PlayerCharacterController FirstPersonController
        {
            get
            {
                return _characterController;
            }
        }

        public PlayerInteractionController InteractionController
        {
            get
            {
                return _interactionController;
            }
        }

        public PlayerHand PlayerActiveItemController
        {
            get
            {
                return _playerActiveItemController;
            }
        }

        public PlayerManagerUI PlayerManagerUI
        {
            get
            {
                return _playerManagerUI;
            }
        }

        public PlaceableEntityController PlaceableEntityController
        {
            get
            {
                return _placeableEntityController;
            }
        }

        public void Build(PlayerData data)
        {
            _characterController = GetComponentInChildren<PlayerCharacterController>();

            _interactionController = new PlayerInteractionController(this, _playerCamera);
            _inventoryController = new PlayerInventoryController(this, _itemRegistry);
            _playerActiveItemController = new PlayerHand(this, _playerHand);

            _inventoryController.Init(data.inventory);
            _playerActiveItemController.Init();
            _interactionController.Init();

            _playerManagerUI.Init(this);
        }

        private void Update()
        {
            _interactionController.Tick();
        }

        public PlayerData Save()
        {
            var result = new PlayerData()
            {
                inventory = InventoryController.Inventory.SaveToData(),
                posX = _characterController.transform.position.x,
                posY = _characterController.transform.position.y,
                posZ = _characterController.transform.position.z
            };

            return result;
        }
    }
}