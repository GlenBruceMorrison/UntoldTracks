using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UntoldTracks.InventorySystem;
using UnityEngine;
using UntoldTracks.CharacterController;
using UntoldTracks.UI;

namespace UntoldTracks.Player
{
    public class PlayerManager : MonoBehaviour
    {
        #region Private
        private PlayerInteractionController _interactionController;
        private PlaceableEntityController _placeableEntityController;
        private PlayerCharacterController _characterController;
        private PlayerInventoryController _inventoryController;
        private PlayerActiveItemController _playerActiveItemController;
        private PlayerManagerUI _playerManagerUI;
        #endregion
        
        #region Getters
        public PlayerInventoryController InventoryController
        {
            get
            {
                return _inventoryController;
            }
        }

        public IFirstPersonController FirstPersonController
        {
            get
            {
                return _characterController;
            }
        }

        public IPlayerInteractionController InteractionController
        {
            get
            {
                return _interactionController;
            }
        }

        public PlayerActiveItemController PlayerActiveItemController
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

        public IPlacableEntityController PlaceableEntityController
        {
            get
            {
                return _placeableEntityController;
            }
        }
        #endregion
        
        private void Awake()
        {
            GetDependancies();
            InitManagers();
        }

        private void GetDependancies()
        {
            _inventoryController = GetComponentInChildren<PlayerInventoryController>();
            if (_inventoryController == null) 
                throw new System.Exception("This player manager must have a PlayerInventoryController script attached");

            _characterController = GetComponentInChildren<PlayerCharacterController>();
            if (_characterController == null)
                throw new System.Exception("This player manager must have a PlayerCharacterController script attached");

            _interactionController = GetComponentInChildren<PlayerInteractionController>();
            if (_interactionController == null)
                throw new System.Exception("This player manager must have a PlayerInteractionController script attached");

            _playerActiveItemController = GetComponentInChildren<PlayerActiveItemController>();
            if (_playerActiveItemController == null)
                throw new System.Exception("This player manager must have an PlayerActiveItem script attached"); 

            _playerManagerUI = GetComponentInChildren<PlayerManagerUI>();
            if (_playerManagerUI == null) 
                throw new System.Exception("This player manager must have a PlayerManagerUI script attached");

            _placeableEntityController = GetComponentInChildren<PlaceableEntityController>();
            if (_placeableEntityController == null)
                throw new System.Exception("This player manager must have a PlaceableEntityController script attached");
        }

        private void InitManagers()
        {
            _inventoryController.InternalInit(this);
            _interactionController.InternalInit(this);
            _playerActiveItemController.InternalInit(this);
            _placeableEntityController.InternalInit(this);
            _characterController.InternalInit(this);
            
            _playerManagerUI.InitPlayerComponent(this);
        }

        private void Update()
        {
            var delta = Time.deltaTime;

            _placeableEntityController.InternalRun(delta);
            _characterController.InternalRun(delta);
            _inventoryController.InternalRun(delta);
            _interactionController.InternalRun(delta);
            _playerActiveItemController.InternalRun(delta);
        }

        private void LateUpdate()
        {
            _characterController.InternalLateRun();
        }
    }
}