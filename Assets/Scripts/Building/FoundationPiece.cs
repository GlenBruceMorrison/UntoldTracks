using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UntoldTracks;
using UntoldTracks.Player;

public class FoundationPiece : MonoBehaviour
{
    private int _gridSize = 1;

    public List<Indicator> Indicators { get; private set; }

    private FoundationPiece _indicating;

    public GameObject testPrefab;

    private void Start()
    {
        Indicators = new List<Indicator>
        {
            CreateIndicator(IndicationDirection.Left, new Vector3(0, 0, 0), new Vector3(1, 1, 1)),
            CreateIndicator(IndicationDirection.Right, new Vector3(0, 0, 0), new Vector3(1, 1, 1)),
            CreateIndicator(IndicationDirection.Forward, new Vector3(0, 0, 0), new Vector3(1, 1, 1)),
            CreateIndicator(IndicationDirection.Backward, new Vector3(0, 0, 0), new Vector3(1, 1, 1))
        };
    }

    public Indicator CreateIndicator(IndicationDirection direction, Vector3 center, Vector3 size)
    {
        var result = new Indicator()
        {
            Direction = direction
        };

        var floorLeft = new GameObject("Highlight");

        floorLeft.transform.parent = this.transform;
        floorLeft.transform.localPosition = Vector3.zero;
        floorLeft.transform.localPosition = result.DirectionVector() * _gridSize;
        floorLeft.transform.localScale = new Vector3(1, 1, 1);

        var col = floorLeft.AddComponent<BoxCollider>();
        col.center = center;
        col.size = size;

        result.GameObject = floorLeft;
        result.Collider = col;

        /*
        var interactable = result.GameObject.AddComponent<UntoldTracks.UntoldTracks>();

        interactable.onGainFocus.AddListener((PlayerManager playerManager) =>
        {
            //playerManager.buildingController.MoveBuildObject(floorLeft.transform.position);
        });

        interactable.onLoseFocus.AddListener((PlayerManager playerManager) =>
        {
            //playerManager.buildingController.MoveBuildObject(Vector3.zero);
        });
        */

        return result;
    }

    public void IndicatePiece(FoundationPiece piece)
    {

    }

    public void OnEnable()
    {

    }
}