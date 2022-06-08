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

        private ItemContainer _container;

        public ItemContainer Container => _container;

        public void LinkToContainer(ItemContainer container)
        {
            _container = container;
            container.OnModified += HandleContainerModified;
            Render();
        }

        private void HandleContainerModified(ItemContainer newValue)
        {
            Render();
        }

        public void Render()
        {
            if (_container == null || _container.IsEmpty())
            {
                _imgItem.sprite = null;
                _imgItem.enabled = false;
                _txtCount.text = "";
                _txtDurability.text = "";
                return;
            }

            _imgItem.enabled = true;
            _imgItem.sprite = _container.Item.sprite;
            _txtCount.text = _container.Item.stackable ? _container.Count.ToString() : "";

            _txtDurability.text = _container.Item.degradable ? $"{_container.CurrentDurability} / {_container.Item.durability}" : "";
        }
    }
}
