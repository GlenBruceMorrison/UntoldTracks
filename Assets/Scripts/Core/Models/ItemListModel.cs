using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks.Models;

namespace UntoldTracks.Models
{
    [CreateAssetMenu(fileName = "Item", menuName = "Data/ItemList")]
    public class ItemListModel : SerializableScriptableObject
    {
        public List<ItemModel> items = new List<ItemModel>();
    }
}