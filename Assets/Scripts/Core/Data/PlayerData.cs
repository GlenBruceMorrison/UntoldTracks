using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UntoldTracks.Data
{
    [System.Serializable]
    public class PlayerData
    {
        public float posX, posY, posZ;
        public float rotX, rotY, rotZ;
        public InventoryData inventory;
    }
}
