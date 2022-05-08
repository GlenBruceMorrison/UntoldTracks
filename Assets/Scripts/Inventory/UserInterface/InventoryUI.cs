using UnityEngine;
using System.Collections.Generic;

namespace UntoldTracks.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        private IInventory _inventory;
        private List<ItemContainerUI> _uiContainers = new List<ItemContainerUI>();
        private int _startIndex = -1;
        private int _endIndex = -1;

        [SerializeField]
        private ItemContainerUI _uiContainerPrefab;

        [SerializeField]
        private Transform _containerRoot;

        public void LinkToInventory(IInventory inventory, int startIndex=-1, int endIndex=-1)
        {
            _inventory = inventory;

            _startIndex = startIndex;
            _endIndex = endIndex;

            Render();
        }

        public void Render()
        {
            foreach (var uiContainer in _uiContainers)
            {
                Destroy(uiContainer.gameObject);
            }

            _uiContainers = new List<ItemContainerUI>();

            var start = _startIndex == -1 ? 0 : _startIndex;
            var end = _endIndex == -1 ? _inventory.Size : _endIndex;

            for (int i = start; i < end; i++)
            {
                var container = Instantiate(_uiContainerPrefab, _containerRoot.transform);
                _uiContainers.Add(container);
                container.LinkContainer(_inventory.Containers[i]);
                container.Render();
            }
        }

        public void RenderContainers()
        {
            foreach (var container in _uiContainers)
            {
                container.Render();
            }
        }
    }
}