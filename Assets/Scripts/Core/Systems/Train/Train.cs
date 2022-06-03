using PathCreation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Train : MonoBehaviour
{
    public int carriagesToSpawn = 10;
    public int carriageLength = 9;

    public List<Carriage> carriagePrefabs = new List<Carriage>();
    public PathCreator path;

    [HideInInspector] public List<Carriage> carriages = new List<Carriage>();

    public bool moving;
    public float currentSpeed = 5;
    public int maxSpeed = 15;
    public float acc;

    public float distanceTravelled;

    private void Awake()
    {
        for (var i = 0; i < carriagesToSpawn; i++)
        {
            AddTrain(carriagePrefabs[Random.Range(0, carriagePrefabs.Count)]);
        }
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
