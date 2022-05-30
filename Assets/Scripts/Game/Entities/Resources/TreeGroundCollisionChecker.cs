using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TreeGroundCollisionChecker : MonoBehaviour
{
    [SerializeField] private Tree tree;
    
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision?.gameObject?.name);
        if (!collision.gameObject.CompareTag("Terrain")) return;
        tree.HandleTreeGrounded();
        this.GameObject().SetActive(false);
    }
}
