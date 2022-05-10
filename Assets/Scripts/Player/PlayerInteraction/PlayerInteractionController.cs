using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UntoldTracks.Player
{
    public class PlayerInteractionController : MonoBehaviour
    {
        public PlayerManager playerManager;

        public Camera playerCamera;

        [SerializeField]
        private PlayerInteractionControllerUI _uiInteractionController;

        public float interactionDistance = 300f;

        public IInteractable currentFocus;



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

        private void Update()
        {
            if (playerManager.FirstPersonController.IsPointerLocked())
            {
                return;
            }

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

            if (Input.GetMouseButtonDown(0))
            {
                if (currentFocus != null)
                {
                    currentFocus.HandleInteraction(playerManager);
                }
            }
        }

        private void Interact(IInteractable interactable)
        {
            interactable.HandleInteraction(playerManager);
        }
    }

}