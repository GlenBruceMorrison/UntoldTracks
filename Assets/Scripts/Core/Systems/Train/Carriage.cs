using KinematicCharacterController;
using PathCreation;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks.Data;
using UntoldTracks.Managers;
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
        if (GameManager.Instance.TrainManager.TrackGenerator.VertexPath == null)
        {
            Debug.LogError("VertexPath on this carriage is null!");
            goalPosition = transform.position;
            goalRotation = transform.rotation;
            return;
        }

        if (GameManager.Instance.TrainManager.TrackGenerator.VertexPath.length == 0)
        {
            Debug.LogError("Vertex path does not have any points.");
            goalPosition = transform.position;
            goalRotation = transform.rotation;
            return;
        }

        goalPosition = GameManager.Instance.TrainManager.TrackGenerator.VertexPath
            .GetPointAtDistance(DistanceTravelled, endOfPathInstruction);

        goalRotation = GameManager.Instance.TrainManager.TrackGenerator.VertexPath
            .GetRotationAtDistance(DistanceTravelled, endOfPathInstruction);
    }

    #region Token
    public void Load(JSONNode node)
    {
        transform.position = GameManager.Instance.TrainManager.TrackGenerator.VertexPath
            .GetPointAtDistance(DistanceTravelled, endOfPathInstruction);

        transform.rotation = GameManager.Instance.TrainManager.TrackGenerator.VertexPath
            .GetRotationAtDistance(DistanceTravelled, endOfPathInstruction);
    }

    public JSONObject Save()
    {
        var carriageJSON = new JSONObject();

        carriageJSON.Add("carriageGUID", model.Guid);

        return carriageJSON;
    }
    #endregion
}