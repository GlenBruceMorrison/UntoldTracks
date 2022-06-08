using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UntoldTracks.Data
{
    [System.Serializable]
    public class VectorData
    {
        public float x, y, z;

        public Vector3 Vector
        {
            get
            {
                return new Vector3(x, y, z);
            }
        }

        public static VectorData FromVector(Vector3 vector)
        {
            return new VectorData()
            {
                x = vector.x,
                y = vector.y,
                z = vector.z
            };
        }
    }
}