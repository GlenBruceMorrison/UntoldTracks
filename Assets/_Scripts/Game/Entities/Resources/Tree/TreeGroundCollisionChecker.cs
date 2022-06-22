using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TreeGroundCollisionChecker : MonoBehaviour
{
    [SerializeField] private Tree tree;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Terrain")) return;
        Debug.Log(other?.gameObject?.name);
        tree.HandleTreeGrounded();
        this.GameObject().SetActive(false);
    }
}
