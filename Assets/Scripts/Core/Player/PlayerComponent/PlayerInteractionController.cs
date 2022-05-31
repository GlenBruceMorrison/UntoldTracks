using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UntoldTracks.InventorySystem;
using UntoldTracks.UI;

namespace UntoldTracks.Player
{
    public interface IPlayerInteractionController
    {
        /// <summary>
        /// The current interactable (if any) that the player is looking at
        /// </summary>
        IInteractable CurrentFocus { get; }

        /// <summary>
        /// The Vector3 position that the player is looking at
        /// </summary>
        Vector3 LookingAtVector { get; }
        
        /// <summary>
        /// The GameObject that the player is currently looking at
        /// </summary>
        GameObject LookingAtGameObject { get; }

        /// <summary>
        /// An event that is fired whenever the player changes the interactable that they are looking at
        /// </summary>
        event FocusChange OnFocusChange;
        
        /// <summary>
        /// Trigger the interaction code on the current interaction that the player is looking at
        /// </summary>
        void TriggerCurrentFocus();
    }
    
    public delegate void FocusChange(IInteractable target);

    public class PlayerInteractionController : MonoBehaviour, IPlayerInteractionController, IPlayerComponent
    {
        #region Private
        private PlayerManager _playerManager;
        
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private float _interactionDistance = 5;

        private IInteractable _currentFocus;
        private Vector3 _lookingAtPosition;
        private GameObject _lookingAtGameObject;
        private FocusChange _onFocusChange;
        #endregion
        
        #region Events
        public event FocusChange OnFocusChange;
        #endregion
        
        #region Getters
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
        #endregion

        #region Life Cycle
        public void Init(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        private void Update()
        {
            if (_playerManager.FirstPersonController.IsPointerLocked())
            {
                return;
            }

            DetermineLookStates();

            DetermineInteractionStates();
        }
        #endregion

        #region Look States
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
                    _currentFocus.HandleLoseFocus(_playerManager);
                    interactable.HandleBecomeFocus(_playerManager);

                    _currentFocus = interactable;

                    OnFocusChange?.Invoke(_currentFocus);
                }
            }

            if (_currentFocus == null)
            {
                _currentFocus = interactable;

                interactable.HandleBecomeFocus(_playerManager);

                OnFocusChange?.Invoke(_currentFocus);
            }

            _playerManager.PlayerManagerUI.DisplayInteractable(_currentFocus);
        }

        private void NotLookingAtInteractable()
        {
            if (_currentFocus != null)
            {
                _currentFocus.HandleLoseFocus(_playerManager);
            }

            _playerManager.PlayerManagerUI.HideInteractable();
            _currentFocus = null;
        }

        private void NotLookingAtAnything()
        {
            if (_currentFocus != null)
            {
                _currentFocus.HandleLoseFocus(_playerManager);
            }

            _playerManager.PlayerManagerUI.HideInteractable();
            _currentFocus = null;
        }
        #endregion

        #region Interaction States
        private void DetermineInteractionStates()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // if we are looking at an interactable
                if (_currentFocus != null)
                {
                    // if we have an item selected in the hot bar
                    if (_playerManager.InventoryController.HasActiveItem)
                    {
                        // if that item handles interactions
                        if (_playerManager.InventoryController.ActiveItemContainer.Item.hasCustomInteractionFrame)
                        {
                            if (_playerManager.PlayerActiveItemController.TryGetTool(out EquipableEntity tool))
                            {
                                tool.HandleInteractionDown(InteractionInput.Primary);
                            }
                        }
                        // we will handle interactions
                        else
                        {
                            // if this item is not a placable item
                            if (!_playerManager.InventoryController.ActiveItemContainer.Item.isPlaceable)
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
                    if (_playerManager.InventoryController.HasActiveItem)
                    {
                        //if (_playerManager.InventoryController.ActiveItemContainer.Item.hasCustomInteractionFrame)
                        if (_playerManager.InventoryController.ActiveItemContainer.Item.isEquipable)
                        {
                            _playerManager.gameObject.GetComponentInChildren<EquipableEntity>()
                                .HandleInteractionDown(InteractionInput.Primary);
                        }
                        else if (_playerManager.PlaceableEntityController.IsPlacingSomething)
                        {
                            _playerManager.PlaceableEntityController.TryPlace();
                            return;
                        }
                    }
                }
            }

            else if (Input.GetMouseButtonUp(0))
            {
                if (_playerManager.InventoryController.ActiveItemContainer?.Item != null)
                {
                    if (_playerManager.InventoryController.ActiveItemContainer.Item.isEquipable)
                    {
                        _playerManager.gameObject.GetComponentInChildren<EquipableEntity>()
                                .HandleInteractionUp(InteractionInput.Primary);
                    }
                }
            }

            else if (Input.GetMouseButtonDown(1))
            {
                if (_currentFocus != null)
                {
                    if (!(_playerManager.InventoryController.HasActiveItem && _playerManager.InventoryController.ActiveItemContainer.Item.hasCustomInteractionFrame))
                    {
                        _currentFocus.HandleInput(_playerManager, InteractionInput.Secondary);
                    }
                }
            }
        }

        private void Interact(IInteractable interactable)
        {
            interactable.HandleInput(_playerManager, InteractionInput.Primary);
        }

        public void TriggerCurrentFocus()
        {
            if (_currentFocus == null)
            {
                return;
            }

            _currentFocus.HandleInput(_playerManager, InteractionInput.Primary);
        }
        #endregion
    }
}