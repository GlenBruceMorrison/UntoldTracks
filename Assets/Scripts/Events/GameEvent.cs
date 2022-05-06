using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameEvent<T>
{
    static event System.Action<T> onEvent;

    public static void RegisterListener(System.Action<T> listener)
    {
        onEvent += listener;
    }

    public static void UnregisterListener(System.Action<T> listener)
    {
        onEvent -= listener;
    }

    public static void BroadcastEvent(T eventData)
    {
        onEvent?.Invoke(eventData);
    }
}