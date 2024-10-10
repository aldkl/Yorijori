using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientBox : MonoBehaviour
{
    public Ingredient CurIngredient;
    

    public void CreateIngredient()
    {
        Instantiate(CurIngredient, transform.position + new Vector3(0, 0.2f,0), new Quaternion(0,0,0,0));
    }
}
