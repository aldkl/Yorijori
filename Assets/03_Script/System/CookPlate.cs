using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CookPlate : MonoBehaviour
{


    public List<Ingredient> ingredients;

    public GameObject OnCook;

    public List<GameObject> Cooks;


    bool IsFinished = false;
    public Recipe FinishRecipe;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "MixBowl" || collision.gameObject.tag == "Ladle" || collision.gameObject.tag == "Fryingpan")
        {
            if (!OnCook.activeSelf && !IsFinished)
            {
                if(collision.gameObject.GetComponent<CookingTools>().InputIngredients.Count > 0)
                {
                    Debug.Log("CookOn");
                    ingredients = new List<Ingredient>(collision.gameObject.GetComponent<CookingTools>().InputIngredients);
                    collision.gameObject.GetComponent<CookingTools>().InputIngredients.Clear();
                    collision.gameObject.GetComponent<CookingTools>().ShowNowCooking.SetActive(false);
                    collision.gameObject.GetComponent<CookingTools>().STopedCoru();
                    OnCook.SetActive(true);
                }
            }
        }
        if (collision.gameObject.CompareTag("Ingredient") && collision.gameObject.GetComponent<Ingredient>().Name == "Fairywings")
        {
            ingredients = new List<Ingredient> { collision.gameObject.GetComponent<Ingredient>() };
            collision.gameObject.SetActive(false);
        }

            if (collision.gameObject.tag == "Magic" && ingredients != null && !IsFinished)
        {
            FinishCooked();
        }
    }
    public void FinishCooked()
    {
        OnCook.SetActive(false);

        // �ߺ� ��Ḧ �����ϱ� ���� HashSet ����
        HashSet<Ingredient> uniqueIngredients = new HashSet<Ingredient>(ingredients);

        // `FinishRecipe` �ʱ�ȭ
        FinishRecipe = null;

        // GameManager�� ��� �����ǿ� ��
        foreach (Recipe recipe in GameManager.Instance.Recipes)
        {
            // �������� ��� ���� uniqueIngredients�� ���� �ٸ��� ���� �����Ƿ� �Ѿ�ϴ�.

            bool isMatching = true;

            // �������� �� ��ῡ ���� �񱳸� ����
            foreach (Ingredient recipeIngredient in recipe.ingredients)
            {

                // �̸��� ��ġ�ϴ� ��Ḧ ã���ϴ�.
                Ingredient matchingIngredient = uniqueIngredients.FirstOrDefault(i => i.Name.Trim().Equals(recipeIngredient.Name.Trim(), StringComparison.OrdinalIgnoreCase));

                // ��ġ�ϴ� ��ᰡ ���ų�, isFailed�� true�� ��� ��Ī ���з� ó��
                if (matchingIngredient == null || matchingIngredient.isFailed)
                {
                    isMatching = false;
                    break;
                }
            }
            if (isMatching)
            {
                FinishRecipe = recipe;
                IsFinished = true;
                break;
            }
        }

        if (FinishRecipe != null)
        {
            Debug.Log("�丮�� �ϼ��Ǿ����ϴ�: " + FinishRecipe.name);

            foreach (GameObject cook in Cooks)
            {
                if (cook.name == FinishRecipe.name)
                {
                    cook.SetActive(true);
                    IsFinished = true;
                    GameManager.Instance.PlayEffectSound(6);
                    break; // ��ġ�ϴ� �丮�� ã���� �ݺ��� ����ϴ�.
                }
            }
        }
        else
        {
            Debug.Log("��ġ�ϴ� �����Ǹ� ã�� �� �����ϴ�.");
            GameManager.Instance.PlayEffectSound(7);
            Cooks[0].SetActive(true);
            IsFinished = true;
        }
    }

    public Recipe GiveRecipe()
    {
        if (FinishRecipe != null)
        {
            return FinishRecipe;
        }
        else
        {
            return null;
        }
    }
    void Start()
    {
        ingredients = null;
    }
}
