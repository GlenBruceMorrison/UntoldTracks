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
        [SerializeField] private ItemContainerDragHandler _itemContainerDragHandler;
        [SerializeField] private PlayerInteractionControllerUI _interactionCanvas;

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

            playerInventoryBarUI.LinkToInventory(manager.InventoryController.Inventory, 0, manager.InventoryController.BarSize);
            playerInventoryUI.LinkToInventory(manager.InventoryController.Inventory, manager.InventoryController.BarSize, manager.InventoryController.SizeBase);

            _itemContainerDragHandler.Init(manager.InventoryController);

            //manager.InventoryController.Inventory.OnModified += HandleInventoryModified;
            manager.InventoryController.OnActiveItemChanged += HandleActiveItemChanged;
            manager.InteractionController.OnFocusChange += HandleFocusChanged;

            craftingUI.Init(manager);

            playerInventoryUI.gameObject.SetActive(false);
            linkedInventory.gameObject.SetActive(false);
            craftingUI.gameObject.SetActive(false);
            
        }

        private void HandleInventoryModified()
        {
            this.playerInventoryBarUI.Render();
            this.playerInventoryUI.Render();
        }

        private void HandleFocusChanged(IInteractable target)
        {
            if (target == null)
            {
                _interactionCanvas.HideInteractable();
                return;
            }

            _interactionCanvas.MoveToInteraction(target);
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

            _itemContainerDragHandler.HandleInventoryOpened();

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

            _itemContainerDragHandler.HandleInventoryClosed();

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

            _itemContainerDragHandler.Tick();

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