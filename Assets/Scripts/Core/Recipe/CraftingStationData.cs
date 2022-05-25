using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldTracks;

[CreateAssetMenu(fileName = "CraftingStation", menuName = "Data/CraftingStation")]
public class CraftingStationData : ScriptableObject
{
    [SerializeField] public Item _item;
    [SerializeField] public CraftingStationEntity _worldObject;
    [SerializeField] public RecipeBook _recipeBook;

    public RecipeBook RecipeBook
    {
        get
        {
            return _recipeBook;
        }
    }
}
