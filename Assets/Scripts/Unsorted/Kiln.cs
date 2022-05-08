using System.Collections;
using System.Collections.Generic;
using Tracks.Inventory;
using UnityEngine;

namespace Tracks.Machines
{
    public class Kiln : MonoBehaviour
    {
        public int fuelLevel = 0;
        public Item currentFuelItem;
        public int maxFuelAtOnce = 5;

        public ItemContainer fuel;
    }
}
