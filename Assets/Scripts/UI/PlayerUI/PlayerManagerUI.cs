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

        [SerializeField]
        private PlayerInventoryUI playerInventoryUI;
        [SerializeField]
        private PlayerInventoryBarUI playerInventoryBarUI;
        [SerializeField]
        private InventoryUI linkedInventory;
        [SerializeField]
        private CraftingUI craftingUI;

        private void Awake()
        {
            playerManager = GetComponentInParent<PlayerManager>();

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
            playerManager.FirstPersonController.LockPointer();

            playerInventoryUI.gameObject.SetActive(false);
            linkedInventory.gameObject.SetActive(false);
            craftingUI.gameObject.SetActive(false);
        }

        public void SetActiveItemIndex(int index)
        {
            playerInventoryBarUI.SetActiveIndex(index);
        }
    }
}