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
            grabInteractable.enabled = false;// ��Ȱ��ȭ

            // �ڷ�ƾ ����
            StartCoroutine(ReenableXRGrab(collision.gameObject, 5.0f)); // 2�� �ڿ� Ȱ��ȭ
        }
    }


    // �ڷ�ƾ�� ���� ���� �ð� �� XRGrabInteractable�� �ٽ� Ȱ��ȭ
    private IEnumerator ReenableXRGrab(GameObject grabInteractable, float delay)
    {
        yield return new WaitForSeconds(delay); // ������ �ð���ŭ ���


        for (int i = 0; i <= grabInteractable.GetComponent<CookingTools>().InputIngredients.Count; i++)
        {
            grabInteractable.GetComponent<CookingTools>().InputIngredients[i].isMixed = true;
        }


        grabInteractable.GetComponent<XRGrabInteractable>().enabled = true; // XRGrabInteractable ������Ʈ �ٽ� Ȱ��ȭ
    }



    public void BowlInput(GameObject ingredient)
    {
        Ingredient ingredientComponent = ingredient.gameObject.GetComponent<Ingredient>();
        InputIngredient(ingredientComponent);


    }

    // ������ ���׸����� public���� �޾ƿ�
    public Material cookedMaterial; // ���� ������ ���׸���
    public GameObject burnedMaterial; // Ÿ���� ������ ���׸���

    private Coroutine cookingCoroutine; // ���� ���� ���� �ڷ�ƾ�� ������ ����

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

        // �ڷ�ƾ�� �̹� ���� ���� ���, �ش� �ڷ�ƾ�� ����
        if (cookingCoroutine != null)
        {
            StopCoroutine(cookingCoroutine);
        }

        // ����� �ʹ� ������ �����ϴ� �ڷ�ƾ�� ����
        cookingCoroutine = StartCoroutine(CookIngredient(ShowNowCooking));
    }
    public void PotInput(GameObject ingredient)
    {
        Ingredient ingredientComponent = ingredient.GetComponent<Ingredient>();
        InputIngredient(ingredientComponent);

        // �ڷ�ƾ�� �̹� ���� ���� ���, �ش� �ڷ�ƾ�� ����
        if (cookingCoroutine != null)
        {
            StopCoroutine(cookingCoroutine);
        }

        // ����� �ʹ� ������ �����ϴ� �ڷ�ƾ�� ����
        cookingCoroutine = StartCoroutine(CookIngredient(ShowNowCooking));
    }

    private IEnumerator CookIngredient(GameObject Cooking)
    {
        // 1�ܰ�: ��ᰡ �ʹ� �ð� (��: 5��)
        yield return new WaitForSeconds(5f);

        // ���� ���·� ���׸��� ����
        Renderer renderer = Cooking.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = cookedMaterial;
            for(int i = 0; i < InputIngredients.Count; i++) 
            {
                InputIngredients[i].isFired = true;
            }
        }

        // 2�ܰ�: ��ᰡ Ÿ�� �ð� (��: 5��)
        yield return new WaitForSeconds(5f);

        // Ÿ���� ���·� ���׸��� ����
        if (renderer != null)
        {
            burnedMaterial.SetActive(true);
            for (int i = 0; i < InputIngredients.Count; i++)
            {
                InputIngredients[i].isFailed = true;
            }
        }

        // �ڷ�ƾ �Ϸ� �� ���� �ʱ�ȭ
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
                // CuttingObject ���� ����
                GameObject newCuttingObject1 = Instantiate(ingredientComponent.CuttingObject1, ingredient.transform.position + new Vector3(0.3f, 0, 0), Quaternion.identity);
                GameObject newCuttingObject2 = Instantiate(ingredientComponent.CuttingObject2, ingredient.transform.position + new Vector3(0, 0, 0), Quaternion.identity);
                GameObject newCuttingObject3 = Instantiate(ingredientComponent.CuttingObject3, ingredient.transform.position + new Vector3(-0.3f, 0, 0), Quaternion.identity);

                GameManager.Instance.PlayEffectSound(4);
                // ���� ingredientComponent ������Ʈ ����
                Destroy(ingredientComponent.gameObject);
            }

        }
    }





}
