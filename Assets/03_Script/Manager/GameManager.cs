using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<Recipe> Recipes;
    public List<Ingredient> Ingredients;

    public List<Customer> Day1Customers;
    public List<Customer> Day2Customers;

    public List<IngredientBox> ingredientBoxes;


    public GameObject nowCook;
    public PlayerControl Player;



    public List<Customer> currentDayCustomers;

    public int CurrentDay = 1;
    public int CurrentCustomer = 0;
    public int CurrentRevenue = 0;
    public int TargetRevenue1 = 9; // 목표 수익 설정
    public int TargetRevenue2 = 5; // 목표 수익 설정

    int[,] PlaceIngredient;

    public List<Transform> customerPositions;

    public Animator CoinAnimator;

    public Image Star3;
    public Image Star2;
    public Image Star1;
    public Text CoText;


    public GameObject ChangeEffect;

    public void GoTitleGame()
    {
        Debug.Log("LoadScene");

        LoadingSceneManager.LoadScene("TitleScene");
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        currentDayCustomers = CurrentDay == 1 ? Day1Customers : Day2Customers;
        InitializeGame();
    }

    private void InitializeGame()
    {


        CoText.gameObject.SetActive(false);
        Star3.gameObject.SetActive(false);
        Star2.gameObject.SetActive(false);
        Star1.gameObject.SetActive(false);
        PlaceIngredient = new int[14, 6] {
            { 8, 7, 1, 10, 3, 11},//1
            { 8, 7, 1, 10, 3, 11},//2
            { 8, 7, 1, 2, 6, 15 },//Boss
            { 8, 12, 1, 9, 7, 2 },//3
            { 8, 12, 1, 9, 7, 2 },//4
            { 0, 12, 1, 9, 7, 2 },//5
            { 0, 12, 6, 9, 5, 4 },//Boss2
                                 
            { 9, 8, 1, 7, 2, 12 },//1
            { 9, 8, 1, 7, 2, 12 },//2
            { 0, 5, 13, 7, 2, 12},//Boss
            { 0, 5, 4, 3, 2, 12 },//3
            { 0, 5, 4, 3, 2, 12 },//4
            { 0, 14, 4, 3, 15,13},//5
            { 0, 16, 17,11,15,13},//Boss2
            
            //{ 9, 8, 1, 7, 2, 12 },//1
            //{ 9, 8, 1, 7, 2, 12 },//2
            //{ 0, 5, 13, 7, 2, 12},//Boss
            //{ 0, 5, 4, 3, 2, 12 },//3
            //{ 0, 5, 4, 3, 2, 12 },//4
            //{ 0, 14, 4, 3, 15,13},//5
            //{ 0, 16, 17,11,15,13},//Boss2
        };

        GenerateNewCustomers();
        UpdateIngredientPlacements();
    }


    //손님 생성
    public void GenerateNewCustomers()
    {
        StartCoroutine(GenerateCustomersWithDelay());
    }
    //손님 시간차 생성 코루틴 함수
    private IEnumerator GenerateCustomersWithDelay()
    {
        for (int i = 0; i < 3; i++)
        {
            // 손님을 생성합니다
            currentDayCustomers[i].CreateCustomer(customerPositions[3].position, customerPositions[i].position, i);

            // 랜덤 시간 간격으로 생성 (2초에서 7초 사이)
            float randomDelay = Random.Range(2f, 7f);
            yield return new WaitForSeconds(randomDelay);
        }
    }

    private IEnumerator GenerateCustomersWithDelay2(int n)
    {
        float randomDelay = Random.Range(2f, 7f);
        yield return new WaitForSeconds(randomDelay);
        currentDayCustomers[n].CreateCustomer(customerPositions[3].position, customerPositions[2].position, n);

    }

    //줄 앞당기는 함수
    public void MoveQueueForward()
    {

        // 고객 수에 따라 처리
        int customerCount = currentDayCustomers.Count;

        if (customerCount > 2)
            StartCoroutine(GenerateCustomersWithDelay2(2));

        // 고객들을 앞으로 이동시킵니다
        for (int i = 0; i < 2; i++)
        {
            // 인덱스가 customerPositions의 범위를 초과하지 않도록 검사
            if (i < customerCount)
            {
                Debug.Log(i + "  AA");
                // 고객을 이동시키기 위해 MoveForwardToPosition 호출
                currentDayCustomers[i].MoveForwardToPosition(customerPositions[i].position, i);
            }
        }
        if (currentDayCustomers.Count > 0)
        { 
            CurrentCustomer++;
        }
        UpdateIngredientPlacements();
    }

    // 손님을 줄에서 제거하는 함수
    public void RemoveCustomerFromQueue()
    {
        List<Customer> currentDayCustomers = CurrentDay == 1 ? Day1Customers : Day2Customers;
        currentDayCustomers.Remove(currentDayCustomers[0]);

        // 손님을 제거한 후 줄을 앞으로 이동
        MoveQueueForward();

        if(currentDayCustomers.Count <= 0)
        {
            EndDay();
        }
    }

    public void StartChangeEffect()
    {
        ChangeEffect.SetActive(true);
    }
    private void UpdateIngredientPlacements()
    {
        // 손님에 따른 재료 배치 로직 구현
        for (int i = 0; i < 6; i++)
        {
            ingredientBoxes[i].CurIngredient = Ingredients[PlaceIngredient[CurrentCustomer, i]];
            Debug.Log("ingredientBoxes[i].CurIngredient == " + ingredientBoxes[i].CurIngredient);
        }
    }

    public GameObject GetPlayerLeftHand()
    {
        return Player.LeftHand;
    }
    public void SetPlayerRightAni()
    {
        Player.LeftHand.GetComponent<HandAniInput>().PlayIdle();
    }
    public GameObject GetPlayerRightHand()
    {
        return Player.RightHand;
    }
    public void EndDay()
    {
        CurrentDay++;
        if (CurrentDay > 2)
        {
            EndGame();
        }
        else
        {
            currentDayCustomers = CurrentDay == 1 ? Day1Customers : Day2Customers;
            PlayEffectSound(8);
            // 다음날 손님 리스트 생성
            GenerateNewCustomers();


        }
    }
    public void EndGame()
    {
        if (CurrentRevenue >= TargetRevenue1)
        {
            Star3.gameObject.SetActive(true);
        }
        if(CurrentRevenue >= TargetRevenue2)
        {
            Star2.gameObject.SetActive(true);
        }
        Star1.gameObject.SetActive(true);
        CoText.gameObject.SetActive(true);
        CoText.text = CurrentRevenue.ToString();
        StartCoroutine(E_GoTitle());
    }
    private IEnumerator E_GoTitle()
    {
        yield return new WaitForSeconds(6f);
        SceneManager.LoadScene("TitleScene");
    }



        public void GoodServeCoin()
    {
        CurrentRevenue += 1;
        CoinAnimator.gameObject.SetActive(true);
        CoinAnimator.Play(0);
        StartCoroutine(StopCoin());
    }

    private IEnumerator StopCoin()
    {
        yield return new WaitForSeconds(2f);
        CoinAnimator.gameObject.SetActive(false);
    }
    public List<AudioClip> EffectClip;
    public List<AudioSource> EffectAudio;
    public void PlayEffectSound(int sourceIndex)
    {
        // 예외 처리: 인덱스가 리스트 범위를 벗어나지 않도록
        if (sourceIndex < 0 || sourceIndex >= EffectAudio.Count)
        {
            Debug.LogWarning("Invalid clip or source index.");
            return;
        }
        EffectAudio[sourceIndex].Play();
    }
    public void PlayEffectSound(int clipIndex, int sourceIndex)
    {
        // 예외 처리: 인덱스가 리스트 범위를 벗어나지 않도록
        if (clipIndex < 0 || clipIndex >= EffectClip.Count || sourceIndex < 0 || sourceIndex >= EffectAudio.Count)
        {
            Debug.LogWarning("Invalid clip or source index.");
            return;
        }

        // 오디오 소스에 사운드 클립을 할당하고 재생
        EffectAudio[sourceIndex].clip = EffectClip[clipIndex];
        EffectAudio[sourceIndex].Play();
    }



}
