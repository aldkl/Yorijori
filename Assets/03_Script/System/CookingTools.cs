using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CookingTools : MonoBehaviour
{

    public string ToolsName;
    HandAniInput PlayerHandAni;

    public List<Ingredient> InputIngredients;
    public GameObject ShowNowCooking;

    public Transform Grabtransform;
    void Start()
    {
        PlayerHandAni = GameManager.Instance.GetPlayerRightHand().GetComponent<HandAniInput>();
        InputIngredients = new List<Ingredient>();
        if (ShowNowCooking != null)
        {
            ShowNowCooking.SetActive(false);
        }
    }

    public void SetHandAni()
    {
        PlayerHandAni.PlayHandAni(ToolsName);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ingredient"))
        {
            GameObject ingredient = collision.gameObject;
            switch (ToolsName)
            {
                case "Bowl":
                    BowlInput(ingredient);
                    break;

                case "Pot":
                    PotInput(ingredient);

                    break;

                case "Cutting_board":

                    break;

                case "FryingPan":
                    if(ingredient.GetComponent<Ingredient>().isCutting && !ShowNowCooking.activeSelf)
                    {
                        PanInput(ingredient);
                    }
                    break;

                case "Knife":
                    KnifeCutting(ingredient);
                    break;

                case "Ladle":

                    break;

                case "Mixing_ball":

                    break;

                case "PepperShaker":

                    break;

                case "spatula":

                    break;

                case "Mixer":

                    break;

            }
        }
        if (collision.gameObject.tag == "Ladle" && ToolsName == "Pot")
        {
            if(InputIngredients.Count > 0)
            {
                collision.gameObject.GetComponent<CookingTools>().InputIngredients = new List<Ingredient>(InputIngredients);
                //Debug.Log(collision.gameObject.GetComponent<CookingTools>().InputIngredients);
                collision.gameObject.GetComponent<CookingTools>().ShowNowCooking.SetActive(true);

                InputIngredients.Clear();
                ShowNowCooking.SetActive(false);
            }
        }

        if (collision.gameObject.tag == "MixBowl" && ToolsName == "Mixer" && collision.gameObject.GetComponent<CookingTools>().InputIngredients != null)
        {
            MixerOnair(collision);
        }
    }




    public void MixerOnair(Collision collision)
    {
        collision.transform.position = Grabtransform.position;
        collision.transform.rotation = Grabtransform.rotation;

        XRGrabInteractable grabInteractable = collision.gameObject.GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.enabled = false;// 비활성화

            // 코루틴 시작
            StartCoroutine(ReenableXRGrab(collision.gameObject, 5.0f)); // 2초 뒤에 활성화
        }
    }


    // 코루틴을 통해 일정 시간 후 XRGrabInteractable을 다시 활성화
    private IEnumerator ReenableXRGrab(GameObject grabInteractable, float delay)
    {
        yield return new WaitForSeconds(delay); // 지정된 시간만큼 대기


        for (int i = 0; i <= grabInteractable.GetComponent<CookingTools>().InputIngredients.Count; i++)
        {
            grabInteractable.GetComponent<CookingTools>().InputIngredients[i].isMixed = true;
        }


        grabInteractable.GetComponent<XRGrabInteractable>().enabled = true; // XRGrabInteractable 컴포넌트 다시 활성화
    }



    public void BowlInput(GameObject ingredient)
    {
        Ingredient ingredientComponent = ingredient.gameObject.GetComponent<Ingredient>();
        InputIngredient(ingredientComponent);


    }

    // 변경할 메테리얼을 public으로 받아옴
    public Material cookedMaterial; // 익은 상태의 메테리얼
    public GameObject burnedMaterial; // 타버린 상태의 메테리얼

    private Coroutine cookingCoroutine; // 현재 실행 중인 코루틴을 저장할 변수

    public void STopedCoru()
    {
        if (cookingCoroutine != null)
        {
            StopCoroutine(cookingCoroutine);
        }
    }
    public void PanInput(GameObject ingredient)
    {
        Ingredient ingredientComponent = ingredient.GetComponent<Ingredient>();
        InputIngredient(ingredientComponent);

        // 코루틴이 이미 실행 중인 경우, 해당 코루틴을 중지
        if (cookingCoroutine != null)
        {
            StopCoroutine(cookingCoroutine);
        }

        // 재료의 익는 과정을 시작하는 코루틴을 실행
        cookingCoroutine = StartCoroutine(CookIngredient(ShowNowCooking));
    }
    public void PotInput(GameObject ingredient)
    {
        Ingredient ingredientComponent = ingredient.GetComponent<Ingredient>();
        InputIngredient(ingredientComponent);

        // 코루틴이 이미 실행 중인 경우, 해당 코루틴을 중지
        if (cookingCoroutine != null)
        {
            StopCoroutine(cookingCoroutine);
        }

        // 재료의 익는 과정을 시작하는 코루틴을 실행
        cookingCoroutine = StartCoroutine(CookIngredient(ShowNowCooking));
    }

    private IEnumerator CookIngredient(GameObject Cooking)
    {
        // 1단계: 재료가 익는 시간 (예: 5초)
        yield return new WaitForSeconds(5f);

        // 익은 상태로 메테리얼 변경
        Renderer renderer = Cooking.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = cookedMaterial;
            for(int i = 0; i < InputIngredients.Count; i++) 
            {
                InputIngredients[i].isFired = true;
            }
        }

        // 2단계: 재료가 타는 시간 (예: 5초)
        yield return new WaitForSeconds(5f);

        // 타버린 상태로 메테리얼 변경
        if (renderer != null)
        {
            burnedMaterial.SetActive(true);
            for (int i = 0; i < InputIngredients.Count; i++)
            {
                InputIngredients[i].isFailed = true;
            }
        }

        // 코루틴 완료 후 변수 초기화
        cookingCoroutine = null;
    }



    public void InputIngredient(Ingredient ingredientComponent)
    {
        if (ingredientComponent != null)
        {
            InputIngredients.Add(ingredientComponent);
            ingredientComponent.gameObject.SetActive(false);
            //Destroy(ingredientComponent.gameObject);
            ShowNowCooking.SetActive(true);
        }
    }
    public void KnifeCutting(GameObject ingredient)
    {
        if (ingredient.GetComponent<Ingredient>().isCutting == false)
        {
            Ingredient ingredientComponent = ingredient.gameObject.GetComponent<Ingredient>();

            string ingredientName = ingredientComponent.Name;

            string[] excludedNames = { "Coral", "Seashell", "Eyeball", "Snail", "Moss", "Fairywings" };

            if (System.Array.IndexOf(excludedNames, ingredientName) == -1)
            {
                // CuttingObject 새로 생성
                GameObject newCuttingObject1 = Instantiate(ingredientComponent.CuttingObject1, ingredient.transform.position + new Vector3(0.3f, 0, 0), Quaternion.identity);
                GameObject newCuttingObject2 = Instantiate(ingredientComponent.CuttingObject2, ingredient.transform.position + new Vector3(0, 0, 0), Quaternion.identity);
                GameObject newCuttingObject3 = Instantiate(ingredientComponent.CuttingObject3, ingredient.transform.position + new Vector3(-0.3f, 0, 0), Quaternion.identity);

                GameManager.Instance.PlayEffectSound(4);
                // 기존 ingredientComponent 오브젝트 삭제
                Destroy(ingredientComponent.gameObject);
            }

        }
    }





}
