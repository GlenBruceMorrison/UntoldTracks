using System;
using System.Collections;
using System.Collections.Generic;
using UntoldTracks.InventorySystem;
using UnityEngine;
using UntoldTracks.CharacterController;
using UntoldTracks.UI;
using UntoldTracks.Player;
using UntoldTracks.Managers;
using UntoldTracks.Models;
using SimpleJSON;

namespace UntoldTracks.Managers
{
    public class PlayerManager : MonoBehaviour, ITokenizable
    {
        [SerializeField] private PlayerInteractionController _interactionController;
        [SerializeField] private PlaceableEntityController _placeableEntityController;
        [SerializeField] private PlayerCharacterController _characterController;
        [SerializeField] private PlayerInventoryController _inventoryController;
        [SerializeField] private PlayerHand _playerActiveItemController;
        [SerializeField] private PlayerManagerUI _playerManagerUI;
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

        #region Token
        public void Load(JSONNode node)
        {
            _characterController = GetComponentInChildren<PlayerCharacterController>();

            _interactionController = new PlayerInteractionController(this, _playerCamera);
            _inventoryController = new PlayerInventoryController(this);
            _playerActiveItemController = new PlayerHand(this, _playerHand);

            _inventoryController.Load(node["inventory"]);
            _playerActiveItemController.Init();
            _interactionController.Init();
            _playerManagerUI.Init(this);
        }

        public JSONObject Save()
        {
            var playerJSONObject = new JSONObject();

            var characterControllerJSONObject = new JSONObject();

            var positionJSON = new JSONObject();
            positionJSON.Add("x", _characterController.transform.position.x);
            positionJSON.Add("y", _characterController.transform.position.y);
            positionJSON.Add("z", _characterController.transform.position.z);

            var rotationJSON = new JSONObject();
            rotationJSON.Add("x", transform.rotation.x);
            rotationJSON.Add("y", transform.rotation.y);
            rotationJSON.Add("z", transform.rotation.z);

            characterControllerJSONObject.Add("position", positionJSON);
            characterControllerJSONObject.Add("rotation", rotationJSON);

            playerJSONObject.Add("entity", characterControllerJSONObject);
            playerJSONObject.Add("inventory", _inventoryController.Inventory.Save());

            return playerJSONObject;
        }
        #endregion

        private void Update()
        {
            _interactionController?.Tick();
        }
    }
}