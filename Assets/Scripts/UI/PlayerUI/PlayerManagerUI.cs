using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UntoldTracks.InventorySystem;
using UntoldTracks.Player;
using UntoldTracks.Models;
using UntoldTracks.Managers;

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

        public bool isInventoryOpen = false;

        private bool _isUiOpen = false;

        public bool IsUiOpen
        {
            get
            {
                return _isUiOpen;
            }
        }

        public void Init(PlayerManager manager)
        {
            playerManager = manager;

            playerInventoryBarUI.LinkToInventory(manager.InventoryController.Inventory, 0, manager.InventoryController.SizeBase);
            playerInventoryUI.LinkToInventory(manager.InventoryController.Inventory, manager.InventoryController.BarSize, manager.InventoryController.SizeBase);
            manager.InventoryController.OnActiveItemChanged += HandleActiveItemChanged;
            manager.InteractionController.OnFocusChange += HandleFocusChanged;

            craftingUI.Init(manager);

            playerInventoryUI.gameObject.SetActive(false);
            linkedInventory.gameObject.SetActive(false);
            craftingUI.gameObject.SetActive(false);
            
        }

        private void HandleFocusChanged(IInteractable target)
        {
            if (target == null)
            {
                _interactionControllerUI.HideInteractable();
                return;
            }

            _interactionControllerUI.DisplayInteractable(target);
        }

        private void HandleActiveItemChanged(PlayerManager player, ItemContainer container)
        {
            playerInventoryBarUI.SetActiveIndex(container.Index);
        }

        public void OpenCraftingWindow(List<Recipe> recipes)
        {
            _isUiOpen = true;
            craftingUI.SetCraftingBook(recipes);
            
            playerManager.FirstPersonController.UnlockPointer();
            playerInventoryUI.gameObject.SetActive(true);
            craftingUI.gameObject.SetActive(true);
        }

        public void OpenMainWindow(Inventory linkedInventory=null, List<Recipe> recipes=null)
        {
            if (_isUiOpen)
            {
                CloseMainWindow();
                return;
            }
            
            _isUiOpen = true;
            
            playerManager.FirstPersonController.UnlockPointer();
            playerInventoryUI.gameObject.SetActive(true);
            
            craftingUI.SetCraftingBook(recipes);
            craftingUI.gameObject.SetActive(true);
            
            if (linkedInventory != null)
            {
                this.linkedInventory.gameObject.SetActive(true);
                this.linkedInventory.LinkToInventory(linkedInventory);
            }
        }

        public void CloseMainWindow()
        {
            _isUiOpen = false;

            craftingUI.ClearRecipe();
            playerManager.FirstPersonController.LockPointer();

            playerInventoryUI.gameObject.SetActive(false);
            linkedInventory.gameObject.SetActive(false);
            craftingUI.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (playerManager == null)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (_isUiOpen)
                {
                    CloseMainWindow();
                }
                else
                {
                    OpenMainWindow(null);
                }
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                playerManager.InventoryController.IncreaseActiveIndex();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                playerManager.InventoryController.DecreaseActiveIndex();
            }

            if (Input.GetKeyDown(KeyCode.Alpha0)) playerManager.InventoryController.SetActiveIndex(8);
            else if (Input.GetKeyDown(KeyCode.Alpha1)) playerManager.InventoryController.SetActiveIndex(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2)) playerManager.InventoryController.SetActiveIndex(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3)) playerManager.InventoryController.SetActiveIndex(2);
            else if (Input.GetKeyDown(KeyCode.Alpha4)) playerManager.InventoryController.SetActiveIndex(3);
            else if (Input.GetKeyDown(KeyCode.Alpha5)) playerManager.InventoryController.SetActiveIndex(4);
            else if (Input.GetKeyDown(KeyCode.Alpha6)) playerManager.InventoryController.SetActiveIndex(5);
            else if (Input.GetKeyDown(KeyCode.Alpha7)) playerManager.InventoryController.SetActiveIndex(6);
            else if (Input.GetKeyDown(KeyCode.Alpha8)) playerManager.InventoryController.SetActiveIndex(7);
        }
    }
}