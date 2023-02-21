using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private bool coopMode;

    private bool isGameOver = false;
    private bool isPaused = false;

    private Animator pauseAnimation;

    private void Start()
    {
        pauseAnimation = GameObject.Find("Pause_menu_panel").GetComponent<Animator>();
        pauseAnimation.updateMode = AnimatorUpdateMode.UnscaledTime;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && isGameOver)
            SceneManager.LoadScene(!coopMode ? 1 : 2); // Current Game Scene

        if (Input.GetKeyDown(KeyCode.X) && isGameOver)
            SceneManager.LoadScene(0); // Main Menu Scene

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (Input.GetKeyDown(KeyCode.P) && !isGameOver)
        {
            if (isPaused)
                UnPause();
            else
                Pause();
        }

    }
    public void GameOver()
    {
        isGameOver = true;
    }

    private void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        StartCoroutine(StartPauseAnimation());
    }

    IEnumerator StartPauseAnimation()
    {
        yield return null;
        pauseAnimation.SetBool("isPaused", true);
    }
    public void UnPause()
    {
        isPaused = false;
        pauseAnimation.SetBool("isPaused", isPaused);
        Time.timeScale = 1f;
    }
}
