using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks.Models;

public class TrainTesting : MonoBehaviour, ITrack
{
    public int carriagesToGenerate = 10;

    public Train train;
    private RoadMeshCreator mesh;
    private TrackGenerator track;
    public VertexPath VertexPath => track.VertexPath;

    private void Awake()
    {
        train = FindObjectOfType<Train>();
        mesh = FindObjectOfType<RoadMeshCreator>();
        track = FindObjectOfType<TrackGenerator>();

        GenerateTrack();
        SpawnGeneratable();
    }

    private void Start()
    {
        HandleUpdateTrack();
    }

    private void OnEnable()
    {
        //track.pathUpdated += HandleUpdateTrack;
    }

    private void OnDisable()
    {
        //track.pathUpdated -= HandleUpdateTrack;
    }

    public void HandleUpdateTrack()
    {
        //train.Init(this);
    }

    [ContextMenu("Init")]
    public void GenerateTrack()
    {
        track.GenerateBezier();
        train.Init(this);
        for (int i = 0; i < carriagesToGenerate; i++)
        {
            train.AddCarriage(ResourceService.Instance.GetRandomOfType<CarriageModel>().prefab);
        }
        train.Move();
        mesh.Init(VertexPath);
    }

    [ContextMenu("ExtendTrack")]
    public void ExtendTrack()
    {
        track.ExtendTrack();
        track.ExtendTrack();
        track.ExtendTrack();
        track.ExtendTrack();
    }

    [ContextMenu("SpawnGeneratable")]
    public void SpawnGeneratable()
    {
        track.ExtendTrack(new Vector3(0, 0, 100), BezierPath.ControlMode.Free);

        //var generatable = ResourceService.Instance.Generatables[UnityEngine.Random.Range(0, 2)];
        var generatable = ResourceService.Instance.Generatables[UnityEngine.Random.Range(0, ResourceService.Instance.Generatables.Count)];

        var firstPoint = generatable.BezierPath.Points[0];

        generatable.BezierPath.ControlPointMode = BezierPath.ControlMode.Free;
        generatable.BezierPath.AddSegmentToStart(new Vector3(0, 0, firstPoint.z - 500));
        generatable.BezierPath.ControlPointMode = BezierPath.ControlMode.Automatic;

        var instance = Instantiate(generatable, track.LastTrackPositionInWorldSpace, Quaternion.identity);

        instance.transform.position += instance.GetSpawnOffset;





        track.ExtendTrack(instance.VertexPath);
    }
}
