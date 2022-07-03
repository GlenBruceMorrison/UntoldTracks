using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UntoldTracks;
using UntoldTracks.Player;
using UntoldTracks.Models;
using UntoldTracks.Managers;
using UntoldTracks.InventorySystem;
using UntoldTracks.Resource;

public class RockVein : MonoBehaviour, IInteractable
{
    public ResourceHolder resourceHolder;

    [SerializeField] private ItemModel _axeItemReference;

    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private MeshCollider _collider;

    [SerializeField] private ParticleSystem _particleSystemExplode;
    [SerializeField] private ParticleSystem _particleSystemChip;

    [SerializeField] int _health = 3;

    public UnityEvent OnRockPicked, OnRockExplode;
    
    public bool AnyHealth
    {
        get
        {
            return _health > 0;
        }
    }

    #region IInteractable
    [SerializeField] private Sprite _displaySprite;
    private List<InteractionDisplay> _possibleInputs = new();
    public Vector3 InteractionAnchor => transform.position + Vector3.up;
    public event InteractionStateUpdate OnInteractionStateUpdate;


    public List<InteractionDisplay> PossibleInputs
    {
        get
        {
            return _possibleInputs;
        }
    }

    public void HandleInput(PlayerManager manager, InteractionData interaction)
    {
        switch (interaction.Input)
        {
            case InteractionInput.Primary:
                if (manager.InventoryController.ActiveItemContainer.Item != _axeItemReference)
                {
                    return;
                }

                manager.InventoryController.ActiveItemContainer.DecreaseDurability(1);

                if (AnyHealth)
                {
                    _particleSystemChip.transform.LookAt(interaction.Origin);
                    _particleSystemChip.transform.eulerAngles += new Vector3(0, 0, 0);
                    _particleSystemChip.transform.position = interaction.InteractionPoint;
                    
                    HandleRockChipped();
                }
                else
                {
                    HandleRockBroken();
                    HandleInteractionChange();
                    
                    var produced = resourceHolder.Harvest();
                    foreach (var product in produced)
                    {
                        manager.InventoryController.Inventory.Give(product);
                    }
                }
                break;
        }
    }

    public void HandleBecomeFocus(PlayerManager player)
    {
        HandleInteractionChange();
    }

    public void HandleLoseFocus(PlayerManager player)
    {

    }

    public void HandleInteractionChange()
    {
        if (_collider.enabled)
        {
            if (GameManager.Instance.LocalPlayer.InventoryController.ActiveItemContainer.Item == _axeItemReference)
            {
                _possibleInputs = new List<InteractionDisplay>()
            {
                new InteractionDisplay(InteractionInput.Primary, "Mine")
            };

                OnInteractionStateUpdate?.Invoke();
                return;
            }
        }

        _possibleInputs = new();
        OnInteractionStateUpdate?.Invoke();
    }

    #endregion

    private void HandleRockChipped()
    {
        _particleSystemChip.Play();
        _health -= 1;
        OnRockPicked?.Invoke();
    }

    public void HandleRockBroken()
    {
        _particleSystemExplode.Play();
        _collider.enabled = false;
        _renderer.enabled = false;
        OnRockExplode?.Invoke();
    }
}
