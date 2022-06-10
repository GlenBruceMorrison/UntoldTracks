using PathCreation;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UntoldTracks.Data;
using UntoldTracks.Managers;
using UntoldTracks.Models;

public class Train : MonoBehaviour, ITokenizable
{
    public TrainData data;
    public TrainModel model;
    public int carriageLength = 9;

    [HideInInspector] public List<Carriage> carriages = new();

    [SerializeField] private float _currentSpeed = 5;
    [SerializeField] private int _maxSpeed = 15;
    [SerializeField] private float _acc;

    private VertexPath _vertexPath;
    private bool _moving;
    private float _distanceTravelled;
    private bool _built = false;

    public VertexPath VertexPath
    {
        get
        {
            return _vertexPath;
        }
        private set
        {
            _vertexPath = value;
            foreach (var carriage in carriages)
            {
                carriage.vertexPath = value;
            }
        }
    }

    public bool Moving
    {
        get
        {
            return _moving;
        }
    }

    public float CurrentSpeed
    {
        get
        {
            return _currentSpeed;
        }
    }

    public float DistanceTravelled
    {
        get
        {
            return _distanceTravelled;
        }
    }

    public UnityEvent OnCarriageAdded, OnTrainBrake, OnTrainStart, OnTrainStop;

    public void Initiate(TrackGenerator generator)
    {

    }

    private Carriage AddTrain(Carriage prefab)
    {
        var carriage = Instantiate(prefab);

        carriage.transform.parent = transform;
        carriage.vertexPath = _vertexPath;

        carriage.delay = -(carriages.Count * carriageLength);

        carriage.train = this;

        carriages.Add(carriage);

        OnCarriageAdded?.Invoke();

        return carriage;
    }

    public void StartStop()
    {
        _moving = !_moving;

        if (_moving)
        {
            OnTrainStart?.Invoke();
        }
        else
        {
            OnTrainBrake?.Invoke();
        }
    }

    private void Update()
    {
        if (!_built)
        {
            return;
        }

        if (_moving)
        {
            if (_currentSpeed < _maxSpeed)
            {
                _currentSpeed += _acc * Time.deltaTime;
            }
        }
        else
        {
            if (_currentSpeed > 0)
            {
                _currentSpeed -= (_acc * 3) * Time.deltaTime;
            }

            if (_currentSpeed < 0)
            {
                _currentSpeed = 0;
            }
        }

        _distanceTravelled += _currentSpeed * Time.deltaTime;
    }

    public Carriage GetClosestCarriage(Transform transform)
    {
        Carriage bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (var carriage in carriages)
        {
            Debug.Log(carriage.transform.position);

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
        _moving = node["isMoving"];
        _currentSpeed = node["currentSpeed"];
        _distanceTravelled = node["distanceTravelled"];

        var carriageJSON = node["carriages"];

        foreach (var item in carriageJSON.Children)
        {
            var guid = item["carriageGUID"].Value;
            var model = GameManager.Instance.Registry.FindByGUID<CarriageModel>(guid);
            var instance = AddTrain(model.prefab);

            instance.Load(item["carriageGUID"]);
        }

        _built = true;
    }

    public JSONObject Save()
    {
        var trainJSON = new JSONObject();

        trainJSON.Add("distanceTravelled", _distanceTravelled);
        trainJSON.Add("currentSpeed", _currentSpeed);
        trainJSON.Add("isMoving", _moving);

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
