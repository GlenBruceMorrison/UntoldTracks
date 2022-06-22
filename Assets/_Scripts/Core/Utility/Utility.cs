using System;
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

    /// <summary>
    ///  Gets the most cimilar string to a given string from a list
    /// </summary>
    public static string Match(this string me, List<string> others)
    {
        static int Compute(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        var closest = int.MaxValue;
        var matched = string.Empty;

        foreach (var str in others)
        {
            var result = Compute(me, str);

            if (result < closest)
            {
                closest = result;
                matched = str;
            }
        }

        return matched;
    }
}
