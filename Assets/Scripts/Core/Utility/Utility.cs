using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public static class Utility
{
    public static void SetLayerRecursively(this GameObject obj, string layerName)
    {
        obj.SetLayerRecursively(LayerMask.NameToLayer(layerName));
    }

    public static void SetLayerRecursively(this GameObject obj, int layerIndex)
    {
        if (null == obj)
        {
            return;
        }

        obj.layer = layerIndex;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }

            SetLayerRecursively(child.gameObject, layerIndex);
        }
    }

    public static void ChildCollidersToTriggers(this GameObject obj, bool isTrigger=true)
    {
        if (null == obj) return;
        obj.GetComponentsInChildren<Collider>().ToList().ForEach(x => x.isTrigger = isTrigger);
    }
}
