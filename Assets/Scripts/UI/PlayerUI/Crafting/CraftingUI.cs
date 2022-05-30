using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks.InventorySystem;
using UntoldTracks.Player;

namespace UntoldTracks.UI
{
    public class CraftingUI : MonoBehaviour
    {
        [SerializeField] private RecipeBook baseRecipeBook;
        [SerializeField] private RecipeBook overrideRecipeBook;

        private RecipeBook CurrentRecipeBook
        {
            get
            {
                return overrideRecipeBook == null ? baseRecipeBook: overrideRecipeBook;
            }
        }

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
            RenderRecipeBook();
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

        public void SetCraftingBook(RecipeBook recipeBook)
        {
            if (recipeBook != null)
            {
                overrideRecipeBook = recipeBook;
            }
            else
            {
                overrideRecipeBook = null;
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

            foreach (var recipe in CurrentRecipeBook.recipes)
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