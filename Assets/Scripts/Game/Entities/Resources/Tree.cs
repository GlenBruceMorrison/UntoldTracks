using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UntoldTracks;
using UntoldTracks.Player;

public class Tree : MonoBehaviour, IInteractable
{
    public GameObject tree;
    public List<GameObject> logs = new List<GameObject>();
    public bool grounded = false;

    [SerializeField] public Sprite _displaySprite;
    [SerializeField] public List<InteractionDisplay> _possibleInputs = new List<InteractionDisplay>();
    [SerializeField] public Rigidbody _rigidbody;

    public UnityEvent OnChopped, OnGrounded;
    
    public Sprite DisplaySprite
    {
        get
        {
            return _displaySprite;
        }
    }

    public List<InteractionDisplay> PossibleInputs
    {
        get
        {
            return _possibleInputs;
        }
    }

    public void HandleInput(PlayerManager manager, InteractionInput input)
    {
        switch (input)
        {
            case InteractionInput.Primary:
                HandleTreeChopped();
                break;
            case InteractionInput.Secondary:
                // no function required
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(input), input, null);
        }
    }

    private void HandleTreeChopped()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        OnChopped?.Invoke();
    }

    public void HandleTreeGrounded()
    {
        GetComponent<Collider>().enabled = false;
        grounded = true;
        
        tree.SetActive(false);
        
        foreach (var log in logs)
        {
            log.SetActive(true);
            var body = log.GetComponent<Rigidbody>();
            body.isKinematic = false;
        }
        
        OnGrounded?.Invoke();
    }

    public void HandleBecomeFocus(PlayerManager player) { }
    public void HandleLoseFocus(PlayerManager player) { }
}
