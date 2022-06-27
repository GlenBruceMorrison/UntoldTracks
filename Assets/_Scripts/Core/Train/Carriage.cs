using KinematicCharacterController;
using PathCreation;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks.Managers;
using UntoldTracks.Models;

public class Carriage : MonoBehaviour, IMoverController, ITokenizable
{
    [SerializeField] private CarriageModel _model;

    private Train _train;
    private bool _isInitiated;
    private float _delay = 0;

    public PhysicsMover Mover;

    public float DistanceTravelled
    {
        get
        {
            return _train.DistanceTravelled - _delay;
        }
    }

    public void Init(Train train, float delay)
    {
        _train = train != null ? train : throw new ArgumentNullException(nameof(train));
        transform.parent = train.transform;
        _delay = delay;
        Mover.MoverController = this;

        transform.SetPositionAndRotation(
            _train.Track.VertexPath.GetPointAtDistance(DistanceTravelled, EndOfPathInstruction.Stop),
            _train.Track.VertexPath.GetRotationAtDistance(DistanceTravelled, EndOfPathInstruction.Stop));
            
        _isInitiated = true;
    }

    public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
    {
        if (!_isInitiated)
        {
            Debug.LogError("not initiated!");
            goalPosition = transform.position;
            goalRotation = transform.rotation;
            return;
        }

        goalPosition = _train.Track.VertexPath.GetPointAtDistance(DistanceTravelled, EndOfPathInstruction.Stop);
        goalRotation = _train.Track.VertexPath.GetRotationAtDistance(DistanceTravelled, EndOfPathInstruction.Stop);
    }

    public void StartStop()
    {
        _train.StartStop();
    }
    
    /*
    void OnPathChanged()
    {
        distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
    }
    */

    #region Token
    public void Load(JSONNode node)
    {

    }

    public JSONObject Save()
    {
        var carriageJSON = new JSONObject();

        if (_model == null)
        {
            throw new Exception("There is no model attatched to this carriage, it cannot be saved!");
        }

        carriageJSON.Add("carriageGUID", _model.Guid);

        return carriageJSON;
    }
    #endregion
}