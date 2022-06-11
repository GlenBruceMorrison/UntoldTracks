using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UntoldTracks.InventorySystem;
using UntoldTracks.UI;
using UntoldTracks.Managers;

namespace UntoldTracks.Player
{
    public delegate void FocusChange(IInteractable target);

    public class PlayerInteractionController
    {
        public PlayerManager playerManager;
        private Camera _playerCamera;
        private float _interactionDistance = 5;

        private IInteractable _currentFocus;
        private Vector3 _lookingAtPosition;
        private GameObject _lookingAtGameObject;
        private FocusChange _onFocusChange;

        public event FocusChange OnFocusChange;

        public IInteractable CurrentFocus
        {
            get
            {
                return _currentFocus;
            }
        }

        public Vector3 LookingAtVector
        {
            get
            {
                return _lookingAtPosition;
            }
        }
        
        public GameObject LookingAtGameObject
        {
            get
            {
                return _lookingAtGameObject;
            }
        }

        public PlayerInteractionController(PlayerManager manager, Camera playerCamera)
        {
            this.playerManager = manager;
            this._playerCamera = playerCamera;
        }

        public void Init()
        {

        }

        public void Tick()
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
            Debug.DrawRay(_playerCamera.transform.position, _playerCamera.transform.forward * _interactionDistance, Color.red);

            // if hit any collider
            if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out RaycastHit hit2, _interactionDistance))
            {
                // set looking at values
                _lookingAtPosition = hit2.point;
                _lookingAtGameObject = hit2.collider.gameObject;
                
                // check if the hit is an interactable
                if (hit2.collider.gameObject.TryGetComponent(out IInteractable interactable))
                {
                    LookingAtInteractable(interactable);
                    return;
                }
                
                NotLookingAtInteractable();
                return;
            }
            
            _lookingAtPosition = Vector3.zero;
            _lookingAtGameObject = null;
            NotLookingAtAnything();
        }

        private void LookingAtInteractable(IInteractable interactable)
        {
            if (_currentFocus != null)
            {
                if (_currentFocus != interactable)
                {
                    _currentFocus.HandleLoseFocus(playerManager);
                    interactable.HandleBecomeFocus(playerManager);

                    _currentFocus = interactable;

                    OnFocusChange?.Invoke(_currentFocus);
                }
            }

            if (_currentFocus == null)
            {
                _currentFocus = interactable;

                interactable.HandleBecomeFocus(playerManager);

                OnFocusChange?.Invoke(_currentFocus);
            }

            //OnFocusChange?.Invoke(null);
        }

        private void NotLookingAtInteractable()
        {
            if (_currentFocus != null)
            {
                _currentFocus.HandleLoseFocus(playerManager);
                OnFocusChange?.Invoke(null);
            }

            _currentFocus = null;
            //OnFocusChange?.Invoke(null);
        }

        private void NotLookingAtAnything()
        {
            if (_currentFocus != null)
            {
                _currentFocus.HandleLoseFocus(playerManager);
                OnFocusChange?.Invoke(null);
            }

            //OnFocusChange?.Invoke(null);
            _currentFocus = null;
        }

        private void DetermineInteractionStates()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // if we are looking at an interactable
                if (_currentFocus != null)
                {
                    // if we have an item selected in the hot bar
                    if (playerManager.InventoryController.HasActiveItem)
                    {
                        // if that item handles interactions
                        if (playerManager.InventoryController.ActiveItemContainer.Item.hasCustomInteractionFrame)
                        {
                            if (playerManager.PlayerActiveItemController.TryGetTool(out EquipableEntity tool))
                            {
                                tool.HandleInteractionDown(InteractionInput.Primary);
                            }
                        }
                        // we will handle interactions
                        else
                        {
                            // if this item is not a placable item
                            if (!playerManager.InventoryController.ActiveItemContainer.Item.isPlaceable)
                            {
                                // perform standard interaction with the primary input
                                Interact(_currentFocus);
                            }
                        }
                    }
                    else
                    {
                        // perform standard interaction with the primary input
                        Interact(_currentFocus);
                    }

                    return;
                }
                else
                {
                    if (playerManager.InventoryController.HasActiveItem)
                    {
                        //if (playerManager.InventoryController.ActiveItemContainer.Item.hasCustomInteractionFrame)
                        if (playerManager.InventoryController.ActiveItemContainer.Item.isEquipable)
                        {
                            playerManager.gameObject.GetComponentInChildren<EquipableEntity>()
                                .HandleInteractionDown(InteractionInput.Primary);
                        }
                        else if (playerManager.PlaceableEntityController.IsPlacingSomething)
                        {
                            playerManager.PlaceableEntityController.TryPlace();
                            return;
                        }
                    }
                }
            }

            else if (Input.GetMouseButtonUp(0))
            {
                if (playerManager.InventoryController.ActiveItemContainer?.Item != null)
                {
                    if (playerManager.InventoryController.ActiveItemContainer.Item.isEquipable)
                    {
                        playerManager.gameObject.GetComponentInChildren<EquipableEntity>()
                                .HandleInteractionUp(InteractionInput.Primary);
                    }
                }
            }

            else if (Input.GetMouseButtonDown(1))
            {
                if (_currentFocus != null)
                {
                    if (!(playerManager.InventoryController.HasActiveItem && playerManager.InventoryController.ActiveItemContainer.Item.hasCustomInteractionFrame))
                    {
                        _currentFocus.HandleInput(playerManager, InteractionInput.Secondary);
                    }
                }

                if (playerManager.InventoryController.ActiveItemContainer?.Item != null)
                {
                    if (playerManager.InventoryController.ActiveItemContainer.Item.isEquipable)
                    {
                        playerManager.gameObject.GetComponentInChildren<EquipableEntity>()
                                .HandleInteractionDown(InteractionInput.Secondary);
                    }
                }
            }

            else if (Input.GetKeyDown(KeyCode.E))
            {
                if (_currentFocus != null)
                {
                    _currentFocus.HandleInput(playerManager, InteractionInput.Action1);
                }
            }
        }

        private void Interact(IInteractable interactable)
        {
            interactable.HandleInput(playerManager, InteractionInput.Primary);
        }

        public void TriggerCurrentFocus()
        {
            if (_currentFocus == null)
            {
                return;
            }

            _currentFocus.HandleInput(playerManager, InteractionInput.Primary);
        }
    }
}