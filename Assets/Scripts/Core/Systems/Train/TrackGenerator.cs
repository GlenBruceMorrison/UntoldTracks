using KinematicCharacterController;
using PathCreation;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

using UntoldTracks.Data;
using UntoldTracks.Managers;
using UntoldTracks.Models;

[System.Serializable]
public class TrackGenerator : ITokenizable
{
    public TrainManager TrainMananger => GameManager.Instance.TrainManager;

    private BezierPath _bezierPath;
    private VertexPath _vertexPath;
    [SerializeField] private Vector3[] _points;

    [SerializeField] private int _pointGeneratedCount;
    [SerializeField] private int _distanceBetweenPointsMin, _distanceBetweenPointsMax;
    [SerializeField] private int _distanceBreadthMin, _distanceBreadthMax;
    [SerializeField] private int _distanceHeightMin, _distanceHeightMax;

    [SerializeField] Vector3 min, max;

    [SerializeField] private Vector3 _origin;

    public UnityAction<VertexPath> OnSegmentAdded, OnCreation;

    public Vector3[] Points
    {
        get
        {
            return _points;
        }
    }

    public Vector3 LastPoint
    {
        get
        {
            return _points[_points.Length - 1];
        }
    }

    private Vector3[] GeneratePoints(int amount)
    {
        var points = new Vector3[amount];

        points[0] = _origin;

        var previousPoint = _origin;

        for (int i = 1; i < amount; i++)
        {
            var breadth = UnityEngine.Random.Range(_distanceBreadthMin, _distanceBreadthMax);

            var x = previousPoint.x > 0 ? -breadth : breadth;
            var z = previousPoint.z + UnityEngine.Random.Range(_distanceBetweenPointsMin, _distanceBetweenPointsMax);
            var y = 0;
            
            if (i > 2)
            {
                if (i % 3 == 0)
                {
                    y = UnityEngine.Random.Range(_distanceHeightMin, _distanceHeightMax);
                }
            }

            var nextPoint = new Vector3(x, y, z);

            points[i] = nextPoint;

            previousPoint = nextPoint;
        }

        return points;
    }

    public void GenerateNewPoint()
    {

    }

    private void GeneratePath(Vector3[] points)
    {
        _bezierPath = new BezierPath(points, false, PathSpace.xyz)
        {
            GlobalNormalsAngle = 90
        };

        _vertexPath = new VertexPath(_bezierPath, TrainMananger.transform);

        Debug.Log("On Creation");

        OnCreation?.Invoke(_vertexPath);
    }

    private void AddTrackSegment(Vector3 point)
    {
        _bezierPath.AddSegmentToEnd(point);
        _vertexPath = new VertexPath(_bezierPath, TrainMananger.transform);
        OnSegmentAdded?.Invoke(_vertexPath);
    }

    public void Initiate()
    {
        _points = GeneratePoints(_pointGeneratedCount);
        GeneratePath(_points);
    }

    #region Token
    public void Load(JSONNode node)
    {
        var pointsJSON = node["points"];

        if (pointsJSON == null)
        {
            _points = GeneratePoints(_pointGeneratedCount);
        }
        else if (pointsJSON.Children == null || pointsJSON.Count <= 0)
        {
            _points = GeneratePoints(_pointGeneratedCount);
        }
        else
        {
            Debug.Log("loading points");
            var points = new List<Vector3>();

            foreach (var item in pointsJSON.Children)
            {
                points.Add(item.ReadVector3());
            }

            _points = points.ToArray();
        }

        GeneratePath(_points);
    }

    public JSONObject Save()
    {
        var pointsJSON = new JSONObject();

        var trackPointsJSON = new JSONArray();

        foreach (var point in _points)
        {
            trackPointsJSON.Add(point);
        }

        pointsJSON.Add("points", trackPointsJSON);

        return pointsJSON;
    }
    #endregion
}