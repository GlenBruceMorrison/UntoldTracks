using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace UntoldTracks.UI
{
    public class UserInterfaceCollection : MonoBehaviour
    {
        public Dictionary<string, Transform> _panels = new Dictionary<string, Transform>();

        public Transform GetPanel(string name)
        {
            if (_panels.TryGetValue(name, out Transform result))
            {
                return result;
            }

            new System.Exception($"Could not find panel named {name}");

            return null;
        }
    }

    public class UserInterface : MonoBehaviour
    {
        public Image img;
        public Dictionary<object, Type> _panels = new Dictionary<object, Type>();

        public void AddElement(string name)
        {
            var element = this.transform.Find(name);

        }
    }
}
