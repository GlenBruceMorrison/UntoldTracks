using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UntoldTracks.Models;

namespace UntoldTracks.UI
{
    public class RecipeSelector : MonoBehaviour, IPointerClickHandler
    {
        public Image sprite;
        public TMP_Text title;

        public Recipe recipe;

        public CraftingUI craftingUI;

        private void Awake()
        {
            craftingUI = GetComponentInParent<CraftingUI>();
        }

        public void Render(Recipe recipe)
        {
            this.recipe = recipe;

            if (recipe == null)
            {
                sprite = null;
                title.text = "";
            }

            sprite.sprite = recipe.produces.Item.sprite;
            title.text = recipe.produces.Item.displayName;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            craftingUI.ShowRecipe(recipe);
        }
    }
}