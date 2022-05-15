using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UntoldTracks.Inventory;

namespace UntoldTracks.UI
{
    public class CraftingWindow : MonoBehaviour
    {
        public Image targetItemSprite;
        public TMP_Text targetItemName;
        public TMP_Text targetItemDescription;

        public Transform ingredientsContainer;

        public RecipeIngredient ingredientPrefab;
        public List<RecipeIngredient> ingredients = new List<RecipeIngredient>();

        public void Render(Recipe recipe, IInventory inventory)
        {
            foreach (var ingredient in ingredients)
            {
                Destroy(ingredient.gameObject);
            }

            ingredients = new List<RecipeIngredient>();

            targetItemSprite.sprite = recipe.produces.Item.sprite;
            targetItemName.text = recipe.produces.Item.name;
            targetItemDescription.text = recipe.produces.Item.description;

            foreach (var ingredient in recipe.ingredients)
            {
                var ingredientObject = Instantiate(ingredientPrefab, ingredientsContainer.transform);

                ingredients.Add(ingredientObject);

                ingredientObject.Render(ingredient, inventory);
            }
        }
    }
}
