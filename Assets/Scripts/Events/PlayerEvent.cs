using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks.Player;

public abstract class PlayerEvent<T> : GameEvent<T>
{
    public PlayerManager player;
}