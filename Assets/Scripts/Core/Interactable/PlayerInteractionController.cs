using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UntoldTracks.InventorySystem;
using UntoldTracks.UI;

namespace UntoldTracks.Player
{
    public delegate void FocusChange(IInteractable target);

    public class PlayerInteractionController : MonoBehaviour
    {
        public PlayerManager playerManager;

        public Camera playerCamera;

        [SerializeField]
        private PlayerInteractionControllerUI _uiInteractionController;

        public float interactionDistance = 5;

        public IInteractable currentFocus;

        [SerializeField]
        private Vector3 _lookingAt;

        public event FocusChange OnFocusChange;

        public Vector3 LookingAt
        {
            get
            {
                return _lookingAt;
            }
        }

        public void Init(PlayerManager playerManager)
        {
            
        }

        private void Update()
        {
            if (playerManager.firstPersonController.IsPointerLocked())
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
                _lookingAt = hit2.point;
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

                    currentFocus = interactable;

                    OnFocusChange?.Invoke(currentFocus);
                }
            }

            if (currentFocus == null)
            {
                currentFocus = interactable;

                interactable.HandleBecomeFocus(playerManager);

                OnFocusChange?.Invoke(currentFocus);
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
                        if (playerManager.playerActiveItem.TryGetTool(out ToolEntity tool))
                        {
                            tool.HandleInteractionDown(InteractionInput.Primary);
                        }
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
                        playerManager.gameObject.GetComponentInChildren<ToolEntity>().HandleInteractionDown(InteractionInput.Primary);
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (currentFocus != null)
                {
                     currentFocus.HandleSecondaryInput(playerManager, (ItemContainer)playerManager.inventoryController.ActiveItem);
                }
            }
        }

        private void Interact(IInteractable interactable)
        {
            interactable.HandlePrimaryInput(playerManager, (ItemContainer)playerManager.inventoryController.ActiveItem);
        }

        public void TriggerCurrentFocus()
        {
            if (currentFocus == null)
            {
                return;
            }

            currentFocus.HandlePrimaryInput(playerManager, (ItemContainer)playerManager.inventoryController.ActiveItem);
        }
        #endregion
    }
}