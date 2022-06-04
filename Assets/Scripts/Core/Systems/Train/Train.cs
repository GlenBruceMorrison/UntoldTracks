using PathCreation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UntoldTracks.Data;
using UntoldTracks.Models;

public class Train : MonoBehaviour
{
    public TrainData data;
    public TrainModel model;
    public CarriageRegistry carriageRegistry;

    public int carriageLength = 9;

    public PathCreator path;

    [HideInInspector] public List<Carriage> carriages = new List<Carriage>();

    public bool moving;
    public float currentSpeed = 5;
    public int maxSpeed = 15;
    public float acc;

    public float distanceTravelled;

    public bool built = false;

    public void Build(TrainData data)
    {
        foreach (var carriage in data.carriages)
        {
            var instance = carriageRegistry.FindByGUID(carriage.carriageGUID);
            AddTrain(instance.view);
        }

        if (data.isMoving)
        {
            moving = true;
        }

        currentSpeed = data.currentSpeed;
        distanceTravelled = data.distanceTravelled;

        built = true;
    }

    public TrainData Save()
    {
        var data = new TrainData()
        {
            distanceTravelled = distanceTravelled,
            currentSpeed = currentSpeed,
            isMoving = moving
        };

        foreach (var carriage in carriages)
        {
            data.carriages.Add(new CarriageData()
            {
                carriageGUID = carriage.model.Guid
            });
        }

        return data;
    }

    private void AddTrain(Carriage prefab)
    {
        var carriage = Instantiate(prefab);

        carriage.transform.parent = transform;
        carriage.pathCreator = path;

        carriage.delay = -(carriages.Count * carriageLength);

        carriage.train = this;

        carriages.Add(carriage);
    }

    public void StartStop()
    {
        moving = !moving;
    }

    private void Update()
    {
        if (!built)
        {
            return;
        }

        if (moving)
        {
            if (currentSpeed < maxSpeed)
            {
                currentSpeed += acc * Time.deltaTime;
            }
        }
        else
        {
            if (currentSpeed > 0)
            {
                currentSpeed -= (acc * 3) * Time.deltaTime;
            }
        }

        distanceTravelled += currentSpeed * Time.deltaTime;
    }
}
