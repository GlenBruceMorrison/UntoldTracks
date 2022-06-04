using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks.Models;
using UntoldTracks;

namespace UntoldTracks.Models
{
    [CreateAssetMenu(fileName = "Carriage", menuName = "Data/Carriage")]
    public class CarriageModel : SerializableScriptableObject
    {
        public Carriage view;
    }
}
