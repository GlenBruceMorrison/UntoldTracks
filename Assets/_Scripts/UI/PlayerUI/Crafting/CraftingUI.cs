using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks.InventorySystem;
using UntoldTracks.Player;
using UntoldTracks.Models;
using UntoldTracks.Managers;

namespace UntoldTracks.UI
{
    public class CraftingUI : MonoBehaviour
    {
        [SerializeField] private List<Recipe> baseRecipes = new();
        [SerializeField] private List<Recipe> overrideRecipes = new();

        private List<Recipe> CurrentRecipes
        {
            get
            {
                return overrideRecipes == null ? baseRecipes : overrideRecipes;
            }
        }

        public Transform recipeContainer;

        public CraftingWindow craftingWindow;

        public CraftingButton craftingButton;

        public List<RecipeSelector> recipeSelectors = new List<RecipeSelector>();
        public RecipeSelector recipeSelectorPrefab;

        private PlayerManager playerManager;

        public Recipe selectedRecipe;
        
        
        public void Init(PlayerManager manager)
        {
            playerManager = manager;
            playerManager.InventoryController.Inventory.OnModified += HandleInventoryModified;
            RenderRecipeBook();
        }

        private void OnDisable()
        {
            //playerManager.InventoryController.Inventory.OnModified -= HandleInventoryModified;
        }

        private void HandleInventoryModified()
        {
            if (selectedRecipe != null)
            {
                ShowRecipe(selectedRecipe); 
            }
        }

        public void SetCraftingBook(List<Recipe> recipes)
        {
            if (recipes != null)
            {
                overrideRecipes = recipes;
            }
            else
            {
                overrideRecipes = null;
            }

            RenderRecipeBook();
        }

        public void Craft()
        {
            if (selectedRecipe == null)
            {
                return;
            }

            if (!selectedRecipe.CanCreate(playerManager.InventoryController.Inventory))
            {
                return;
            }

            foreach (var ingredient in selectedRecipe.ingredients)
            {
                playerManager.InventoryController.Inventory.Take(new ItemQuery(ingredient.Item, ingredient.Count));
            }


            var container = new ItemContainer(
                selectedRecipe.produces.Item,
                selectedRecipe.produces.Count);

            container.SetDurability(selectedRecipe.produces.Item.durability);

            playerManager.InventoryController.Inventory.Give(container);

            RenderRecipeBook();
        }

        public void RenderRecipeBook()
        {
            foreach (var recipe in recipeSelectors)
            {
                Destroy(recipe.gameObject);
            }

            recipeSelectors = new List<RecipeSelector>();

            foreach (var recipe in CurrentRecipes)
            {
                var newPanel = Instantiate(recipeSelectorPrefab, recipeContainer.transform);
                recipeSelectors.Add(newPanel);

                newPanel.Render(recipe);
            }
        }

        public void ClearRecipe()
        {
            selectedRecipe = null;
            craftingWindow.Render(null, playerManager.InventoryController.Inventory);
        }

        public void ShowRecipe(Recipe recipe)
        {
            selectedRecipe = recipe;
            craftingWindow.Render(recipe, playerManager.InventoryController.Inventory);
        }
    }
}