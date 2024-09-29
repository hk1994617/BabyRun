using UnityEngine;
using Dreamteck.Splines;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Добавляем AudioSource для музыки
    public AudioSource audioSource;

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
    }

    void Start()
    {
        levelNoDisplay.text = string.Format("Level " + "{0:0}", levelNo + 1);
        ObstacleSpawn();

        // Увеличиваем значение денег в стэках, используя модификатор
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
        if (Input.GetMouseButtonDown(0) && canvas.transform.GetChild(0).gameObject.activeSelf && !EventSystem.current.IsPointerOverGameObject(0))
        {
            Time.timeScale = 1;
            canvas.transform.GetChild(0).gameObject.SetActive(false);
            canvas.transform.GetChild(1).gameObject.SetActive(true);

            // Запуск музыки при начале игры
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

            player.GetComponent<Animator>().SetTrigger("Start");
            player.transform.rotation = Quaternion.Euler(0, -90, 0);
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
}
