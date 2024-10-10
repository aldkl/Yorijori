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

        // 중복 재료를 제거하기 위한 HashSet 생성
        HashSet<Ingredient> uniqueIngredients = new HashSet<Ingredient>(ingredients);

        // `FinishRecipe` 초기화
        FinishRecipe = null;

        // GameManager의 모든 레시피와 비교
        foreach (Recipe recipe in GameManager.Instance.Recipes)
        {
            // 레시피의 재료 수와 uniqueIngredients의 수가 다르면 다음 레시피로 넘어갑니다.

            bool isMatching = true;

            // 레시피의 각 재료에 대해 비교를 수행
            foreach (Ingredient recipeIngredient in recipe.ingredients)
            {

                // 이름이 일치하는 재료를 찾습니다.
                Ingredient matchingIngredient = uniqueIngredients.FirstOrDefault(i => i.Name.Trim().Equals(recipeIngredient.Name.Trim(), StringComparison.OrdinalIgnoreCase));

                // 일치하는 재료가 없거나, isFailed가 true인 경우 매칭 실패로 처리
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
            Debug.Log("요리가 완성되었습니다: " + FinishRecipe.name);

            foreach (GameObject cook in Cooks)
            {
                if (cook.name == FinishRecipe.name)
                {
                    cook.SetActive(true);
                    IsFinished = true;
                    GameManager.Instance.PlayEffectSound(6);
                    break; // 일치하는 요리를 찾으면 반복을 멈춥니다.
                }
            }
        }
        else
        {
            Debug.Log("일치하는 레시피를 찾을 수 없습니다.");
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
