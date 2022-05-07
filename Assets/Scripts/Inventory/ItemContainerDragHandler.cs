using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Tracks.Inventory
{
    [RequireComponent(typeof(PlayerInventoryController))]
    public class ItemContainerDragHandler : MonoBehaviour
    {
        private PlayerInventoryController _playerInventoryController;
        public ItemContainerUI selectedContainerUI;

        public ItemContainerUI GetFirstContainerUnderMouse()
        {
            var pointerData = new PointerEventData(EventSystem.current) { pointerId = -1 };
            pointerData.position = Input.mousePosition;
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (var result in results)
            {
                if (result.gameObject.TryGetComponent(out ItemContainerUI containerUI))
                {
                    if (selectedContainerUI.Container.IsEmpty())
                    {
                        if (containerUI == selectedContainerUI)
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

        public void HandleInventoryOpened()
        {
            selectedContainerUI.Container.Empty();
            selectedContainerUI.gameObject.SetActive(true);
        }

        public void HandleInventoryClosed()
        {
            if (!selectedContainerUI.Container.IsEmpty())
            {
                _playerInventoryController.PlayerInventory.FillAndReturnRemaining(selectedContainerUI.Container.Item, selectedContainerUI.Container.Count);
                selectedContainerUI.Container.Empty();
                selectedContainerUI.gameObject.SetActive(false);
            }
        }

        public void HandleHoldingContainer()
        {
            selectedContainerUI.transform.position = Input.mousePosition;
        }

        public void HandlePickUpContainer()
        {
            var containerUnderMouse = GetFirstContainerUnderMouse();
            if (containerUnderMouse != null)
            {
                if (!containerUnderMouse.Container.IsEmpty())
                {
                    var taken = containerUnderMouse.Container.TakeAll();
                    selectedContainerUI.Container.FillAndReturnRemaining(taken.Item, taken.Count);
                }
            }
        }

        public void HandleDropContainer()
        {
            var otherContainer = GetFirstContainerUnderMouse();

            // check if we are dropping on another container
            if (otherContainer != null)
            {
                // if the items are the same
                if (otherContainer.Container.HasItem(selectedContainerUI.Container.Item))
                {
                    var item = selectedContainerUI.Container.Item;

                    // fill container
                    var remainder = otherContainer.Container.FillAndReturnRemaining(selectedContainerUI.Container.Item, selectedContainerUI.Container.Count);

                    selectedContainerUI.Container.Empty();
                    selectedContainerUI.Container.FillAndReturnRemaining(item, remainder);
                }
                // if dropping into an empty container
                else if (otherContainer.Container.IsEmpty())
                {
                    // fill space
                    otherContainer.Container.FillAndReturnRemaining(selectedContainerUI.Container.Item, selectedContainerUI.Container.Count);
                    selectedContainerUI.Container.Empty();
                }
                // if items are different
                else if (!otherContainer.Container.HasItem(selectedContainerUI.Container.Item))
                {
                    //swap items
                    otherContainer.Container.Swap(selectedContainerUI.Container);
                }
            }
            else
            {
                //playerInventory.FillAndReturnRemaining(selectedContainerUI.Container.Item, selectedContainerUI.Container.Count);
            }
        }

        private void Update()
        {
            if (!_playerInventoryController.isOpen)
            {
                return;
            }

            //if (!selectedContainerUI.Container.IsEmpty())
            //{
                HandleHoldingContainer();
            //}

            if (Input.GetMouseButtonDown(0))
            {
                if (selectedContainerUI.Container.IsEmpty())
                {
                    HandlePickUpContainer();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (selectedContainerUI.Container.IsEmpty())
                {
                    return;
                }

                HandleDropContainer();
            }
        }
    }
}
