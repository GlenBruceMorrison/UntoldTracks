using System.Collections;
using System.Collections.Generic;
using UntoldTracks.InventorySystem;
using UnityEngine;
using UntoldTracks.CharacterController;

namespace UntoldTracks.Player
{
    public class PlayerManager : MonoBehaviour
    {
        public PlayerInventoryController inventoryController;
        public PlayerCharacterController FirstPersonController;
        public PlayerInteractionController interactionController;
        public PlayerActiveItem playerActiveItem;
        //public PlayerBuildingController buildingController;

        private void Awake()
        {
            inventoryController = GetComponentInChildren<PlayerInventoryController>();
            if (inventoryController == null)
            {
                throw new System.Exception("This player mananger must have an inventory controller atatched");
            }
            inventoryController.playerManager = this;

            FirstPersonController = GetComponentInChildren<PlayerCharacterController>();
            if (FirstPersonController == null)
            {
                throw new System.Exception("This player mananger must have an control controller atatched");
            }
            //FirstPersonController.playerManager = this;

            interactionController = GetComponentInChildren<PlayerInteractionController>();
            if (interactionController == null)
            {
                throw new System.Exception("This player mananger must have an interaction controller atatched");
            }
            interactionController.playerManager = this;

            playerActiveItem = GetComponentInChildren<PlayerActiveItem>();
            if (playerActiveItem == null)
            {
                throw new System.Exception("This player mananger must have an active item controller atatched");
            }

            //buildingController = GetComponentInChildren<PlayerBuildingController>();
            //if (buildingController == null)
            //{
            //    throw new System.Exception("This player mananger must have a building controller attatched");
            //}
            //buildingController.playerManager = this;
        }
    }
}