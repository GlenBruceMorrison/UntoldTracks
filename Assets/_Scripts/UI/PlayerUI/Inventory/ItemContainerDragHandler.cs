using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UntoldTracks.UI;
using UntoldTracks.Managers;

namespace UntoldTracks.InventorySystem
{
    public class ItemContainerDragHandler : MonoBehaviour
    {
        private PlayerInventoryController _playerInventoryController;

        [SerializeField]
        private ItemContainerUI _selectedContainerUI;

        #region Unity
        public void Init(PlayerInventoryController playerInventoryController)
        {
            _playerInventoryController = playerInventoryController;
            _selectedContainerUI.LinkToContainer(new ItemContainer(null, 0, 0));
        }

        private void OnDisable()
        {
            //_playerInventoryController.OnClose-=(HandleInventoryClosed);
            //_playerInventoryController.OnOpen-=(HandleInventoryOpened);
        }

        public void Tick()
        {
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

        public void HandleInventoryOpened()
        {
            _selectedContainerUI.Container.Empty();
            _selectedContainerUI.gameObject.SetActive(true);
        }

        public void HandleInventoryClosed()
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

            if (otherContainer != null)
            {
                otherContainer.Container.DropContainerOnUs(_selectedContainerUI.Container);
            }
        }
    }
}
