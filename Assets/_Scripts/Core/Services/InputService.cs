using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class InputService : Singleton<InputService>
{
    public UnityAction OnPrimaryDown, OnPrimaryUp;
    public UnityAction OnSecondaryDown, OnSecondaryUp;
    public UnityAction OnAction1Down, OnAction1Up;

    public bool PrimaryDown { get; private set; }
    public bool SecondaryDown { get; private set; }
    public bool Action1Down { get; private set; }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PrimaryDown = true;
            OnPrimaryDown?.Invoke();
        }

        if (Input.GetMouseButtonUp(0))
        {
            PrimaryDown = false;
            OnPrimaryUp?.Invoke();
        }


        if (Input.GetMouseButtonDown(1))
        {
            SecondaryDown = true;
            OnSecondaryDown?.Invoke();
        }

        if (Input.GetMouseButtonUp(1))
        {
            SecondaryDown = false;
            OnSecondaryUp?.Invoke();
        }


        if (Input.GetKeyDown(KeyCode.E))
        {
            Action1Down = true;
            OnAction1Down?.Invoke();
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            Action1Down = false;
            OnAction1Up?.Invoke();
        }
    }
}
