using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace UntoldTracks.Inventory
{
    [RequireComponent(typeof(PlayerInventoryController))]
    public class ItemContainerDragHandler : MonoBehaviour
    {
        private PlayerInventoryController _playerInventoryController;

        [SerializeField]
        private ItemContainerUI _selectedContainerUI;

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

        private void HandleInventoryOpened()
        {
            _selectedContainerUI.Container.Empty();
            _selectedContainerUI.gameObject.SetActive(true);
        }

        private void HandleInventoryClosed()
        {
            if (!_selectedContainerUI.Container.IsEmpty())
            {
                _playerInventoryController.Inventory.FillAndReturnRemaining(_selectedContainerUI.Container.Item, _selectedContainerUI.Container.Count);
                _selectedContainerUI.Container.Empty();
                _selectedContainerUI.gameObject.SetActive(false);
            }
        }

        private void HandleHoldingContainer()
        {
            _selectedContainerUI.transform.position = Input.mousePosition;
            //Debug.Log(_selectedContainerUI.Container?.Item?.name);
        }

        private void HandlePickUpContainer()
        {
            var containerUnderMouse = GetFirstContainerUnderMouse();
            if (containerUnderMouse != null)
            {
                if (!containerUnderMouse.Container.IsEmpty())
                {
                    var taken = containerUnderMouse.Container.TakeAll();
                    _selectedContainerUI.Container.FillAndReturnRemaining(taken.Item, taken.Count);

                    Debug.Log($"{_selectedContainerUI.Container.Item.name} & {_selectedContainerUI.Container.Count}");
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
                    var item = _selectedContainerUI.Container.Item;

                    // fill container
                    var remainder = otherContainer.Container.FillAndReturnRemaining(_selectedContainerUI.Container.Item, _selectedContainerUI.Container.Count);

                    _selectedContainerUI.Container.Empty();
                    _selectedContainerUI.Container.FillAndReturnRemaining(item, remainder);
                }
                // if dropping into an empty container
                else if (otherContainer.Container.IsEmpty())
                {
                    // fill space
                    otherContainer.Container.FillAndReturnRemaining(_selectedContainerUI.Container.Item, _selectedContainerUI.Container.Count);
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
    }
}
