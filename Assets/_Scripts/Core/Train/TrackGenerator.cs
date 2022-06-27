using KinematicCharacterController;
using PathCreation;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

using UntoldTracks.Managers;
using UntoldTracks.Models;
using static PathCreation.BezierPath;

public interface ITrack
{
    public VertexPath VertexPath { get; }
}

[System.Serializable]
public class TrackGenerator : MonoBehaviour, ITokenizable, ITrack
{
    #region Debug
    private void OnDrawGizmos()
    {
        if (BezierPath == null) return;

        if (!showTrackPoints) return;

        for (int i = 0; i < BezierPath.Points.Count; i++)
        {
            var pos = BezierPath.Points[i];
            Gizmos.DrawCube(pos, new Vector3(0.3f, 10, 0.3f));
            Handles.Label(pos + Vector3.up * 15, i.ToString());
            Handles.Label( pos + (Vector3.up * 10), $"({pos.x}, {pos.y}, {pos.z})");
        }

        if (VertexPath == null) return;
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(LastTrackPositionInWorldSpace, new Vector3(0.3f, 20, 0.3f));
        
        var direction = VertexPath.GetTangent(VertexPath.NumPoints-1) * 5;
        Gizmos.DrawRay(LastTrackPositionInWorldSpace, direction);
        
        var directionTwo = VertexPath.GetNormal(VertexPath.NumPoints-1) * 5;
        Gizmos.DrawRay(LastTrackPositionInWorldSpace, directionTwo);
    }

    [SerializeField] public bool showTrackPoints = true;
    #endregion    
    
    public int intitialTrackCount = 4;
    public int distanceBetweenPointsMin = 200;
    public int distanceBetweenPointsMax = 400;
    public int distanceBreadthMin = 200;
    public int distanceBreadthMax = 400;

    public float initialTrackHeight = 0;

    public List<Vector3> initialGenerationPoints = new()
    {
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 300)
    };

    public VertexPath VertexPath { get; private set; }
    public BezierPath BezierPath { get; private set; }

    public List<Vector3> Tracks => BezierPath.Points;

    /// <summary>
    /// The Vector3 value of the last point we generated
    /// </summary>
    public Vector3 LastTrackPositionInWorldSpace => transform.position + BezierPath.Points[BezierPath.NumPoints-1];

    /// <summary>
    /// The number of points that have been generated so far, can be used to determine 
    /// how far we have come
    /// </summary>
    public int NumberOfTracks => BezierPath.NumPoints;

    #region Points Generation
    /// <summary>
    /// Generate a point based on the variables
    /// </summary>
    public Vector3 GenerateTrackPoint(Vector3 lastPoint)
    {
        var breadth = UnityEngine.Random.Range(distanceBreadthMin, distanceBreadthMax);

        var x = lastPoint.x > 0 ? -breadth : breadth;
        var z = lastPoint.z + UnityEngine.Random.Range(distanceBetweenPointsMin, distanceBetweenPointsMax);

        return new Vector3(x, 0, z);
    }

    private List<Vector3> GenerateInitialTracks(int toGenerate)
    {
        var points = initialGenerationPoints;
        var lastPoint = points.Last();

        for (int i = initialGenerationPoints.Count; i < toGenerate; i++)
        {
            var point = GenerateTrackPoint(lastPoint);
            points.Add(point);
            lastPoint = point;
        }

        return points;
    }
    #endregion

    #region Track Generation
    public void GenerateBezier(List<Vector3> points)
    {
        BezierPath = new BezierPath(
            points,
            false,
            PathSpace.xyz)
        {
            GlobalNormalsAngle = 90
        };

        RegenerateVertex();
    }

    public void GenerateBezier()
    {
        var points = GenerateInitialTracks( intitialTrackCount);

        points.ForEach(x => Debug.Log((x)));
        
        GenerateBezier(points);
    }

    public void RegenerateVertex()
    {
        VertexPath = new VertexPath(BezierPath, transform);
        GameObject.FindObjectOfType<RoadMeshCreator>().Init(VertexPath);
    }
    #endregion

    #region Track Extensions
    public void ExtendTrack()
    {
        ExtendTrack(new List<Vector3> { GenerateTrackPoint(LastTrackPositionInWorldSpace) });
    }

    public void ExtendTrack(Vector3 point, ControlMode controlMode = ControlMode.Automatic)
    {
        var nextTrack = new Vector3(point.x, point.y, LastTrackPositionInWorldSpace.z + point.z);
        ExtendTrack(new List<Vector3>(){nextTrack});
    }

    public void ExtendTrack(List<Vector3> points, ControlMode controlMode = ControlMode.Automatic)
    {
        BezierPath.ControlPointMode = controlMode;
        foreach(var point in points)
        {
            BezierPath.AddSegmentToEnd(point);
        }
        BezierPath.ControlPointMode = ControlMode.Automatic;

        RegenerateVertex();
    }

    public void ExtendTrack(VertexPath path)
    {
        for(var i=0;i<path.NumPoints;i++)
        {
            var point = path.GetPoint(i);

            BezierPath.AddSegmentToEnd(point);
        }

        RegenerateVertex();
    }
    #endregion

    #region Token
    public void Load(JSONNode node)
    {
        var pointsJSON = node["points"];

        // create new track data if it does not exist
        if (pointsJSON == null | pointsJSON.Children == null || pointsJSON.Count <= 0)
        {
            GenerateBezier();
            return;
        }

        // Load track data
        var points = new List<Vector3>();

        foreach (var item in pointsJSON.Children)
        {
            points.Add(item.ReadVector3());
        }

        GenerateBezier(points);
    }

    public JSONObject Save()
    {
        var pointsJSON = new JSONObject();

        var trackPointsJSON = new JSONArray();

        foreach (var point in Tracks)
        {
            trackPointsJSON.Add(point);
        }

        pointsJSON.Add("points", trackPointsJSON);

        return pointsJSON;
    }
    #endregion
}