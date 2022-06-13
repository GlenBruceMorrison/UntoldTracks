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

    [SerializeField] private int _pointsToGenerate;
    [SerializeField] private int _distanceBetweenPointsMin, _distanceBetweenPointsMax;
    [SerializeField] private int _distanceBreadthMin, _distanceBreadthMax;
    [SerializeField] private int _distanceHeightMin, _distanceHeightMax;

    [SerializeField] private Vector3 _origin;

    public UnityAction<VertexPath> OnNewPointGenerated;

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

    /// <summary>
    /// The running list of Vector3 points that have been generated so far
    /// </summary>
    public List<Vector3> Points
    {
        get
        {
            return _points;
        }
    }

    /// <summary>
    /// The Vector3 value of the last point we generated
    /// </summary>
    public Vector3 LastPoint
    {
        get
        {
            return _points[^1];
        }
    }

    /// <summary>
    /// The number of points that have been generated so far, can be used to determine 
    /// how far we have come
    /// </summary>
    public int NumberOfPointsGenerated
    {
        get
        {
            return _points.Count;
        }
    }

    /// <summary>
    /// Generate a List of points based on the variables
    /// </summary>
    private void GeneratePoints()
    {
        _points = new List<Vector3>
        {
            _origin
        };

        for (int i = 1; i < _pointsToGenerate; i++)
        {
            _points.Add(GenerateTrackPoint());
        }
    }

    /// <summary>
    /// Generate a point based on the variables
    /// </summary>
    public Vector3 GenerateTrackPoint()
    {
        var breadth = UnityEngine.Random.Range(_distanceBreadthMin, _distanceBreadthMax);

        var x = LastPoint.x > 0 ? -breadth : breadth;
        var z = LastPoint.z + UnityEngine.Random.Range(_distanceBetweenPointsMin, _distanceBetweenPointsMax);
        var y = 0;

        if (Points.Count > 2)
        {
            if (Points.Count % 3 == 0)
            {
                y = UnityEngine.Random.Range(_distanceHeightMin, _distanceHeightMax);
            }
        }

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Generates a new bexier and vertex path using the data in out points array
    /// </summary>
    private void GeneratePath()
    {
        _bezierPath = new BezierPath(_points.ToArray(), false, PathSpace.xyz)
        {
            GlobalNormalsAngle = 90
        };

        _vertexPath = new VertexPath(_bezierPath, GameManager.Instance.TrainManager.transform);
    }

    /// <summary>
    /// Extends the current track by a generated point
    /// </summary>
    public void ExtendTrack()
    {
        ExtendTrack(GenerateTrackPoint());
    }

    /// <summary>
    /// Extends the current track by a generated point but keeps the x value at 0
    /// </summary>
    public void ExtendTrack(int x)
    {
        var pos = GenerateTrackPoint();

        ExtendTrack(new Vector3(x, pos.y, pos.z));
    }

    /// <summary>
    /// Extends the current to a specific point
    /// </summary>
    public void ExtendTrack(Vector3 point)
    {
        //Points.RemoveAt(0);
        Points.Add(point);

        //_bezierPath.DeleteSegment(0);
        _bezierPath.AddSegmentToEnd(point);
        _vertexPath = new VertexPath(_bezierPath, GameManager.Instance.TrainManager.transform);

        OnNewPointGenerated?.Invoke(_vertexPath);

        GameObject.FindObjectOfType<RoadMeshCreator>().Init(VertexPath);
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