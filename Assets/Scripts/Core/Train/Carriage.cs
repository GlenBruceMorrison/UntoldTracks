using KinematicCharacterController;
using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carriage : MonoBehaviour, IMoverController
{
    public PhysicsMover Mover;
    public PathCreator pathCreator;
    public float speed = 5;
    float distanceTravelled;
    public EndOfPathInstruction endOfPathInstruction;
    public float delay = 0;

    private void Start()
    {
        Mover.MoverController = this;
    }

    public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
    {
        distanceTravelled += speed * deltaTime;

        goalPosition = pathCreator.path.GetPointAtDistance(distanceTravelled-delay, endOfPathInstruction);
        goalRotation = pathCreator.path.GetRotationAtDistance(distanceTravelled-delay, endOfPathInstruction);
    }
}
