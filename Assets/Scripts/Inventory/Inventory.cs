using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UntoldTracks.Inventory
{
    public class Inventory : IInventory
    {
        #region private
        private PlayerManager _accessedBy;
        private List<IItemContainer> _containers = new List<IItemContainer>();
        #endregion

        #region getters
        public bool IsOpen
        {
            get
            {
                return _accessedBy != null;
            }
        }

        public PlayerManager AccessedBy
        {
            get
            {
                return _accessedBy;
            }
        }

        public List<IItemContainer> Containers
        {
            get
            {
                return _containers;
            }
        }

        public int Size
        {
            get
            {
                return _containers.Count;
            }
        }
        #endregion

        #region events
        //public event PlayerInteraction OnOpened;
        //public event PlayerInteraction OnClosed;
        public event InventoryModified OnModified;
        #endregion
         
        public Inventory(int size)
        {
            SetSize(size);
        }

        public IItemContainer AppendContainer()
        {
            var container = new ItemContainer(this, _containers.Count);
            _containers.Add(container);
            return container;
        }

        public bool CanFill(Item item, int count)
        {
            var remainder = count;

            var containers = _containers.Where(x => x.HasItem(item) || x.IsEmpty()).ToList();

            if (containers.Count == 0)
            {
                return false;
            }

            foreach (var container in containers)
            {
                remainder = container.FillAndReturnRemaining(item, count, false);
            }

            return !(remainder > 0);
        }

        public int FillAndReturnRemaining(Item item, int count)
        {
            var remaining = count;

            _containers.Where(x => x.HasItem(item)).ToList().ForEach(x =>
            {
                if (remaining > 0)
                {
                    remaining = x.FillAndReturnRemaining(item, remaining);
                }
            });

            if (remaining > 0)
            {
                _containers.Where(x => x.IsEmpty()).ToList().ForEach(x =>
                {
                    if (remaining > 0)
                    {
                        remaining = x.FillAndReturnRemaining(item, remaining);
                    }
                });
            }

            OnModified?.Invoke();
            return remaining;
        }

        public int TakeAndReturnRemaining(Item item, int count)
        {
            var remaining = count;

            var toCheck = Containers.Where(x => x.HasItem(item)).Reverse().ToList();

            toCheck.ForEach(x =>
            {
                if (remaining > 0)
                {
                    remaining = x.TakeAndReturnRemaining(remaining);
                }
            });

            OnModified?.Invoke();
            return remaining;
        }

        public List<IItemContainer> GetNonEmptyContainer()
        {
            return _containers.Where(x => !x.IsEmpty()).ToList();
        }

        public bool HasItem(Item item, int count)
        {
            var remainder = count;

            var containers = _containers.Where(x => x.HasItem(item)).ToList();

            if (containers.Count == 0)
            {
                return false;
            }

            foreach (var container in GetNonEmptyContainer())
            {
                remainder = container.TakeAndReturnRemaining(count, false);
            }

            return !(remainder > 0);
        }

        /*
        public void Open(PlayerManager player)
        {
            if (IsOpen)
            {
                return;
            }

            _accessedBy = player;
            OnOpened?.Invoke(player);
        }
        */

        /*
        public void Close(PlayerManager player)
        {
            if (!IsOpen)
            {
                Debug.LogWarning("This inventory was closed, but it was not open in the first place");
                return;
            }

            _accessedBy = null;
            OnClosed?.Invoke(player);
        }
        */

        public IItemContainer GetContainerAtIndex(int index)
        {
            if (_containers == null || _containers.Count < 1)
            {
                return null;
            }

            if (index + 1 >= _containers.Capacity || index < 0)
            {
                Debug.LogWarning("Trying to acces container at non existing index");
                return null;
            }

            return Containers[index];
        }

        public void SetSize(int size)
        {
            _containers = new List<IItemContainer>();

            for (int i=0;i<size; i++)
            {
                AppendContainer();
            }
        }
    }
}