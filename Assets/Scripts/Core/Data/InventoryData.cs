using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UntoldTracks.Data
{
    [System.Serializable]
    public class InventoryData
    {
        public int size;
        [NonReorderable] public List<ItemContainerData> items = new();
    }
}
