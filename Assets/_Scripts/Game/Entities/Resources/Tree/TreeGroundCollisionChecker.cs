using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGroundCollisionChecker : MonoBehaviour
{
    [SerializeField] private Tree tree;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Terrain")) return;
        tree.HandleTreeGrounded();
        gameObject.SetActive(false);
    }
}
