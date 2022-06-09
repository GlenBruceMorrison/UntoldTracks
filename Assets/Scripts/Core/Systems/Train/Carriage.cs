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
    public VertexPath vertexPath;
    public EndOfPathInstruction endOfPathInstruction;
    public float delay = 0;

    public float DistanceTravelled
    {
        get
        {
            return train.DistanceTravelled - delay;
        }
    }

    public float Speed
    {
        get
        {
            return train.CurrentSpeed;
        }
    }

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
        if (vertexPath == null)
        {
            Debug.LogError("VertexPath on this carriage is null!");
            goalPosition = transform.position;
            goalRotation = transform.rotation;
            return;
        }

        goalPosition = vertexPath.GetPointAtDistance(DistanceTravelled, endOfPathInstruction);
        goalRotation = vertexPath.GetRotationAtDistance(DistanceTravelled, endOfPathInstruction);
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