using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UntoldTracks.Inventory;

namespace UntoldTracks.Player
{
    public class PlayerInteractionController : MonoBehaviour
    {
        public PlayerManager playerManager;

        public Camera playerCamera;

        [SerializeField]
        private PlayerInteractionControllerUI _uiInteractionController;

        public float interactionDistance = 5;

        public IInteractable currentFocus;

        private void Update()
        {
            if (playerManager.FirstPersonController.IsPointerLocked())
            {
                return;
            }

            DetermineLookStates();

            DetermineInteractionStates();
        }

        private void DetermineLookStates()
        {
            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactionDistance, Color.red);

            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit2, interactionDistance))
            {
                if (hit2.collider.gameObject.TryGetComponent(out IInteractable interactable))
                {
                    LookingAtInteractable(interactable);
                }
                else
                {
                    NotLookingAtInteractable();
                }
            }
            else
            {
                NotLookingAtAnything();
            }
        }

        #region LookStates
        private void LookingAtInteractable(IInteractable interactable)
        {
            if (currentFocus != null)
            {
                if (currentFocus != interactable)
                {
                    currentFocus.HandleLoseFocus(playerManager);
                    interactable.HandleBecomeFocus(playerManager);

                    /*
                    OnFocusChangeEvent.BroadcastEvent(new OnFocusChangeEvent()
                    {
                        player = playerManager,
                        newFocus = interactable,
                        oldFocus = currentFocus
                    });
                    */

                    currentFocus = interactable;
                }
            }

            if (currentFocus == null)
            {
                currentFocus = interactable;

                /*
                OnFocusChangeEvent.BroadcastEvent(new OnFocusChangeEvent()
                {
                    player = playerManager,
                    newFocus = interactable,
                    oldFocus = currentFocus
                });
                */

                interactable.HandleBecomeFocus(playerManager);
            }

            _uiInteractionController.DisplayInteractable(currentFocus);
        }

        private void NotLookingAtInteractable()
        {
            if (currentFocus != null)
            {
                currentFocus.HandleLoseFocus(playerManager);
            }

            _uiInteractionController.HideInteractable();
            currentFocus = null;
        }

        private void NotLookingAtAnything()
        {
            if (currentFocus != null)
            {
                currentFocus.HandleLoseFocus(playerManager);
            }

            _uiInteractionController.HideInteractable();
            currentFocus = null;
        }
        #endregion


        #region InteractionStates
        private void DetermineInteractionStates()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (currentFocus != null)
                {
                    if (playerManager.inventoryController.HasActiveItem && playerManager.inventoryController.ActiveItem.Item.hasCustomInteractionFrame)
                    {
                        playerManager.gameObject.GetComponentInChildren<UseableItem>().HandleInteractionDown(InteractionInput.Primary);
                    }
                    else
                    {
                        Interact(currentFocus);
                    }
                }
                else
                {
                    if (playerManager.inventoryController.HasActiveItem && playerManager.inventoryController.ActiveItem.Item.hasCustomInteractionFrame)
                    {
                        playerManager.gameObject.GetComponentInChildren<UseableItem>().HandleInteractionDown(InteractionInput.Primary);
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (currentFocus != null)
                {
                     currentFocus.HandleInteraction(playerManager, (ItemContainer)playerManager.inventoryController.ActiveItem, InteractionInput.Secondary);
                }
            }
        }

        private void Interact(IInteractable interactable)
        {
            interactable.HandleInteraction(playerManager, (ItemContainer)playerManager.inventoryController.ActiveItem, InteractionInput.Primary);
        }

        public void TriggerCurrentFocus()
        {
            if (currentFocus == null)
            {
                return;
            }

            currentFocus.HandleInteraction(playerManager, (ItemContainer)playerManager.inventoryController.ActiveItem, InteractionInput.Primary);
        }
        #endregion
    }
}