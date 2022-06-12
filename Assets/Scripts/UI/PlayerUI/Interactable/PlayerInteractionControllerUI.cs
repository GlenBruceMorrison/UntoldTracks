using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UntoldTracks;

public class PlayerInteractionControllerUI : MonoBehaviour
{
    public Camera _interactionCamera;

    [SerializeField] private Transform pnlTextHolder;

    private bool _showing => pnlTextHolder.gameObject.activeInHierarchy;
    private IInteractable _target;

    public List<InteractionInputDisplay> inputDisplays = new();

    public void MoveToInteraction(IInteractable target)
    {
        ClearAllInput();

        if (target == null)
        {
            _target = null;
            return;
        }

        if (_target != null)
        {
            _target.OnInteractionStateUpdate -= HandleInteractionStateUpdate;
        }

        _target = target;

        _target.OnInteractionStateUpdate += HandleInteractionStateUpdate;

        HandleInteractionStateUpdate();
    }

    public void HandleInteractionStateUpdate()
    {
        ClearAllInput();

        if (_target.PossibleInputs == null)
        {
            return;
        }

        foreach (var input in _target.PossibleInputs)
        {
            var inputDisplay = inputDisplays.FirstOrDefault(x => x.inputType == input.input);

            if (inputDisplay == null)
            {
                break;
            }

            pnlTextHolder.gameObject.SetActive(true);
            inputDisplay.gameObject.SetActive(true);
            inputDisplay.SetText(input.text);
        }
    }

    public void HideInteractable()
    {
        pnlTextHolder.gameObject.SetActive(false);

        ClearAllInput();
    }

    public void ClearAllInput()
    {
        foreach (var input in inputDisplays)
        {
            input.ClearText();
            input.gameObject.SetActive(false);
        }
    }

    public void LateUpdate()
    {
        if (!_showing || _target == null) return;

        transform.position = _target.InteractionAnchor;
        transform.LookAt(transform.position + _interactionCamera.transform.rotation * Vector3.forward, _interactionCamera.transform.rotation * Vector3.up);
    }
}
