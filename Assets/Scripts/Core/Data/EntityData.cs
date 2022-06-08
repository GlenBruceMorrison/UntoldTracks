using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UntoldTracks.Data
{
    [System.Serializable]
    public class EntityData
    {
        public VectorData position;
        public VectorData rotation;

        public static EntityData FromTransform(Transform transform)
        {
            return new EntityData()
            {
                position = VectorData.FromVector(transform.position),
                rotation = VectorData.FromVector(transform.localEulerAngles)
            };
        }
    }
}
