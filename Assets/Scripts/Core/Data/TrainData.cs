using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UntoldTracks.Models;

namespace UntoldTracks.Data
{
    [System.Serializable]
    public class TrainData
    {
        public float distanceTravelled;
        public float currentSpeed;
        public bool isMoving;

        public List<CarriageData> carriages = new();
    }
}
