using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks.Models;
using UntoldTracks;

namespace UntoldTracks.Models
{
    [CreateAssetMenu(fileName = "StorageMedium", menuName = "Data/StorageMedium")]
    public class StorageMediumModel : SerializableScriptableObject
    {
        public StorageContainer view;
    }
}
