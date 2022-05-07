using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Tracks.Inventory
{
    [System.Serializable]
    public class ItemContainerTemplate
    {
        public Item item;
        public int count;
    }

    public class PlayerInventoryController : MonoBehaviour
    {
        internal PlayerManager playerManager;

        public PlayerManager playerMananger;

        public PlayerInventory PlayerInventory { get; private set; }

        public bool isOpen;

        public UnityEvent OnClose, OnOpen;

        [SerializeField]
        private List<ItemContainerTemplate> _starterTemplate = new List<ItemContainerTemplate>();

        public PlayerInventoryUI _ui;
        public PlayerInventoryBarUI _uiInventoryBar;

        public void AddFromTempalte()
        {
            foreach (var containerTemplate in _starterTemplate)
            {
                if (containerTemplate.item == null || containerTemplate.count < 1)
                {
                    continue;
                }

                PlayerInventory.FillAndReturnRemaining(containerTemplate.item, containerTemplate.count);
            }
        }

        public void Awake()
        {
            PlayerInventory = new PlayerInventory(25);
            _ui.Initiate(PlayerInventory);

            if (_starterTemplate != null)
            {
                AddFromTempalte();
            }

            _uiInventoryBar.LinkToInventory(PlayerInventory, 0, 9);
            _ui.LinkToInventory(PlayerInventory, 9, 25);
        }

        private void Start()
        {
            Close();
        }

        public void OpenOtherInventory(IInventory inventory)
        {
            Open();
            _ui.Open(inventory);
        }

        public void Close()
        {
            playerManager.FirstPersonController.Look.LockPointer();
            _ui.gameObject.SetActive(false);
            _ui.Close();
            isOpen = false;
            OnClose?.Invoke();
        }

        public void Open()
        {
            playerManager.FirstPersonController.Look.UnlockPointer();
            _ui.gameObject.SetActive(true);
            _ui.Open();
            isOpen = true;
            OnOpen?.Invoke();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (isOpen)
                {
                    Close();
                }
                else
                {
                    Open();
                }
            }
        }
    }
}