using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks.InventorySystem;
using UntoldTracks.Player;

namespace UntoldTracks.UI
{
    public class CraftingUI : MonoBehaviour
    {
        public RecipeBook recipeBook;

        public Transform recipeContainer;

        public CraftingWindow craftingWindow;

        public CraftingButton craftingButton;

        public List<RecipeSelector> recipeSelectors = new List<RecipeSelector>();
        public RecipeSelector recipeSelectorPrefab;

        public PlayerManager playerManager => GameObject.FindObjectOfType<PlayerManager>();

        public Recipe selectedRecipe;
        
        private void OnEnable()
        {
            playerManager.InventoryController.Inventory.OnModified += HandleInventoryModified;
            
            RenderRecipeBook(recipeBook);

            craftingButton.OnClick += Craft;
        }

        private void OnDisable()
        {
            playerManager.InventoryController.Inventory.OnModified -= HandleInventoryModified;
            
            craftingButton.OnClick -= Craft;
        }

        private void HandleInventoryModified()
        {
            if (selectedRecipe != null)
            {
                ShowRecipe(selectedRecipe); 
            }
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

            playerManager.InventoryController.Inventory.Give(selectedRecipe.produces);

            RenderRecipeBook(recipeBook);
        }

        public void Init(PlayerManager playerManager)
        {
            //this.playerManager = playerManager;
        }

        public void RenderRecipeBook(RecipeBook book)
        {
            foreach (var recipe in recipeSelectors)
            {
                Destroy(recipe.gameObject);
            }

            recipeSelectors = new List<RecipeSelector>();

            foreach (var recipe in book.recipes)
            {
                var newPanel = Instantiate(recipeSelectorPrefab, recipeContainer.transform);
                recipeSelectors.Add(newPanel);

                newPanel.Render(recipe);
            }
        }

        public void ShowRecipe(Recipe recipe)
        {
            selectedRecipe = recipe;
            craftingWindow.Render(recipe, playerManager.InventoryController.Inventory);
        }
    }
}