using UnityEngine;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

namespace UntoldTracks.UI
{
    public class PlayerInteractionControllerUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text txtInteraction;
        [SerializeField] private Image imgInteraction;
        [SerializeField] private Transform pnlTextHolder;

        public List<InteractionInputDisplay> inputDisplays = new List<InteractionInputDisplay>();

        public void DisplayInteractable(IInteractable interactable)
        {
            if (interactable.DisplaySprite != null)
            {
                imgInteraction.sprite = interactable.DisplaySprite;
                imgInteraction.gameObject.SetActive(true);
            }

            ClearAllInput();

            if (interactable.PossibleInputs != null)
            { 
                foreach (var input in interactable.PossibleInputs)
                {
                    var inputDisplay = inputDisplays.FirstOrDefault(x => x.inputType == input.input);
                
                    if (inputDisplay != null)
                    {
                        inputDisplay.gameObject.SetActive(true);
                        inputDisplay.SetText(input.text);
                    }
                }
            }
        }

        public void HideInteractable()
        {
            imgInteraction.sprite = null;

            imgInteraction.gameObject.SetActive(false);

            ClearAllInput();
        }

        public void ClearAllInput()
        {
            foreach(var input in inputDisplays)
            {
                input.ClearText();
                input.gameObject.SetActive(false);
            }
        }
    }
}