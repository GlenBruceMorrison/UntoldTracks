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
            _trackGenerator.Load(node["track"]);

            _train.Load(node);
            _placeableEntityManager.Load(node["placeableEntities"]);

            _train.Initiate(_trackGenerator);

            GetComponent<RoadMeshCreator>().Init(_trackGenerator.VertexPath);
        }

        public JSONObject Save()
        {
            var trainJSON = _train.Save();

            trainJSON.Add("placeableEntities", _placeableEntityManager.Save());
            trainJSON.Add("track", _trackGenerator.Save());

            return trainJSON;
        }
        #endregion

        private void OnDrawGizmos()
        {

            Gizmos.DrawCube(_trackGenerator.LastPoint, Vector3.one);
        }
    }
}