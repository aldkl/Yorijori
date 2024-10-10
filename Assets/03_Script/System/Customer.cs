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
            Debug.LogError("Bone004��� �̸��� �ڽ� ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    // �մ��� Ư�� ��ġ�� �̵���Ű�� �Լ�
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
        // Walk1 �ִϸ��̼� ���
        animator.Play("Walk1");
        transform.position = startPosition;
        // �̵��� ���� ���
        Vector3 direction = (targetPosition - startPosition).normalized;

        // ȸ���� ������ Y�����θ� ���
        direction.y = 0; // Y�� ȸ���� ����ϱ� ���� y�� 0���� ����

        // ���� ȸ���� �ʱ�ȭ
        Quaternion startRotation = transform.rotation;

        // ��ǥ ȸ�� ���
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // �̵��ϴ� ���� �ִϸ��̼� ���
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            // ���� ������Ʈ (y�ุ ���)
            direction = (targetPosition - transform.position).normalized;
            direction.y = 0; // Y�� ȸ���� ����ϱ� ���� y�� 0���� ����

            // ȸ�� ������Ʈ
            targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

            // ��ġ �̵�
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 2f);

            yield return null;
        }

        // ���� ��ġ�� ��Ȯ�� �����ϵ��� ����
        transform.position = targetPosition;

        // ��ġ�� ������ ��
        if (isCreating)
        {
            // ù ���� ��, ��ġ�� ���� �ֹ� �ִϸ��̼� ���
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
            // �̵� �� �������� ��, �ʿ��� �ٸ� �ִϸ��̼��̳� ������ �߰��� �� �ֽ��ϴ�.
        }
    }



    public void ServeRecipe(Recipe recipe)
    {
        Debug.Log("���� �޾ҽ��ϴ�");

        // ���� ���� �� Eat �ִϸ��̼� ���
        animator.Play("Eat");

        UIImageGameObject.SetActive(false);
        StartCoroutine(CheckAndPlayClapAnimation(recipe));
    }

    private IEnumerator CheckAndPlayClapAnimation(Recipe recipe)
    {
        // Eat �ִϸ��̼��� ���� ������ ���
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


        // �ִϸ��̼��� ���� �� �� ������Ʈ �ı�
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
