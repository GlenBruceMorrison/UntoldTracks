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
        public TrackGenerator trackGenerator;

        #region Token
        public void Load(JSONNode node)
        {
            train.Load(node);
            placeableEntityManager.Load(node["placeableEntities"]);

            train.Initiate(trackGenerator);
            trackGenerator.Load(node["track"]);
        }

        public JSONObject Save()
        {
            var trainJSON = train.Save();

            trainJSON.Add("placeableEntities", placeableEntityManager.Save());
            trainJSON.Add("track", trackGenerator.Save());

            return trainJSON;
        }
        #endregion


        private void Update()
        {
            if (Input.GetKeyDown("l"))
            {
                foreach (var entity in placeableEntityManager.entities)
                {
                    entity.AfterBuild();
                }
            }
        }
    }
}