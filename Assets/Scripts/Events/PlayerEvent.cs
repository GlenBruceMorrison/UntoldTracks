using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerEvent<T> : GameEvent<T>
{
    public PlayerManager player;
}