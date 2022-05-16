using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class BezierCurveVisualizer : MonoBehaviour
{
    public BezierPath path;

    private void Update()
    {
        path = new PathCreation.BezierPath(Vector3.zero);
    }
}


/*
public class BezierCurveVisualizer : MonoBehaviour
{
    public LineRenderer curveLineRenderer;
    public LineRenderer straightLineRenderer;

    public Color LineColour;
    public Color BezierCurveColour;

    public float LineWidth=1;
    public List<Vector3> ControlPoints = new List<Vector3>();


    void Awake()
    {
        curveLineRenderer = CreateLine();
        straightLineRenderer = CreateLine();

        curveLineRenderer.gameObject.name = "CurveLineRenderer";
        straightLineRenderer.gameObject.name = "StraightLineRenderer";
    }

    private void OnEnable()
    {
        RenderLines();
    }

    private LineRenderer CreateLine()
    {
        var obj = new GameObject();

        var lineRenderer = obj.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = LineColour;
        lineRenderer.endColor = LineColour;
        lineRenderer.startWidth = LineWidth;
        lineRenderer.endWidth = LineWidth;

        return lineRenderer;
    }

    public void RenderLines()
    {
        var points = new List<Vector3>();

        for (int k = 0; k < ControlPoints.Count; ++k)
        {
            points.Add(ControlPoints[k]);
        }

        straightLineRenderer.positionCount = points.Count;
        for (int i = 0; i < points.Count; ++i)
        {
            straightLineRenderer.SetPosition(i, points[i]);
        }

        var curve = BezierCurve.PointList3D(points, 0.01f);
        curveLineRenderer.startColor = BezierCurveColour;
        curveLineRenderer.endColor = BezierCurveColour;
        curveLineRenderer.positionCount = curve.Count;

        for (int i = 0; i < curve.Count; ++i)
        {
            Debug.Log(curve[i]);

            curveLineRenderer.SetPosition(i, curve[i]);
        }
    }


    public Vector3 GetPointAlongCurve(float distance)
    {
        var curve = BezierCurve.PointList3D(ControlPoints, 0.01f);


        return Vector3.zero;
    }
}
*/