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

        [SerializeField] private SerializableRegistry _registry;
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

        public PlayerData Save(string f)
        {
            var result = new PlayerData()
            {
                inventory = InventoryController.Inventory.SaveToData(),
                entity = EntityData.FromTransform(transform)
            };

            return result;
        }

        public void Load(JSONNode node)
        {
            _characterController = GetComponentInChildren<PlayerCharacterController>();

            _interactionController = new PlayerInteractionController(this, _playerCamera);
            _inventoryController = new PlayerInventoryController(this, _registry);
            _playerActiveItemController = new PlayerHand(this, _playerHand);

            _inventoryController.Load(node["inventory"]);
            _playerActiveItemController.Init();
            _interactionController.Init();

            _playerManagerUI.Init(this);
        }

        public JSONObject Save()
        {
            var playerJSON = new JSONObject();

            var entityJSON = new JSONObject();

            var positionJSON = new JSONObject();
            positionJSON.Add("x", _characterController.transform.position.x);
            positionJSON.Add("y", _characterController.transform.position.y);
            positionJSON.Add("z", _characterController.transform.position.z);

            var rotationJSON = new JSONObject();
            rotationJSON.Add("x", transform.rotation.x);
            rotationJSON.Add("y", transform.rotation.y);
            rotationJSON.Add("z", transform.rotation.z);

            entityJSON.Add("position", positionJSON);
            entityJSON.Add("rotation", rotationJSON);

            playerJSON.Add("entity", entityJSON);
            playerJSON.Add("inventory", _inventoryController.Inventory.Save());

            return playerJSON;
        }

        private void Update()
        {
            _interactionController?.Tick();
        }
    }
}