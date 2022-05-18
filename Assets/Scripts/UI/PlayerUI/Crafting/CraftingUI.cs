using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks.Inventory;
using UntoldTracks.Player;

namespace UntoldTracks.UI
{
    public class CraftingUI : MonoBehaviour
    {
        public Transform recipeContainer;

        public CraftingWindow craftingWindow;

        public CraftingButton craftingButton;

        public List<RecipeSelector> recipeSelectors = new List<RecipeSelector>();
        public RecipeSelector recipeSelectorPrefab;

        public List<Recipe> currentRecipes = new List<Recipe>();

        public PlayerManager playerManager => GameObject.FindObjectOfType<PlayerManager>();

        public Recipe selectedRecipe;
        
        private void OnEnable()
        {
            playerManager.inventoryController.Inventory.OnContainerModified +=  (container => ShowRecipe(selectedRecipe));
            
            RenderRecipeBook(currentRecipes);

            craftingButton.OnClick += Craft;
        }

        private void OnDisable()
        {
            craftingButton.OnClick -= Craft;
        }

        public void Craft()
        {
            if (selectedRecipe == null)
            {
                return;
            }

            if (!selectedRecipe.CanCreate(playerManager.inventoryController.Inventory))
            {
                return;
            }

            foreach (var ingredient in selectedRecipe.ingredients)
            {
                playerManager.inventoryController.Inventory.TakeAndReturnRemaining(ingredient.Item, ingredient.Count);
            }

            playerManager.inventoryController.Inventory.FillAndReturnRemaining(selectedRecipe.produces.Item, selectedRecipe.produces.Count);

            RenderRecipeBook(currentRecipes);
        }

        public void Init(PlayerManager playerManager)
        {
            //this.playerManager = playerManager;
        }

        public void RenderRecipeBook(List<Recipe> recipes)
        {
            foreach (var recipe in recipeSelectors)
            {
                Destroy(recipe.gameObject);
            }

            recipeSelectors = new List<RecipeSelector>();

            foreach (var recipe in recipes)
            {
                var newPanel = Instantiate(recipeSelectorPrefab, recipeContainer.transform);
                recipeSelectors.Add(newPanel);

                newPanel.Render(recipe);
            }
        }

        public void ShowRecipe(Recipe recipe)
        {
            selectedRecipe = recipe;
            craftingWindow.Render(recipe, playerManager.inventoryController.Inventory);
        }
    }
}