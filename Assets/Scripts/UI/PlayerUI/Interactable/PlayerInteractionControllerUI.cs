using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

namespace UntoldTracks.UI
{
    public class PlayerInteractionControllerUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text txtInteraction;

        [SerializeField]
        private Image imgInteraction;

        [SerializeField]
        private Transform pnlTextHolder;

        public void DisplayInteractable(IInteractable interactable)
        {
            if (interactable.DisplaySprite != null)
            {
                imgInteraction.sprite = interactable.DisplaySprite;
                imgInteraction.gameObject.SetActive(true);
            }

            if (!string.IsNullOrWhiteSpace(interactable.DisplayText))
            {
                txtInteraction.text = interactable.DisplayText;

                pnlTextHolder.gameObject.SetActive(true);
                txtInteraction.gameObject.SetActive(true);
            }
        }

        public void HideInteractable()
        {
            txtInteraction.text = "";
            imgInteraction.sprite = null;

            pnlTextHolder.gameObject.SetActive(false);
            txtInteraction.gameObject.SetActive(false);
            imgInteraction.gameObject.SetActive(false);
        }
    }
}