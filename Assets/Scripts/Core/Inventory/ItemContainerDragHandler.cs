using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UntoldTracks.UI;

namespace UntoldTracks.Inventory
{
    [RequireComponent(typeof(PlayerInventoryController))]
    public class ItemContainerDragHandler : MonoBehaviour
    {
        private PlayerInventoryController _playerInventoryController;

        [SerializeField]
        private ItemContainerUI _selectedContainerUI;

        #region Unity
        private void Awake()
        {
            _playerInventoryController = GetComponent<PlayerInventoryController>();
        }

        private void OnEnable()
        {
            _playerInventoryController.OnClose.AddListener(HandleInventoryClosed);
            _playerInventoryController.OnOpen.AddListener(HandleInventoryOpened);
        }

        private void OnDisable()
        {
            _playerInventoryController.OnClose.RemoveListener(HandleInventoryClosed);
            _playerInventoryController.OnOpen.RemoveListener(HandleInventoryOpened);
        }

        private void Update()
        {
            if (!_playerInventoryController.IsOpen)
            {
                return;
            }

            HandleHoldingContainer();

            if (Input.GetMouseButtonDown(0))
            {
                if (_selectedContainerUI.Container.IsEmpty())
                {
                    HandlePickUpContainer();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (_selectedContainerUI.Container.IsEmpty())
                {
                    return;
                }

                HandleDropContainer();
            }
        }
        #endregion

        private ItemContainerUI GetFirstContainerUnderMouse()
        {
            var pointerData = new PointerEventData(EventSystem.current) { pointerId = -1 };
            pointerData.position = Input.mousePosition;
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (var result in results)
            {
                if (result.gameObject.TryGetComponent(out ItemContainerUI containerUI))
                {
                    if (_selectedContainerUI.Container.IsEmpty())
                    {
                        if (containerUI == _selectedContainerUI)
                        {
                            continue;
                        }
                    }

                    return containerUI;
                }
            }

            return null;
        }

        private void HandleInventoryOpened()
        {
            _selectedContainerUI.Container.Empty();
            _selectedContainerUI.gameObject.SetActive(true);
        }

        private void HandleInventoryClosed()
        {
            if (!_selectedContainerUI.Container.IsEmpty())
            {
                _playerInventoryController.Inventory.Give(_selectedContainerUI.Container);
                _selectedContainerUI.Container.Empty();
                _selectedContainerUI.gameObject.SetActive(false);
            }
        }

        private void HandleHoldingContainer()
        {
            _selectedContainerUI.transform.position = Input.mousePosition;
        }

        private void HandlePickUpContainer()
        {
            var containerUnderMouse = GetFirstContainerUnderMouse();
            if (containerUnderMouse != null)
            {
                if (!containerUnderMouse.Container.IsEmpty())
                {
                    var container = containerUnderMouse.Container.TakeAll();
                    _selectedContainerUI.Container.Give(container);
                }
            }
        }

        private void HandleDropContainer()
        {
            var otherContainer = GetFirstContainerUnderMouse();

            // check if we are dropping on another container
            if (otherContainer != null)
            {
                // if the items are the same
                if (otherContainer.Container.HasItem(_selectedContainerUI.Container.Item))
                {
                    // fill other container with what we can
                    var queryResult = otherContainer.Container.Give(_selectedContainerUI.Container);

                    _selectedContainerUI.Container.Empty();

                    // if not everything was able to be added, give the remaining back to this container
                    var amountNotAdded = queryResult.amountAdded - _selectedContainerUI.Container.Count;
                    if (amountNotAdded > 0)
                    {
                        _selectedContainerUI.Container.Give(new ItemContainer(_selectedContainerUI.Container.Item, queryResult.amountAdded));
                    }
                }
                // if dropping into an empty container
                else if (otherContainer.Container.IsEmpty())
                {
                    // fill space
                    otherContainer.Container.Give(_selectedContainerUI.Container);
                    _selectedContainerUI.Container.Empty();
                }
                // if items are different
                else if (!otherContainer.Container.HasItem(_selectedContainerUI.Container.Item))
                {
                    //swap items
                    otherContainer.Container.Swap(_selectedContainerUI.Container);
                }
            }
            else
            {
                //playerInventory.FillAndReturnRemaining(selectedContainerUI.Container.Item, selectedContainerUI.Container.Count);
            }
        }
    }
}
