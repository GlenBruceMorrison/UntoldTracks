using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks.Inventory;

namespace UntoldTracks.UI
{
    public class PlayerInventoryBarUI : InventoryUI
    {
        [SerializeField]
        private Transform _selector;

        public void SetActiveIndex(int activeItemIndex)
        {
            _selector.transform.SetParent(_uiContainers[activeItemIndex].transform);
            _selector.transform.localPosition = Vector3.zero;
        }

        public override void LinkToInventory(IInventory inventory, int startIndex = -1, int endIndex = -1)
        {
            base.LinkToInventory(inventory, startIndex, endIndex);

            _selector.transform.SetParent(_uiContainers[0].transform);
            _selector.transform.localPosition = Vector3.zero;
        }
    }
}
