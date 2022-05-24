using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController;
using UnityEngine;

public class TrainMoveTest : MonoBehaviour, IMoverController
{
    public PhysicsMover Mover;
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;

    public bool moving = false;
    public float speed = 0;
    public float maxSpeed = 5;
    public float acceleration = 0.2f;
    
    private void Start()
    {
        _originalPosition = Mover.Rigidbody.position;
        _originalRotation = Mover.Rigidbody.rotation;

        Mover.MoverController = this;
    }

    public void Toggle()
    {
        moving = !moving;
    }
    
    public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
    {
        goalRotation = transform.localRotation;  
        
        if (!moving)
        {
            if (speed > 0)
            {
                speed -= acceleration * 3 * Time.deltaTime;
            }
        }
        else
        {
            if (speed < maxSpeed)
            {
                speed += acceleration * Time.deltaTime;
            } 
        }

        
        goalPosition = transform.position + Vector3.right * speed * Time.deltaTime;
    }
}
