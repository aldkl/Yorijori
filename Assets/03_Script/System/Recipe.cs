using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Recipe : MonoBehaviour
{
    public string Name;//요리 이름
    public List<Ingredient> ingredients;

    public Recipe(string _name, List<Ingredient> _ingredients)
    {
        Name = _name;
        ingredients = _ingredients;
    }
    public bool IsUnlocked { get; private set; }

    public void UnlockRecipe()
    {
        IsUnlocked = true;
    }
}
