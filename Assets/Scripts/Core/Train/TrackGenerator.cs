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
    [SerializeField] private List<Vector3> _points;

    [SerializeField] private int _pointsToGenerate = 10;
    [SerializeField] private int _distanceBetweenPointsMin, _distanceBetweenPointsMax;
    [SerializeField] private int _distanceBreadthMin, _distanceBreadthMax;

    [SerializeField] private Vector3 _origin;

    public UnityAction<VertexPath> OnNewPointGenerated;

    public VertexPath VertexPath => _vertexPath;
    public BezierPath BezierPath => _bezierPath;

    /// <summary>
    /// The running list of Vector3 points that have been generated so far
    /// </summary>
    public List<Vector3> Points => _points;

    /// <summary>
    /// The Vector3 value of the last point we generated
    /// </summary>
    public Vector3 LastPoint => _points.Last();

    /// <summary>
    /// The number of points that have been generated so far, can be used to determine 
    /// how far we have come
    /// </summary>
    public int NumberOfPointsGenerated => _points.Count();

    /// <summary>
    /// Generate a point based on the variables
    /// </summary>
    public Vector3 GenerateTrackPoint()
    {
        var breadth = UnityEngine.Random.Range(_distanceBreadthMin, _distanceBreadthMax);

        var x = LastPoint.x > 0 ? -breadth : breadth;
        var z = LastPoint.z + UnityEngine.Random.Range(_distanceBetweenPointsMin, _distanceBetweenPointsMax);

        return new Vector3(x, 0, z);
    }

    /// <summary>
    /// Extends the current track by a generated point
    /// </summary>
    public void ExtendTrack()
    {
        ExtendTrack(new List<Vector3> { GenerateTrackPoint() });
    }

    /// <summary>
    /// Extends the current track by a generated point but keeps the x value at 0
    /// </summary>
    public void ExtendTrack(int x)
    {
        var pos = GenerateTrackPoint();
        ExtendTrack(new List<Vector3> { new Vector3(x, pos.y, pos.z) });
    }

    /// <summary>
    /// Extends the current to a specific point
    /// </summary>
    public void ExtendTrack(List<Vector3> points)
    {
        foreach(var point in points)
        {
            _points.Add(point);
            _bezierPath.AddSegmentToEnd(point);
        }

        _vertexPath = new VertexPath(_bezierPath, GameManager.Instance.TrainManager.transform);
        OnNewPointGenerated?.Invoke(_vertexPath);
        GameObject.FindObjectOfType<RoadMeshCreator>().Init(VertexPath);
    }

    /// <summary>
    /// Extends the current to a specific point
    /// </summary>
    public void ExtendTrack(VertexPath path)
    {
        for(var i=0;i<path.NumPoints;i++)
        {
            var point = path.GetPoint(i);

            Debug.Log(point);

            _points.Add(point);
            _bezierPath.AddSegmentToEnd(point);
        }

        _vertexPath = new VertexPath(_bezierPath, GameManager.Instance.TrainManager.transform);
        OnNewPointGenerated?.Invoke(_vertexPath);
        GameObject.FindObjectOfType<RoadMeshCreator>().Init(VertexPath);
    }

    #region Token
    public void Load(JSONNode node)
    {
        var pointsJSON = node["points"];

        if (pointsJSON == null | pointsJSON.Children == null || pointsJSON.Count <= 0)
        {
            _points.Add(_origin);

            for (int i = 1; i < _pointsToGenerate; i++)
            {
                _points.Add(GenerateTrackPoint());
            }
        }
        else
        {
            foreach (var item in pointsJSON.Children)
            {
                _points.Add(item.ReadVector3());
            }
        }

        _bezierPath = new BezierPath(_points, false, PathSpace.xyz)
        {
            GlobalNormalsAngle = 90
        };

        _vertexPath = new VertexPath(_bezierPath, GameManager.Instance.TrainManager.transform);
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