using PathCreation;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UntoldTracks.Models;

namespace UntoldTracks.Managers
{
    public class TrainManager : MonoBehaviour, ITokenizable
    {
        [SerializeField] private Train _train;
        [SerializeField] private PlaceableEntityManager _placeableEntityManager;
        [SerializeField] private TrackGenerator _trackGenerator;

        public TrackGenerator TrackGenerator
        {
            get
            {
                if (_trackGenerator == null)
                {
                    _trackGenerator = GetComponent<TrackGenerator>();
                }

                return _trackGenerator;
            }
        }

        public PlaceableEntityManager PlaceableEntityManager
        {
            get
            {
                return _placeableEntityManager;
            }
        }

        public Train Train
        {
            get
            { 
                return _train;
            }
        }

        #region Token
        public void Load(JSONNode node)
        {
            TrackGenerator.Load(node["track"]);

            _train.Init(TrackGenerator);

            _train.Load(node);
            _placeableEntityManager.Load(node["placeableEntities"]);

            GetComponentInChildren<RoadMeshCreator>().Init(TrackGenerator.VertexPath);
        }

        public JSONObject Save()
        {
            var trainJSON = _train.Save();

            trainJSON.Add("placeableEntities", _placeableEntityManager.Save());
            trainJSON.Add("track", _trackGenerator.Save());

            return trainJSON;
        }
        #endregion
    }
}