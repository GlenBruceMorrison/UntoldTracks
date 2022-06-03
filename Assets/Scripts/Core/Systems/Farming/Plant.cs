using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public partial class Plant : Entity
{
    [SerializeField] private PlantGrowthStage[] _growthStages;
    
    [SerializeField] private float _currentGrowthTime = 0f;

    public GrowingState currentGrowthState;
    public PlantGrowthStage currentGrowthStage;

    private void Start()
    {
        SetupBasedOnGrowthTime(0);
    }

    private void SetupBasedOnGrowthTime(float growthTime)
    {
        _currentGrowthTime = growthTime;

        _growthStages = GetComponentsInChildren<PlantGrowthStage>();

        HideAllGrowthStageTransforms();

        currentGrowthStage = GetGrowthStageByGrowthTime(_currentGrowthTime);
        currentGrowthState = GrowingState.Growing;

        EnableCurrentGrowthStageTransform();
    }

    private PlantGrowthStage GetGrowthStageByGrowthTime(float growthTime)
    {
        foreach (var growthStage in _growthStages.OrderByDescending(x => x.timeNeeded))
        {
            if (growthTime > growthStage.timeNeeded)
            {
                return growthStage;
            }
        }

        if (!_growthStages.Any())
        {
            return null;
        }

        return _growthStages.OrderBy(x => x.timeNeeded)?.First();
    }

    private void HideAllGrowthStageTransforms()
    {
        foreach (var growthStage in _growthStages.Reverse())
        {
            growthStage.gameObject.SetActive(false);
        }
    }

    private void EnableCurrentGrowthStageTransform()
    {
        if (currentGrowthStage == null)
        {
            return;
        }

        currentGrowthStage.gameObject.SetActive(true);
    }

    private void Update()
    {
        switch (currentGrowthState)
        {
            case GrowingState.Growing:
                _currentGrowthTime += Time.deltaTime;

                var targetGrowthStage = GetGrowthStageByGrowthTime(_currentGrowthTime);

                if (targetGrowthStage == null || !_growthStages.Any())
                {
                    currentGrowthState = GrowingState.Empty;
                }

                if (targetGrowthStage != currentGrowthStage)
                {
                    HideAllGrowthStageTransforms();

                    currentGrowthStage = targetGrowthStage;

                    EnableCurrentGrowthStageTransform();
                }

                if (currentGrowthStage == _growthStages.Last() && _currentGrowthTime > currentGrowthStage.timeNeeded)
                {
                    currentGrowthState = GrowingState.Grown;
                }
                break;
        }
    }
}

