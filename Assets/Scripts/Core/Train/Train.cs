using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Train : MonoBehaviour
{
    public List<Carriage> carriages = new List<Carriage>();
    public Carriage carriagePrefab;

    public int offset = 5;

    public int amountToSpawn = 1;

    private void Awake()
    {
        if (amountToSpawn == -1)
        {
            carriages = GetComponentsInChildren<Carriage>().ToList();

            return;
        }

        for (int i = 0; i < amountToSpawn; i++)
        {
            var carriage = Instantiate(carriagePrefab, transform);

            carriages.Add(carriage);
        }
    }
}
