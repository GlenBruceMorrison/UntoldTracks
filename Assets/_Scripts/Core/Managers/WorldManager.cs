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
    public class WorldManager : MonoBehaviour, ITokenizable
    {
        public List<Entity> _worldEntities = new();
        
        public void SpawnGeneratable()
        {
            GameManager.Instance.TrainManager.TrackGenerator.ExtendTrack(0);
            GameManager.Instance.TrainManager.TrackGenerator.ExtendTrack(0);

            //var generatable = ResourceService.Instance.Generatables[UnityEngine.Random.Range(0, 2)];
            var generatable = ResourceService.Instance.Generatables[1];

            var instance = Instantiate(generatable, GameManager.Instance.TrainManager.TrackGenerator.LastPoint, Quaternion.identity);

            instance.transform.position += instance.GetSpawnOffset;

            GameManager.Instance.TrainManager.TrackGenerator.ExtendTrack(instance.VertexPath);
        }

        private void Update()   
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                SpawnGeneratable();
            }
        }

        private void Start()
        {
            SpawnGeneratable();
        }

        #region Token
        public void Load(JSONNode node)
        {

        }

        public JSONObject Save()
        {
            return null;
        }
        #endregion
    }
}