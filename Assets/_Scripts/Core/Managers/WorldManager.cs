using PathCreation;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.Rendering;
using UnityEngine;
using UntoldTracks.Models;

namespace UntoldTracks.Managers
{
    public class WorldManager : MonoBehaviour, ITokenizable
    {
        public List<Entity> _worldEntities = new();

        public Vector3 one, two, three, four;
        
        public void SpawnGeneratable()
        {
            // add a straight track at end of main line
            GameManager.Instance.TrainManager.TrackGenerator.ExtendTrack(new Vector3(0, 0, 100), BezierPath.ControlMode.Automatic);
            GameManager.Instance.TrainManager.TrackGenerator.ExtendTrack(new Vector3(0, 0, 100), BezierPath.ControlMode.Free);

            var instance = SpawnGeneratable(
                GameManager.Instance.TrainManager.TrackGenerator.LastTrackPositionInWorldSpace,
                Vector3.forward * -100);
            
            GameManager.Instance.TrainManager.TrackGenerator.ExtendTrack(instance.VertexPath);
        }

        public GeneratableBase SpawnGeneratable(Vector3 attatchPoint, Vector3 trackExtension)
        {
            var generatable = ResourceService.Instance.Generatables[UnityEngine.Random.Range(0, ResourceService.Instance.Generatables.Count)];
            var instance = Instantiate(generatable, attatchPoint, Quaternion.identity);
            
            instance.AddTrackToStart(trackExtension);
            
            instance.transform.position += generatable.GetSpawnOffset;
            instance.transform.position = new Vector3(instance.transform.position.x, 0, instance.transform.position.z);
            return instance;
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
            //SpawnGeneratable();
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