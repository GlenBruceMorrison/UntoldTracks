﻿using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Tracks.Inventory
{
    public class ItemContainerUI : MonoBehaviour
    {
        public Image imgItem;
        public TMP_Text txtCount;

        public IItemContainer Container { get; private set; }

        private void Awake()
        {
            if (Container == null)
            {
                Container = new ItemContainer(null, -0);
            }
        }

        private void OnEnable()
        {
            if (Container == null)
            {
                return;
            }

            Container.OnModified += HandleContainerModified;
        }

        private void OnDisable()
        {
            if (Container == null)
            {
                return;
            }

            Container.OnModified -= HandleContainerModified;
        }

        public void HandleContainerModified(IItemContainer oldValue, IItemContainer newValue)
        {
            Render();
        }

        public void Render()
        {
            if (Container == null || Container.IsEmpty())
            {
                imgItem.sprite = null;
                imgItem.enabled = false;
                txtCount.text = "";
                return;
            }

            imgItem.enabled = true;
            imgItem.sprite = Container.Item.sprite;
            txtCount.text = Container.Item.stackable ? Container.Count.ToString() : "";
        }

        public void LinkContainer(IItemContainer container)
        {
            if (Container != null)
            {
                Container.OnModified -= HandleContainerModified;
            }

            Container = container;
            Container.OnModified += HandleContainerModified;

            Render();
        }
    }
}