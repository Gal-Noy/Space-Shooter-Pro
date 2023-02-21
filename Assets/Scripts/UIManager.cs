using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text scoreText, bestScoreText;
    private int score, bestScore;

    [SerializeField]
    private Sprite[] livesSprites;
    [SerializeField]
    private Image livesImg;

    [SerializeField]
    protected Text gameOverText;
    [SerializeField]
    protected Text gameOverRestartExitText;

    [SerializeField]
    protected GameManager GM;

    // Start is called before the first frame update
    void Start()
    {
        GM = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        bestScore = PlayerPrefs.GetInt("BestScore");

        scoreText.text = "Score: " + 0;
        bestScoreText.text = "Best: " + bestScore;

        gameOverText.gameObject.SetActive(false);
        gameOverRestartExitText.gameObject.SetActive(false);
    }

    public void UpdateScore(int currScore)
    {
        score = currScore;
        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt("BestScore", bestScore);
        }

        scoreText.text = "Score: " + score;
        bestScoreText.text = "Best: " + bestScore;
    }

    public void UpdateLives(int currLives)
    {
        livesImg.sprite = livesSprites[currLives];
        if (currLives == 0)
        {
            GameOverSequence();
        }
    }

    protected void GameOverSequence()
    {
        GM.GameOver();
        gameOverRestartExitText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }

    }

    public void ResumeGame()
    {
        GM.UnPause();
    }

    public void Exit2MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
