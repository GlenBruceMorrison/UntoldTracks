using PathCreation;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UntoldTracks.Data;
using UntoldTracks.Models;

namespace UntoldTracks.Managers
{
    public class TrainManager : MonoBehaviour, ITokenizable
    {
        public Train train;
        public SerializableRegistry registry;
        public PlaceableEntityManager placeableEntityManager;

        public void Load(JSONNode node)
        {
            train.Load(node);
            placeableEntityManager.Load(node["placeableEntities"]);
        }

        public JSONObject Save()
        {
            var trainJSON = train.Save();
            trainJSON.Add("placeableEntities", placeableEntityManager.Save());

            return trainJSON;
        }
    }
}