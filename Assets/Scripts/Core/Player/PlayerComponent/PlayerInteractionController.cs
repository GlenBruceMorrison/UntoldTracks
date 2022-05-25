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

    public class PlayerInteractionController : PlayerComponent, IPlayerInteractionController
    {
        #region Private
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
        
        #region Player Component
        protected override void Initiate()
        {
            
        }

        protected override void Run(float deltaTime)
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

            if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out RaycastHit hit2, _interactionDistance))
            {
                _lookingAtPosition = hit2.point;
                _lookingAtGameObject = hit2.collider.gameObject;
                
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
                if (_currentFocus != null)
                {
                    if (_playerManager.InventoryController.HasActiveItem)
                    {
                        if (_playerManager.InventoryController.ActiveItem.Item.hasCustomInteractionFrame)
                        {
                            if (_playerManager.PlayerActiveItemController.TryGetTool(out ToolEntity tool))
                            {
                                tool.HandleInteractionDown(InteractionInput.Primary);
                            }
                        }
                        else
                        {
                            if (!_playerManager.InventoryController.ActiveItem.Item.isPlaceable)
                            {
                                Interact(_currentFocus);
                            }
                        }
                    }
                    else
                    {
                        Interact(_currentFocus);
                    }
                }
                else
                {
                    if (_playerManager.InventoryController.HasActiveItem)
                    {
                        if (_playerManager.InventoryController.ActiveItem.Item.hasCustomInteractionFrame)
                        {
                            _playerManager.gameObject.GetComponentInChildren<ToolEntity>()
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

            if (Input.GetMouseButtonDown(1))
            {
                if (_currentFocus != null)
                {
                    if (!(_playerManager.InventoryController.HasActiveItem && _playerManager.InventoryController.ActiveItem.Item.hasCustomInteractionFrame))
                    {
                        _currentFocus.HandleSecondaryInput(_playerManager,
                            (ItemContainer)_playerManager.InventoryController.ActiveItem);
                    }
                }
            }
        }

        private void Interact(IInteractable interactable)
        {
            interactable.HandlePrimaryInput(_playerManager, (ItemContainer)_playerManager.InventoryController.ActiveItem);
        }

        public void TriggerCurrentFocus()
        {
            if (_currentFocus == null)
            {
                return;
            }

            _currentFocus.HandlePrimaryInput(_playerManager, (ItemContainer)_playerManager.InventoryController.ActiveItem);
        }
        #endregion
    }
}