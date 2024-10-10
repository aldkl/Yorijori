using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Customer : MonoBehaviour
{
    public Recipe requestedRecipe;


    public string Name { get; private set; }
    private Animator animator;
    public int positionIndex;
    public GameObject MyHand;
    public GameObject UIImageGameObject;

    public bool IsBoss;
    public GameObject BossChange1;
    public GameObject BossChange2;


    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        Transform handTransform = transform.GetChild(0).Find("Bone004");
        UIImageGameObject.SetActive(false);
        if (handTransform != null)
        {
            MyHand = handTransform.gameObject;
        }
        else
        {
            Debug.LogError("Bone004라는 이름의 자식 오브젝트를 찾을 수 없습니다.");
        }
    }

    // 손님을 특정 위치로 이동시키는 함수
    public void CreateCustomer(Vector3 STartPosition, Vector3 targetPosition, int i)
    {
        StartCoroutine(MoveToPosition(STartPosition, targetPosition, true));
        Debug.Log(name + "GOGOGO");
        positionIndex = i;
    }

    public void MoveForwardToPosition(Vector3 targetPosition, int i)
    {
        StartCoroutine(MoveToPosition(transform.position, targetPosition, true));
        positionIndex = i;
    }
    private IEnumerator MoveToPosition(Vector3 startPosition, Vector3 targetPosition, bool isCreating)
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        // Walk1 애니메이션 재생
        animator.Play("Walk1");
        transform.position = startPosition;
        // 이동할 방향 계산
        Vector3 direction = (targetPosition - startPosition).normalized;

        // 회전할 방향을 Y축으로만 고려
        direction.y = 0; // Y축 회전만 고려하기 위해 y를 0으로 설정

        // 현재 회전을 초기화
        Quaternion startRotation = transform.rotation;

        // 목표 회전 계산
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // 이동하는 동안 애니메이션 재생
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            // 방향 업데이트 (y축만 고려)
            direction = (targetPosition - transform.position).normalized;
            direction.y = 0; // Y축 회전만 고려하기 위해 y를 0으로 설정

            // 회전 업데이트
            targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

            // 위치 이동
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 2f);

            yield return null;
        }

        // 최종 위치에 정확히 도착하도록 설정
        transform.position = targetPosition;

        // 위치에 도착한 후
        if (isCreating)
        {
            // 첫 생성 시, 위치에 따라 주문 애니메이션 재생
            if (positionIndex == 0)
            {
                animator.Play("Order1");
                UIImageGameObject.SetActive(true);
                Debug.Log(name + "IS Walk");
                GameManager.Instance.PlayEffectSound(3);

            }
            else
            {
                animator.Play("Order2");
            }
        }
        else
        {
            // 이동 후 도착했을 때, 필요한 다른 애니메이션이나 동작을 추가할 수 있습니다.
        }
    }



    public void ServeRecipe(Recipe recipe)
    {
        Debug.Log("음식 받았습니다");

        // 음식 서빙 후 Eat 애니메이션 재생
        animator.Play("Eat");

        UIImageGameObject.SetActive(false);
        StartCoroutine(CheckAndPlayClapAnimation(recipe));
    }

    private IEnumerator CheckAndPlayClapAnimation(Recipe recipe)
    {
        // Eat 애니메이션이 끝날 때까지 대기
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        if (recipe != null)
        {
            if (requestedRecipe.Name == recipe.Name)
            {
                GameManager.Instance.PlayEffectSound(1);
                animator.Play("Clap");
                MyHand.transform.GetChild(0).gameObject.SetActive(false);
                Debug.Log(name + "IS Clap");

                GameManager.Instance.GoodServeCoin();
            }
            else
            {

                GameManager.Instance.PlayEffectSound(2);
            }
        }
        else
        {
            GameManager.Instance.RemoveCustomerFromQueue();
            Destroy(gameObject);
        }


        // 애니메이션이 끝난 후 고객 오브젝트 파괴
        yield return null;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + 0.2f);

        if (requestedRecipe.Name == recipe.Name && IsBoss)
        {
            BossChange1.SetActive(false);
            BossChange2.SetActive(true);
            animator = BossChange2.GetComponent<Animator>();
            yield return new WaitForSeconds(0.1f);
            GameManager.Instance.PlayEffectSound(0);
            GameManager.Instance.StartChangeEffect();
            yield return new WaitForSeconds(0.5f);
            animator.Play("BossClear");
            yield return null;
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + 0.2f);
            GameManager.Instance.RemoveCustomerFromQueue();
            Destroy(gameObject);
        }
        else
        {
            GameManager.Instance.RemoveCustomerFromQueue();
            Destroy(gameObject);
        }
    }
}
