using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UntoldTracks.Data
{
    [System.Serializable]
    public class ItemContainerData
    {
        public string itemGUID;
        public int amount;
        public int durability;
        public int inventoryIndex=-1;
    }
}
