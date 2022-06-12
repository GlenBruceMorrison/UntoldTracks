using KinematicCharacterController;
using PathCreation;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

using UntoldTracks.Managers;
using UntoldTracks.Models;

[System.Serializable]
public class TrackGenerator : ITokenizable
{
    private BezierPath _bezierPath;
    private VertexPath _vertexPath;
    private List<Vector3> _points;

    [SerializeField] private int _pointGeneratedCount;
    [SerializeField] private int _distanceBetweenPointsMin, _distanceBetweenPointsMax;
    [SerializeField] private int _distanceBreadthMin, _distanceBreadthMax;
    [SerializeField] private int _distanceHeightMin, _distanceHeightMax;

    [SerializeField] private Vector3 _origin;

    public UnityAction<VertexPath> OnSegmentAdded, OnCreation;

    public VertexPath VertexPath
    {
        get
        {
            return _vertexPath;
        }
    }

    public BezierPath BezierPath
    {
        get
        {
            return _bezierPath;
        }
    }

    public List<Vector3> Points
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
            return _points[^1];
        }
    }

    private void GeneratePoints()
    {
        _points = new List<Vector3>
        {
            _origin
        };

        for (int i = 1; i < _pointGeneratedCount; i++)
        {
            var breadth = UnityEngine.Random.Range(_distanceBreadthMin, _distanceBreadthMax);

            var x = LastPoint.x > 0 ? -breadth : breadth;
            var z = LastPoint.z + UnityEngine.Random.Range(_distanceBetweenPointsMin, _distanceBetweenPointsMax);
            var y = 0;
            
            if (i > 2)
            {
                if (i % 3 == 0)
                {
                    y = UnityEngine.Random.Range(_distanceHeightMin, _distanceHeightMax);
                }
            }

            _points.Add(new Vector3(x, y, z));
        }
    }

    public void GenerateNewPoint()
    {

    }

    private void GeneratePath()
    {
        _bezierPath = new BezierPath(_points.ToArray(), false, PathSpace.xyz)
        {
            GlobalNormalsAngle = 90
        };

        _vertexPath = new VertexPath(_bezierPath, GameManager.Instance.TrainManager.transform);

        OnCreation?.Invoke(_vertexPath);
    }

    private void AddTrackSegment(Vector3 point)
    {
        _bezierPath.AddSegmentToEnd(point);
        _vertexPath = new VertexPath(_bezierPath, GameManager.Instance.TrainManager.transform);
        OnSegmentAdded?.Invoke(_vertexPath);
    }

    public void Initiate()
    {
        GeneratePoints();
        GeneratePath();
    }

    #region Token
    public void Load(JSONNode node)
    {
        var pointsJSON = node["points"];

        if (pointsJSON == null)
        {
            GeneratePoints();
        }
        else if (pointsJSON.Children == null || pointsJSON.Count <= 0)
        {
            GeneratePoints();
        }
        else
        {
            var points = new List<Vector3>();

            foreach (var item in pointsJSON.Children)
            {
                points.Add(item.ReadVector3());
            }

            _points = points;
        }

        GeneratePath();
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