using UnityEngine;
using System.Collections.Generic;

public class PlaceableEntity : Entity
{
    public Item source;
    public List<Transform> raycastOrigins = new List<Transform>();
}
