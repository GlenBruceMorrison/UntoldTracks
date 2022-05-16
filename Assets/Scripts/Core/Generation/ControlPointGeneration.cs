using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

public class ControlPointGeneration : MonoBehaviour
{
    public int count;

    public float minDistance = 0.5f;
    public float maxDistance = 1.5f;

    public float maxWidth = 1.5f;

    public PathCreation.PathCreator path;

    private void Awake()
    {
        path = GameObject.FindObjectOfType<PathCreation.PathCreator>();

        var points = GeneratePoints(count, minDistance, maxDistance, maxWidth);

        foreach (var point in points)
        {
            path.bezierPath.AddSegmentToEnd(point);
        }
    }

    public Vector3 GeneratePoint(Vector3 previousPoint, float minDistance, float maxDistance, float maxWidth)
    {
        var x = previousPoint.x < 0
            ? Random.Range(0, maxWidth)
            : Random.Range(-maxWidth, 0);

        var z = previousPoint.z + Random.Range(minDistance, maxDistance);

        var newPoint = new Vector3(x, 0, z);

        return newPoint;
    }

    public List<Vector3> GeneratePoints(int count, float minDistance, float maxDistance, float maxWidth)
    {
        var results = new List<Vector3>();

        var previousPoint = Vector3.zero;

        for (int i=0; i<count; i++)
        {
            var point = GeneratePoint(previousPoint, minDistance, maxDistance, maxWidth);
            results.Add(point);
            previousPoint = point;
        }

        return results;
    }

    private void Start()
    {
        var points = GeneratePoints(count, minDistance, maxDistance, maxWidth);

        for (int i=0; i<points.Count; i++)
        {
            var obj = new GameObject($"Point {i}");
            obj.transform.parent = this.transform;
            obj.transform.localPosition = points[i];
        }

        //GetComponent<BezierCurveVisualizer>().ControlPoints = points;
        //GetComponent<BezierCurveVisualizer>().RenderLines();
    }
}
