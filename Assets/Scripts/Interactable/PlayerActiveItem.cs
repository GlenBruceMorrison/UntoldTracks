using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks.Inventory;

namespace UntoldTracks.Player
{
    public class PlayerActiveItem : MonoBehaviour
    {
        public PlayerManager playerMananger => GameObject.FindObjectOfType<PlayerManager>();
        public Item ActiveItem => playerMananger.inventoryController.ActiveItem?.Item;

        public GameObject activeItemObject;

        private void OnEnable()
        {
            playerMananger.inventoryController.OnActiveItemChanged += HandleActiveItemChanged;
        }

        private void OnDisable()
        {
            playerMananger.inventoryController.OnActiveItemChanged -= HandleActiveItemChanged;
        }

        public void EmptyHand()
        {
            Destroy(activeItemObject);
        }

        private void HandleActiveItemChanged(PlayerManager player, IItemContainer container)
        {
            EmptyHand();

            if (container.IsEmpty())
            {
                return;
            }

            if (container.Item.toolPrefab != null)
            {
                SwitchItem(container.Item.toolPrefab);
            }
        }

        public void SwitchItem(GameObject prefab)
        {
            activeItemObject = Instantiate(prefab, this.transform);
            activeItemObject.transform.localPosition = Vector3.zero;
            activeItemObject.transform.localEulerAngles = Vector3.zero;
        }
    }
}
