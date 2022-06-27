using System;
using PathCreation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEditor.Animations;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class GeneratableBase : MonoBehaviour
{
    private PathCreator _pathCreator;
    private BezierPath _bezierPath;

    public BezierPath BezierPath
    {
        get
        {
            if (_pathCreator == null)
            {
                _pathCreator = GetComponentInChildren<PathCreator>();
            }
            
            return _pathCreator.EditorData.bezierPath;
        }
    }

    public VertexPath VertexPath => new VertexPath(BezierPath, transform);
    public Vector3 GetSpawnOffset => transform.position - GetFirstTrackInWorldSpace;
    public Vector3 GetFirstTrackInLocalSpace => BezierPath.GetPoint(0);
    public Vector3 GetFirstTrackInWorldSpace => GetPointInWorldSpace(0);
    public Vector3 GetLastTrackInWorldSpace => GetPointInWorldSpace(BezierPath.NumPoints-1);
    public Vector3 GetPointInWorldSpace(int index) => transform.position + BezierPath.Points[index];

    public void AddTrackToStart(Vector3 offset, BezierPath.ControlMode mode = BezierPath.ControlMode.Automatic)
    {
        var previousFirstPoint = GetFirstTrackInWorldSpace;
        
        //BezierPath.ControlPointMode = mode;
        BezierPath.AddSegmentToStart(GetFirstTrackInLocalSpace + offset);
        
        var trackOffset = (previousFirstPoint - GetFirstTrackInWorldSpace);
        
        //Debug.Log($"Increasing position by [{previousFirstPoint} - {GetFirstPointWorldPosition}] to {transform.position + trackOffset}, GetFirstPointWorldPosition {GetFirstPointWorldPosition}");
        
        transform.position += trackOffset;
    }

    private void OnEnable()
    {
        if (GetComponentInChildren<PathCreator>() == null)
            Debug.LogError("WE NEED A PATH CREATOR AS A CHILD OF THIS GENERATABLE. THE TRAIN NEEDS THIS TO KNOW WHAT TO FOLLOW");
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(GetFirstTrackInWorldSpace, new Vector3(5, 5, 0.1f));

        Gizmos.color = Color.red;
        Gizmos.DrawCube(GetLastTrackInWorldSpace, new Vector3(5, 5, 0.1f));
    }
}
