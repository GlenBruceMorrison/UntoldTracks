using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UntoldTracks.InventorySystem;

namespace UntoldTracks.UI
{
    public class ItemContainerUI : MonoBehaviour
    {
        [SerializeField] private Image _imgItem;
        [SerializeField] private TMP_Text _txtCount;
        [SerializeField] private TMP_Text _txtDurability;

        public ItemContainer Container { get; private set; }

        private void Awake()
        {
            if (Container == null)
            {
                Container = new ItemContainer(inventory:null, 0);
            }

            Render();
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

        private void HandleContainerModified(ItemContainer newValue)
        {
            Render();
        }

        public void Render()
        {
            if (Container == null || Container.IsEmpty())
            {
                _imgItem.sprite = null;
                _imgItem.enabled = false;
                _txtCount.text = "";
                _txtDurability.text = "";
                return;
            }

            _imgItem.enabled = true;
            _imgItem.sprite = Container.Item.sprite;
            _txtCount.text = Container.Item.stackable ? Container.Count.ToString() : "";

            _txtDurability.text = Container.Item.degradable ? $"{Container.CurrentDurability} / {Container.Item.durability}" : "";
        }

        public void LinkContainer(ItemContainer container)
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
