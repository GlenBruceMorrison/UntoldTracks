using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UntoldTracks;
using UntoldTracks.Player;
using UntoldTracks.Models;
using UntoldTracks.Managers;
using Random = UnityEngine.Random;

public class Tree : MonoBehaviour, IInteractable
{
    public int health=5;

    [SerializeField] private ItemModel _axeItemReference;

    public GameObject tree;
    public List<GameObject> logs = new();

    public bool grounded = false;

    [SerializeField] private Sprite _displaySprite;
    [SerializeField] private List<InteractionDisplay> _possibleInputs = new();
    [SerializeField] private TreeGroundCollisionChecker _treeGroundCollisionChecker;

    [SerializeField] private TreeState _currentState;

    [SerializeField] private Transform _chipParticles;

    public Vector3 InteractionAnchor => transform.position + Vector3.up;
    private Rigidbody _rigidbody;
    private Collider _collider;

    public enum TreeState
    {
        Idle,
        Falling,
        Grounded
    }

    public UnityEvent OnFalling, OnGrounded, OnHit;


    public event InteractionStateUpdate OnInteractionStateUpdate;

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

        transform.localEulerAngles = new Vector3(0, Random.Range(0, 360), 0.5f);
    }

    public void HandleInput(PlayerManager manager, InteractionData interaction)
    {
        switch (interaction.Input)
        {
            case InteractionInput.Primary:
                switch (_currentState)
                {
                    case TreeState.Idle:
                        if (manager.InventoryController.ActiveItemContainer.Item != _axeItemReference)
                        {
                            return;
                        }
                        _chipParticles.transform.LookAt(interaction.Origin);
                        _chipParticles.eulerAngles += new Vector3(0, -90, 0);
                        _chipParticles.transform.position = interaction.InteractionPoint;

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
                throw new ArgumentOutOfRangeException(nameof(interaction.Input), interaction.Input, null);
        }
    }

    private void HandleTreeChopped()
    {
        _rigidbody.isKinematic = false;
        OnFalling?.Invoke();
        _treeGroundCollisionChecker.enabled = true;
        HandleInteractionChange();
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

    public void HandleBecomeFocus(PlayerManager player)
    {
        HandleInteractionChange();
    }

    public void HandleInteractionChange()
    {
        if (GameManager.Instance.LocalPlayer.InventoryController.ActiveItemContainer.Item == _axeItemReference)
        {
            if (health > 0)
            {
                _possibleInputs = new List<InteractionDisplay>()
                {
                    new InteractionDisplay(InteractionInput.Primary, "Chop")
                };
                OnInteractionStateUpdate?.Invoke();
                return;
            }
        }

        _possibleInputs = new();
        OnInteractionStateUpdate?.Invoke();
    }

    public void HandleLoseFocus(PlayerManager player) { }

}
