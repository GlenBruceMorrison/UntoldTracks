using System.Collections;
using System.Collections.Generic;
using UntoldTracks.InventorySystem;
using UnityEngine;
using UntoldTracks.CharacterController;
using UntoldTracks.UI;

namespace UntoldTracks.Player
{
    public class PlayerManager : MonoBehaviour
    {
        public PlayerInventoryController inventoryController;
        public PlayerCharacterController firstPersonController;
        public PlayerInteractionController interactionController;
        public PlayerActiveItem playerActiveItem;
        public PlayerManagerUI playerManagerUI;

        private void Awake()
        {
            GetDependancies();
            InitManagers();
        }

        private void GetDependancies()
        {
            inventoryController = GetComponentInChildren<PlayerInventoryController>();
            if (inventoryController == null) throw new System.Exception("This player manager must have a PlayerInventoryController script attached");

            firstPersonController = GetComponentInChildren<PlayerCharacterController>();
            if (firstPersonController == null) throw new System.Exception("This player manager must have a PlayerCharacterController script attached");

            interactionController = GetComponentInChildren<PlayerInteractionController>();
            if (interactionController == null) throw new System.Exception("This player manager must have a PlayerInteractionController script attached");

            playerActiveItem = GetComponentInChildren<PlayerActiveItem>();
            if (playerActiveItem == null) throw new System.Exception("This player manager must have an PlayerActiveItem script attached"); 

            playerManagerUI = GetComponentInChildren<PlayerManagerUI>();
            if (playerManagerUI == null) throw new System.Exception("This player manager must have a PlayerManagerUI script attached");
        }

        private void InitManagers()
        {
            inventoryController.Init(this);
            firstPersonController.Init(this);
            interactionController.Init(this);
            playerActiveItem.Init(this);
            playerManagerUI.Init(this);
        }
    }
}