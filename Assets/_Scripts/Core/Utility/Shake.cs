using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Shake : MonoBehaviour
{
    public Vector3 axisShakeMin;
    public Vector3 axisShakeMax;
    public float timeOfShake;
    private float timeOfShakeStore;
    private bool shake;
    private Vector3 startPos;
    // Use this for initialization
    void Start()
    {
        shake = false;
        startPos = transform.eulerAngles;
        timeOfShakeStore = timeOfShake;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (shake)
        {
            transform.localEulerAngles = startPos + new Vector3(Random.Range(axisShakeMin.x, axisShakeMax.x), Random.Range(axisShakeMin.y, axisShakeMax.y), Random.Range(axisShakeMin.z, axisShakeMax.z));
            timeOfShake -= Time.deltaTime;
            if (timeOfShake <= 0.0f)
            {
                shake = false;
                transform.localEulerAngles = startPos;
            }
        }
    }

    public void StartShaking(float shakeTime = -1.0f)
    {
        if (shakeTime > 0.0f)
        {
            timeOfShake = shakeTime;
        }
        else
        {
            timeOfShake = timeOfShakeStore;
        }
        shake = true;
    }
}