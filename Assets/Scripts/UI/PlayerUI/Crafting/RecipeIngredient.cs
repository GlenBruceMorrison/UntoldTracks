using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UntoldTracks.Inventory;

public class RecipeIngredient : MonoBehaviour
{
    public Image ingredientImage;
    public TMP_Text ingredientName;
    public TMP_Text ingredientCount;

    public void Render(ItemContainer container, IInventory source)
    {
        ingredientImage.sprite = container.Item.sprite;
        ingredientName.text = container.Item.name;

        var sourceCount = source.GetItemCount(container.Item);

        ingredientCount.text = $"{sourceCount}/{container.Count}";
    }
}
