using PathCreation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Train : MonoBehaviour
{
    public float trainSpeed = 5;
    public int carriageLength = 9;
    public int carriagesToSpawn = 10;
    public List<Carriage> carriages = new List<Carriage>();

    public Carriage carriagePrefab;
    public PathCreator path;

    private void Awake()
    {
        for (var i = 0; i < carriageLength; i++)
        {
            var carriage = Instantiate(carriagePrefab);
            
            carriage.transform.parent = transform;
            carriage.pathCreator = path;
            carriage.speed = trainSpeed;

            carriage.delay = -(i * carriageLength);

            carriages.Add(carriage);
        }
    }
}
