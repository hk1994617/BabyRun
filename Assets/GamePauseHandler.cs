using UnityEngine;

public class GamePauseHandler : MonoBehaviour
{
    private bool isGameStarted = false;
    private bool isPaused = true;

    void Start()
    {
        // Игра начинается на паузе, но звук остается включенным
        Time.timeScale = 0f;
        AudioListener.pause = false;
        Debug.Log("Game Started on Pause, Audio ON");
    }

    // Этот метод вызывается, когда приложение теряет или получает фокус
    void OnApplicationFocus(bool hasFocus)
    {
        if (!isGameStarted)
        {
            return; // Не обрабатываем события сворачивания до старта игры
        }

        if (!hasFocus)
        {
            PauseGameWithAudioOff();
        }
        else
        {
            ResumeGameWithAudio();
        }
    }

    void Update()
    {
        // Игра начнется только после первого нажатия на кнопку (или тапа на экране)
        if (isPaused && Input.anyKeyDown)
        {
            StartGame();
        }
    }

    void StartGame()
    {
        isGameStarted = true;
        ResumeGameWithAudio();
    }

    void PauseGameWithAudioOff()
    {
        // Ставим игру на паузу и выключаем звук
        isPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        Debug.Log("Game Paused, Audio OFF");
    }

    void ResumeGameWithAudio()
    {
        // Снимаем игру с паузы и включаем звук
        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Debug.Log("Game Resumed, Audio ON");
    }
}
