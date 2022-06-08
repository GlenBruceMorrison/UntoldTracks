using System.Collections.Generic;
using UnityEngine;
using UntoldTracks.Models;
using System.Linq;

public class PlantRecipe : MonoBehaviour
{
    public ItemModel input;
    public List<ItemModel> outputs = new();
    public List<PlantStage> stages = new();

    public int currentIndex;
    public float timePassed;

    public float varianceMultiplier = 0.2f;

    public bool IsPickable
    {
        get
        {
            return currentIndex >= stages.Count - 1;
        }
    }

    private void Awake()
    {
        Reset();
    }

    public void Reset()
    {
        currentIndex = 0;
        timePassed = 0;
        GenerateVariances();
        ShowStage(0);
    }

    public void GenerateVariances()
    {
        return;
        foreach (var stage in stages)
        {
            stage.secondsAtThisStage *= Random.Range(1 - varianceMultiplier, 1 + varianceMultiplier);
        }
    }

    public void Grow(float deltaTime)
    {
        if (!IsPickable)
        {
            timePassed += deltaTime;

            if (ReachedNextStage())
            {
                UpdateGrowthStage();
            }
        }
    }

    public bool ReachedNextStage()
    {
        var timeRequired = stages.Where((x, index) => index < currentIndex+1).Sum(x => x.secondsAtThisStage);

        return timePassed >= timeRequired;
    }

    public void ShowStage(int index)
    {
        foreach (var x in stages)
        {
            x.Hide();
        }

        stages[index].Show();
    }

    public void UpdateGrowthStage()
    {
        for (var i=stages.Count-1; i>=0; i--)
        {
            var stage = stages[i];

            var startTime = stages.Where((x, index) => index < i).Sum(x => x.secondsAtThisStage);
            
            if (timePassed >= startTime && i != currentIndex)
            {
                ShowStage(i);
                currentIndex = i;
                return;
            }
        }
    }
}
