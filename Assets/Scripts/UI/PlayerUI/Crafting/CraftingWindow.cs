using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UntoldTracks.Inventory;
using UntoldTracks.Player;

namespace UntoldTracks.UI
{
    public class CraftingWindow : MonoBehaviour
    {
        public PlayerManagerUI playerUI;
        
        public Image targetItemSprite;
        public TMP_Text targetItemName;
        public TMP_Text targetItemDescription;

        public Transform ingredientsContainer;

        public RecipeRequirement requirementPrefab;
        public List<RecipeRequirement> ingredients = new List<RecipeRequirement>();

        private void Awake()
        {
            
        }

        public void Render(Recipe recipe, IInventory inventory)
        {
            foreach (var ingredient in ingredients)
            {
                Destroy(ingredient.gameObject);
            }

            ingredients = new List<RecipeRequirement>();

            targetItemSprite.sprite = recipe.produces.Item.sprite;
            targetItemName.text = recipe.produces.Item.name;
            targetItemDescription.text = recipe.produces.Item.description;

            foreach (var ingredient in recipe.ingredients)
            {
                var ingredientObject = Instantiate(requirementPrefab, ingredientsContainer.transform);

                ingredients.Add(ingredientObject);

                ingredientObject.Render(ingredient, inventory);
            }
        }
    }
}
