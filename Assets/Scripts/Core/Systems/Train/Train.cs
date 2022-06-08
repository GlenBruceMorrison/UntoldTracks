using PathCreation;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UntoldTracks.Data;
using UntoldTracks.Managers;
using UntoldTracks.Models;

public class Train : MonoBehaviour, ITokenizable
{
    public TrainData data;
    public TrainModel model;
    public SerializableRegistry registry;

    public int carriageLength = 9;

    public PathCreator path;

    [HideInInspector] public List<Carriage> carriages = new List<Carriage>();

    public bool moving;
    public float currentSpeed = 5;
    public int maxSpeed = 15;
    public float acc;

    public float distanceTravelled;

    public bool built = false;

    private Carriage AddTrain(Carriage prefab)
    {
        var carriage = Instantiate(prefab);

        carriage.transform.parent = transform;
        carriage.pathCreator = path;

        carriage.delay = -(carriages.Count * carriageLength);

        carriage.train = this;

        carriages.Add(carriage);

        return carriage;
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

            if (currentSpeed < 0)
            {
                currentSpeed = 0;
            }
        }

        distanceTravelled += currentSpeed * Time.deltaTime;
    }

    public Carriage GetClosestCarriage(Transform transform)
    {
        Carriage bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (var carriage in carriages)
        {
            Vector3 directionToTarget = carriage.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = carriage;
            }
        }

        return bestTarget;
    }

    #region Token
    public void Load(JSONNode node)
    {
        moving = node["isMoving"];
        currentSpeed = node["currentSpeed"];
        distanceTravelled = node["distanceTravelled"];

        var carriageJSON = node["carriages"];

        foreach (var item in carriageJSON.Children)
        {
            var guid = item["carriageGUID"].Value;
            var model = GameManager.Instance.Registry.FindByGUID<CarriageModel>(guid);
            var instance = AddTrain(model.prefab);
        }

        built = true;
    }

    public JSONObject Save()
    {
        var trainJSON = new JSONObject();

        trainJSON.Add("distanceTravelled", distanceTravelled);
        trainJSON.Add("currentSpeed", currentSpeed);
        trainJSON.Add("isMoving", moving);

        var carriagesJSON = new JSONArray();

        foreach (var carriage in carriages)
        {
            carriagesJSON.Add(carriage.Save());
        }
        trainJSON.Add("carriages", carriagesJSON);

        return trainJSON;
    }
    #endregion
}
