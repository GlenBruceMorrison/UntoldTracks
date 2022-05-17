using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

public class FollowCurve : MonoBehaviour
{
    public PathCreator pathCreator;
    public EndOfPathInstruction endOfPathInstruction;
    public float speed = 5;
    float distanceTravelled;

    float startDistance = 0;

    private void Start()
    {
        pathCreator = GameObject.FindObjectOfType<PathCreator>();    
    }

    void Update()
    {
        distanceTravelled += speed * Time.deltaTime;
        transform.position = pathCreator.path.GetPointAtDistance(startDistance + distanceTravelled, endOfPathInstruction);
        transform.rotation = pathCreator.path.GetRotationAtDistance(startDistance + distanceTravelled, endOfPathInstruction);
    }
}
