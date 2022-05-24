using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UntoldTracks.InventorySystem;
using UnityEngine;
using UntoldTracks.CharacterController;
using UntoldTracks.UI;

namespace UntoldTracks.Player
{
    public interface IPlayerManangerComponent
    {
        void Init(PlayerManager manager);
    }
    
    public class PlayerManager : MonoBehaviour
    {
        private PlayerInventoryController _inventoryController;
        private PlayerCharacterController _firstPersonController;
        private PlayerInteractionController _interactionController;
        private PlayerActiveItem _playerActiveItem;
        private PlayerManagerUI _playerManagerUI;
        private IPlacableEntityController _placeableEntityController;

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
                return _firstPersonController;
            }
        }

        public PlayerInteractionController InteractionController
        {
            get
            {
                return _interactionController;
            }
        }

        public PlayerActiveItem PlayerActiveItem
        {
            get
            {
                return _playerActiveItem;
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
        
        private void Awake()
        {
            GetDependancies();
            InitManagers();
        }

        private void GetDependancies()
        {
            _inventoryController = GetComponentInChildren<PlayerInventoryController>();
            if (_inventoryController == null) throw new System.Exception("This player manager must have a PlayerInventoryController script attached");

            _firstPersonController = GetComponentInChildren<PlayerCharacterController>();
            if (_firstPersonController == null) throw new System.Exception("This player manager must have a PlayerCharacterController script attached");

            _interactionController = GetComponentInChildren<PlayerInteractionController>();
            if (_interactionController == null) throw new System.Exception("This player manager must have a PlayerInteractionController script attached");

            _playerActiveItem = GetComponentInChildren<PlayerActiveItem>();
            if (_playerActiveItem == null) throw new System.Exception("This player manager must have an PlayerActiveItem script attached"); 

            _playerManagerUI = GetComponentInChildren<PlayerManagerUI>();
            if (_playerManagerUI == null) throw new System.Exception("This player manager must have a PlayerManagerUI script attached");

            _placeableEntityController = GetComponentInChildren<PlaceableEntityController>();
            if (_placeableEntityController == null) throw new System.Exception("This player manager must have a PlaceableEntityController script attached");
        }

        private void InitManagers()
        {
            _inventoryController.Init(this);
            _firstPersonController.Init(this);
            _interactionController.Init(this);
            _playerActiveItem.Init(this);
            _playerManagerUI.Init(this);
            _placeableEntityController.Init(this);
        }
    }
}