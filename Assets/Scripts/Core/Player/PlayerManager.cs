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

        private List<IPlayerComponent> _components = new List<IPlayerComponent>();
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
            {
                throw new System.Exception("This player manager must have a PlayerInventoryController script attached");
            }
            _components.Add(_inventoryController);

            _characterController = GetComponentInChildren<PlayerCharacterController>();
            if (_characterController == null)
            {
                throw new System.Exception("This player manager must have a PlayerCharacterController script attached");
            }
            _components.Add(_characterController);

            _interactionController = GetComponentInChildren<PlayerInteractionController>();
            if (_interactionController == null)
            {
                throw new System.Exception("This player manager must have a PlayerInteractionController script attached");
            }
            _components.Add(_interactionController);

            _playerActiveItemController = GetComponentInChildren<PlayerActiveItemController>();
            if (_playerActiveItemController == null)
            { 
                throw new System.Exception("This player manager must have an PlayerActiveItem script attached");
            }
            _components.Add(_playerActiveItemController);

            _playerManagerUI = GetComponentInChildren<PlayerManagerUI>();
            if (_playerManagerUI == null)
            {
                throw new System.Exception("This player manager must have a PlayerManagerUI script attached");
            }
            _components.Add(_playerManagerUI);

            _placeableEntityController = GetComponentInChildren<PlaceableEntityController>();
            if (_placeableEntityController == null)
            {
                throw new System.Exception("This player manager must have a PlaceableEntityController script attached");
            }
            _components.Add(_placeableEntityController);

            InitManagers();
        }

        private void InitManagers()
        {
            foreach (var manager in _components)
            {
                manager.Init(this);
            }
        }
    }
}