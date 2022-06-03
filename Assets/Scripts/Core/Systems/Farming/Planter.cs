using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks;
using UntoldTracks.Player;
using UntoldTracks.Models;

public class Planter : MonoBehaviour
{
    private PlanterSpot[] _planters;
    public List<PlantModel> possiblePlants = new List<PlantModel>();

    private void Awake()
    {
        _planters = GetComponentsInChildren<PlanterSpot>();

        foreach(var planterSpot in _planters)
        {
            planterSpot.possiblePlants = possiblePlants;
        }
    }

    public void LoadFromJson(string json)
    {

    }

    public string ToJson()
    {
        return "";
    }
}
