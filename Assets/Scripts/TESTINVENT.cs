using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks.Inventory;
using UntoldTracks.Player;

public class TESTINVENT : MonoBehaviour
{
    public PlayerInventoryController invent;

    public Item take;

    private void Awake()
    {
        invent = FindObjectOfType<PlayerManager>().inventoryController;
    }

    private void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            var containerTake = new ItemContainer(take, 15);
            var resultGive = invent.Inventory.Give(containerTake);
        }

        if (Input.GetKeyDown("2"))
        {
            var queryTake = new ItemQuery(take, 15);
            var resultTake = invent.Inventory.Take(queryTake);
        }
    }
}
