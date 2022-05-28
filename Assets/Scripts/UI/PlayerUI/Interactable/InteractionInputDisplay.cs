using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UntoldTracks;

public class InteractionInputDisplay : MonoBehaviour
{
    public InteractionInput inputType;
    [SerializeField] private TMP_Text inputText;
    [SerializeField] private Image inputImage;
    
    public void SetText(string text)
    {
        inputText.text = text;
    }

    public void ClearText()
    {
        inputText.text = string.Empty;
    }
}
