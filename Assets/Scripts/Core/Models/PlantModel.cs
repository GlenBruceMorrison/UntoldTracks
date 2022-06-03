using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks.Models;

namespace UntoldTracks.Models
{
    [CreateAssetMenu(fileName = "Plant", menuName = "Data/Plant")]
    public class PlantModel : ScriptableObject
    {
        public ItemModel seed;
        public Plant plant;
    }

    public class PlanterSO : ScriptableObject
    {
        public List<PlantModel> possiblePlants = new List<PlantModel>();
    }
}