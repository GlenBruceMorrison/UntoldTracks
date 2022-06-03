using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UntoldTracks;
using UntoldTracks.Player;
using UntoldTracks.Models;
using UntoldTracks.Managers;

public class Tree : MonoBehaviour, IInteractable
{
    public int health=5;

    [SerializeField] private ItemModel _axeItemReference;

    public GameObject tree;
    public List<GameObject> logs = new List<GameObject>();

    public bool grounded = false;

    [SerializeField] private Sprite _displaySprite;
    [SerializeField] private List<InteractionDisplay> _possibleInputs = new List<InteractionDisplay>();
    [SerializeField] private TreeGroundCollisionChecker _treeGroundCollisionChecker;

    [SerializeField] private TreeState _currentState;

    private Rigidbody _rigidbody;
    private Collider _collider;

    public enum TreeState
    {
        Idle,
        Falling,
        Grounded
    }

    public UnityEvent OnFalling, OnGrounded, OnHit;
    
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

    private void Awake()
    {
        _treeGroundCollisionChecker = GetComponentInChildren<TreeGroundCollisionChecker>();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _currentState = TreeState.Idle;

        _treeGroundCollisionChecker.enabled = false;
    }

    public void HandleInput(PlayerManager manager, InteractionInput input)
    {
        switch (input)
        {
            case InteractionInput.Primary:
                switch (_currentState)
                {
                    case TreeState.Idle:
                        if (manager.InventoryController.ActiveItemContainer.Item != _axeItemReference)
                        {
                            return;
                        }

                        if (health > 0)
                        {
                            OnHit?.Invoke();
                            health -= 1;
                        }
                        else
                        {
                            OnHit?.Invoke();
                            HandleTreeChopped();
                            _currentState = TreeState.Falling;
                        }

                        manager.InventoryController.ActiveItemContainer.DecreaseDurability(1);
                        break;
                    case TreeState.Falling:

                        break;
                    case TreeState.Grounded:

                        break;
                }
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
        _rigidbody.isKinematic = false;
        OnFalling?.Invoke();
        _treeGroundCollisionChecker.enabled = true;
    }

    public void HandleTreeGrounded()
    {
        _collider.enabled = false;
        grounded = true;
        
        tree.SetActive(false);
        
        foreach (var log in logs)
        {
            log.SetActive(true);
            log.transform.parent = null;
        }
        
        OnGrounded?.Invoke();
        _currentState = TreeState.Grounded;
    }

    public void HandleBecomeFocus(PlayerManager player) { }
    public void HandleLoseFocus(PlayerManager player) { }

}
