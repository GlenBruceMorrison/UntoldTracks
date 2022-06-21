using PathCreation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeneratableBase : MonoBehaviour
{
    private PathCreator _pathCreator;

    public BezierPath BezierPath
    {
        get
        {
            return _pathCreator.bezierPath;
        }
    }

    public VertexPath VertexPath
    {
        get
        {
            return _pathCreator.path;
        }
    }

    public Vector3 GetSpawnOffset
    {
        get
        {
            return transform.position - GetFirstPoint;
        }
    }

    public Vector3 GetFirstPoint
    {
        get
        {
            return VertexPath.GetPoint(0);
        }
    }

    public Vector3 GetLastPoint
    {
        get
        {
            return VertexPath.GetPoint(VertexPath.NumPoints-1);
        }
    }
       
    private void OnEnable()
    {
        if (GetComponentInChildren<PathCreator>() == null)
            Debug.LogError("WE NEED A PATH CREATOR AS A CHILD OF THIS GENERATABLE. THE TRAIN NEEDS THIS TO KNOW WHAT TO FOLLOW");

        _pathCreator = GetComponentInChildren<PathCreator>();
    }

    void OnDrawGizmos()
    {
        if (_pathCreator == null)
        {
            _pathCreator = GetComponentInChildren<PathCreator>();
        }

        Gizmos.color = Color.green;
        Gizmos.DrawCube(GetFirstPoint, Vector3.one * 30);

        Gizmos.color = Color.red;
        Gizmos.DrawCube(GetLastPoint, Vector3.one * 30);

        //Gizmos.DrawCube(transform.position + GetSpawnOffset, Vector3.one);
    }
}
