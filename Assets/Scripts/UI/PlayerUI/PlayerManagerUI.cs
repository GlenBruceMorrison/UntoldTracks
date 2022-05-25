using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UntoldTracks.InventorySystem;
using UntoldTracks.Player;

namespace UntoldTracks.UI
{
    public class PlayerManagerUI : MonoBehaviour
    {
        public PlayerManager playerManager;

        [SerializeField] private PlayerInventoryUI playerInventoryUI;
        [SerializeField] private PlayerInventoryBarUI playerInventoryBarUI;
        [SerializeField] private InventoryUI linkedInventory;
        [SerializeField] private CraftingUI craftingUI;
        [SerializeField] private PlayerInteractionControllerUI _interactionControllerUI;

        private bool _isUiOpen = false;

        public bool IsUiOpen
        {
            get
            {
                return _isUiOpen;
            }
        }
        
        public void InitPlayerComponent(PlayerManager playerManager)
        {
            this.playerManager = playerManager;

            playerInventoryUI.gameObject.SetActive(false);
            linkedInventory.gameObject.SetActive(false);
            craftingUI.gameObject.SetActive(false);
        }

        public void LinkInventory(Inventory inventory, int inventorySize, int inventoryBarSize)
        {
            playerInventoryBarUI.LinkToInventory(inventory, 0, inventoryBarSize);
            playerInventoryUI.LinkToInventory(inventory, inventoryBarSize, inventorySize);
        }

        public void OpenInventory(Inventory linkedInventory=null)
        {
            _isUiOpen = true;
            
            playerManager.FirstPersonController.UnlockPointer();
            playerInventoryUI.gameObject.SetActive(true);
            craftingUI.gameObject.SetActive(true);

            if (linkedInventory != null)
            {
                this.linkedInventory.gameObject.SetActive(true);
                this.linkedInventory.LinkToInventory(linkedInventory);
            }
        }

        public void CloseInventory()
        {
            _isUiOpen = false;
            
            playerManager.FirstPersonController.LockPointer();

            playerInventoryUI.gameObject.SetActive(false);
            linkedInventory.gameObject.SetActive(false);
            craftingUI.gameObject.SetActive(false);
        }

        public void SetActiveItemIndex(int index)
        {
            playerInventoryBarUI.SetActiveIndex(index);
        }

        public void DisplayInteractable(IInteractable interactable)
        {
            _interactionControllerUI.DisplayInteractable(interactable);
        }
        
        public void HideInteractable()
        {
            _interactionControllerUI.HideInteractable();
        }
    }
}