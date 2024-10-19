using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject splineComputer;
    public SplineComputer spline;

    public static float totalGemAmount;
    public float currentGemCollected = 0f;

    public float gemWithIncome;
    public float gemWithStackMoney;
    public float gemByCompleteMap;
    public float gemByStar;

    public bool gainPower = false;

    public GameObject player;

    public GameObject canvas;
    public bool hasWon = false;
    public bool gameOver = false;
    public bool startLosing = true;
    public GameObject powerDisplay;
    public Text powerDisplayText;
    public Text levelNoDisplay;

    public Image secondStar;
    public Image thirdStar;

    public Image fillDistanceBar;
    public GameObject cameraFollowPoint;

    public int levelNo;

    public float totalNumberOfStack;

    public GameObject stackPos;
    public float increamentBlockSpeed;

    public GameObject[] dataObstacle;

    public List<GameObject> dataLevels;

    public GameObject targetObject; // Object to activate only on the first level

    // Массив для хранения материалов скайбоксов
    public Material[] skyboxMaterials;


    private bool isPaused = false;

    private void Awake()
    {
        instance = this;

        Time.timeScale = 0;
        levelNo = PlayerPrefs.GetInt("Level_Number", 0);
        if (levelNo >= dataLevels.Count)
        {
            levelNo = 0;
        }
        totalGemAmount = PlayerPrefs.GetFloat("Total_Gem", 0);

        // Activate object only on the first level
        if (levelNo == 0)
        {
            targetObject.SetActive(true);
        }
        else
        {
            targetObject.SetActive(false);
        }

        // Устанавливаем скайбокс в зависимости от номера уровня
        ChangeSkybox(levelNo);
    }

    // Функция для смены скайбокса
    void ChangeSkybox(int levelIndex)
    {
        // Убедитесь, что индекс не превышает количество материалов скайбоксов
        if (skyboxMaterials.Length > 0 && levelIndex < skyboxMaterials.Length)
        {
            RenderSettings.skybox = skyboxMaterials[levelIndex];
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        levelNoDisplay.text = string.Format("Level " + "{0:0}", levelNo + 1);

        ObstacleSpawn();

        // Setup money stack value through upgrade
        foreach (var stack in GameObject.FindGameObjectsWithTag("Uncollected"))
        {
            stack.GetComponent<MoneyStackValue>().moneyValue += stack.GetComponent<MoneyStackValue>().moneyValue * MenuManager.instance.moneyStackMod;
        }
    }

    private void Update()
    {
        SpeedCalculation();

        LetUsStartTheGame();

        WinScreenPopup();

        LoseCondition();
        LoseScreenPopup();

        fillDistanceBar.GetComponent<Image>().fillAmount = 1 - (cameraFollowPoint.transform.position.x / spline.CalculateLength() + 0.227217f);

        currentGemCollected = gemWithStackMoney + gemByCompleteMap + gemByStar + gemWithIncome;
    }

    void LetUsStartTheGame()
    {
        // Mouse and touch input handling
        if ((Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)) &&
            canvas.transform.GetChild(0).gameObject.activeSelf &&
            !EventSystem.current.IsPointerOverGameObject(0))
        {
            Time.timeScale = 1;
            canvas.transform.GetChild(0).gameObject.SetActive(false);
            canvas.transform.GetChild(1).gameObject.SetActive(true);

            player.GetComponent<Animator>().SetTrigger("Start");
            player.transform.rotation = Quaternion.Euler(0, -90, 0);

            // Deactivate the object after the first touch or click
            if (targetObject.activeSelf)
            {
                targetObject.SetActive(false);
            }
        }
    }

    public void SpeedCalculation()
    {
        for (int i = 0; i < stackPos.transform.childCount; i++)
        {
            stackPos.transform.GetChild(i).gameObject.GetComponent<BuildingBlockMoveSpeed>().buildingBlockMoveSpeed = Constants.STARTING_BUILDING_BLOCK_SPEED + (increamentBlockSpeed * i);
        }
    }

    void WinScreenPopup()
    {
        if (hasWon)
        {
            Time.timeScale = 0;
            canvas.transform.GetChild(2).gameObject.SetActive(true);
        }
    }

    void LoseScreenPopup()
    {
        if (gameOver)
        {
            Time.timeScale = 0;
            canvas.transform.GetChild(3).gameObject.SetActive(true);
        }
    }

    void LoseCondition()
    {
        if (player.GetComponent<PlayerPowerController>().moneyAmount < 0 && startLosing)
        {
            startLosing = false;
            StartCoroutine("LoseScreenDelay");
        }
    }

    IEnumerator LoseScreenDelay()
    {
        yield return new WaitForSeconds(1.5f);
        gameOver = true;
    }

    void ObstacleSpawn()
    {
        Instantiate(dataLevels[levelNo], dataLevels[levelNo].transform.position, dataLevels[levelNo].transform.rotation);

        StartCoroutine("DelayCountingStack");
    }

    IEnumerator DelayCountingStack()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        totalNumberOfStack = GameObject.FindGameObjectsWithTag("Uncollected").Length;
    }
    //void OnApplicationFocus(bool hasFocus)
    //{
    //    if (!hasFocus)
    //    {
    //        PauseGame();
    //    }
    //    else
    //    {
    //        ResumeGame();
    //    }
        
    //}
    //void PauseGame()
    //{
    //    // Ставим игру на паузу
    //    isPaused = true;
    //    Time.timeScale = 0f;

    //    // Отключаем звук
    //    AudioListener.pause = true;

    //    Debug.Log("Game Paused");
    //}
    //void ResumeGame()
    //{
    //    // Снимаем игру с паузы
    //    isPaused = false;
    //    Time.timeScale = 1f;

    //    // Включаем звук
    //    AudioListener.pause = false;

    //    Debug.Log("Game Resumed");
    //}

}
