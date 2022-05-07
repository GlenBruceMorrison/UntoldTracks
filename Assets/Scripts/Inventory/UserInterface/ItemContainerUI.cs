using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Tracks.Inventory
{
    public class ItemContainerUI : MonoBehaviour
    {
        [SerializeField]
        private Image _imgItem;

        [SerializeField]
        private TMP_Text _txtCount;

        public IItemContainer Container { get; private set; }

        private void Awake()
        {
            if (Container == null)
            {
                Container = new ItemContainer(null, -0);
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

        private void HandleContainerModified(IItemContainer oldValue, IItemContainer newValue)
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
                return;
            }

            _imgItem.enabled = true;
            _imgItem.sprite = Container.Item.sprite;
            _txtCount.text = Container.Item.stackable ? Container.Count.ToString() : "";
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
