using PathCreation;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UntoldTracks.Managers;
using UntoldTracks.Models;

public class Train : MonoBehaviour, ITokenizable
{
    public TrainModel model;
    public int carriageLength = 9;

    [HideInInspector] public List<Carriage> carriages = new();

    private bool _initiated = false;

    [SerializeField] private float _currentSpeed = 5;
    [SerializeField] private int _maxSpeed = 15;
    [SerializeField] private float _acc;
    [SerializeField] private bool _moving;
    [SerializeField] private float _distanceTravelled;

    public ITrack Track
    {
        get;
        private set;
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
        set
        {
            _currentSpeed = value;
        }
    }

    public int MaxSpeed
    {
        get
        {
            return _maxSpeed;
        }
        set
        {
            _maxSpeed = value;
        }
    }

    public float DistanceTravelled
    {
        get
        {
            return _distanceTravelled;
        }
        set
        {
            _distanceTravelled = value;
        }
    }

    public Vector3 GetPointAtPosition => throw new NotImplementedException();

    public UnityEvent OnCarriageAdded, OnTrainBrake, OnTrainStart, OnTrainStop;

    private void Start()
    {
        /*
        if (FindObjectOfType<PlayerManager>() == null)
        {
            var pathCreator = FindObjectOfType<PathCreator>();

            pathCreator.pathUpdated += () => VertexPath = new VertexPath(pathCreator.bezierPath, pathCreator.transform);

            var vert = new VertexPath(pathCreator.bezierPath, pathCreator.transform);

            _vertexPath = vert;

            for (int i = 0; i < 10; i++)
            {
                var carriageModel = ResourceService.Instance.GetRandomOfType<CarriageModel>();
                AddTrain(carriageModel.prefab);
            }
            _built = true;
        }
        */
    }

    public void Init(ITrack track)
    {
        Track = track ?? throw new ArgumentNullException(nameof(track));
        _initiated = true;
    }

    public Carriage AddCarriage(Carriage prefab)
    {
        var carriage = Instantiate(prefab);
        var delay = -(carriages.Count * carriageLength);
        carriage.Init(this, delay);
        carriages.Add(carriage);
        OnCarriageAdded?.Invoke();
        return carriage;
    }

    public void Move()
    {
        if (_moving)
        {
            return;
        }

        _moving = true;
        OnTrainStart?.Invoke();
    }

    public void Brake()
    {
        if (!_moving)
        {
            return;
        }

        _moving = false;
        OnTrainBrake?.Invoke();
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
        if (!_initiated)
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

            //if (_currentSpeed < 0)
            //{
            //    _currentSpeed = 0;
            //}
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
            var model = ResourceService.Instance.FindByGUID<CarriageModel>(guid);

            if (model == null)
            {
                Debug.LogError($"Could not find carriage of ID {guid}");
                continue;
            }

            var instance = AddCarriage(model.prefab);

            instance.Load(item["carriageGUID"]);
        }

        _initiated = true;
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
