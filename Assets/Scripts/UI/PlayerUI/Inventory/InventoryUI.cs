using UnityEngine;
using System.Collections.Generic;
using UntoldTracks.InventorySystem;

namespace UntoldTracks.UI
{
    public class InventoryUI : MonoBehaviour
    {
        private Inventory _inventory;
        private int _startIndex = -1;
        private int _endIndex = -1;

        protected ItemContainerUI[] _uiContainers;

        [SerializeField] private ItemContainerUI _uiContainerPrefab;
        [SerializeField] private Transform _containerRoot;

        public virtual void LinkToInventory(Inventory inventory, int startIndex=-1, int endIndex=-1)
        {
            _inventory = inventory;
            _startIndex = startIndex;
            _endIndex = endIndex;

            _uiContainers = _containerRoot.GetComponentsInChildren<ItemContainerUI>();

            Render();

            inventory.OnModified += Render;
        }

        public void Render()
        {
            var start = _startIndex == -1 ? 0 : _startIndex;
            var end = _endIndex == -1 ? _inventory.Size : _endIndex;

            for (int i = start; i < end; i++)
            {
                _uiContainers[i- start].LinkToContainer(_inventory.Containers[i]);
            }
        }
    }
}