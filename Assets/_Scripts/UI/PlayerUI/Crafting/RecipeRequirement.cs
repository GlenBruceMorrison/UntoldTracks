using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UntoldTracks.InventorySystem;

namespace UntoldTracks.UI
{
    public class RecipeRequirement : MonoBehaviour
    {
        public Color colourEnough;
        public Color colourNotEnough;

        public Image backdrop;
        public Image ingredientImage;
        public TMP_Text ingredientCount;
        public TMP_Text ingredientName;

        public void Render(ItemContainer container, Inventory source)
        {
            ingredientImage.sprite = container.Item.sprite;
            var sourceCount = source.GetItemCount(container.Item);

            ingredientName.text = container.Item.displayName;
            ingredientCount.text = $"{sourceCount}/{container.Count}";

            backdrop.color = sourceCount > container.Count ? colourEnough : colourNotEnough;
        }
    }
}
