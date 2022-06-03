using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGrowthStage : MonoBehaviour
{
    public float timeNeeded;

    private void Awake()
    {
        timeNeeded *= Random.Range(0.8f, 1.2f);
    }
}
