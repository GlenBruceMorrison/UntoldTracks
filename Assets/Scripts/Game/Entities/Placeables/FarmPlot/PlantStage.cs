using System;
using UnityEngine;

public class PlantStage : MonoBehaviour
{
    public float secondsAtThisStage;

    public bool ReachedNextStage(float timePassed)
    {
        return (secondsAtThisStage >= timePassed);
    }

    internal void Hide()
    {
        this.transform.gameObject.SetActive(false);
    }

    internal void Show()
    {
        this.transform.gameObject.SetActive(true);
    }
}
