using KinematicCharacterController;
using PathCreation;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks.Data;
using UntoldTracks.Models;

public class Carriage : MonoBehaviour, IMoverController, ITokenizable
{
    public CarriageModel model;

    public Train train;
    public PhysicsMover Mover;
    public PathCreator pathCreator;
    public float Speed => train.currentSpeed;
    float DistanceTravelled => train.distanceTravelled - delay;
    public EndOfPathInstruction endOfPathInstruction;
    public float delay = 0;

    private void Start()
    {
        Mover.MoverController = this;
    }

    public void StartStop()
    {
        train.StartStop();
    }

    public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
    {
        goalPosition = pathCreator.path.GetPointAtDistance(DistanceTravelled, endOfPathInstruction);
        goalRotation = pathCreator.path.GetRotationAtDistance(DistanceTravelled, endOfPathInstruction);
    }

    #region Token
    public void Load(JSONNode node)
    {
        throw new NotImplementedException();
    }

    public JSONObject Save()
    {
        var carriageJSON = new JSONObject();

        carriageJSON.Add("carriageGUID", model.Guid);

        return carriageJSON;
    }
    #endregion
}