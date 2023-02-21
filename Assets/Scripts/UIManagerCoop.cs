using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UIManagerCoop : UIManager
{
    [SerializeField]
    private Text p1ScoreText, p2ScoreText;
    private int p1Score, p2Score;

    [SerializeField]
    private Sprite[] p1LivesSprites, p2LivesSprites;

    [SerializeField]
    private Image p1LivesImg, p2LivesImg;
    private int p1Lives = 3;
    private int p2Lives = 3;

    void Start()
    {
        GM = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        p1ScoreText.text = "Score: " + 0;
        p2ScoreText.text = "Score: " + 0;

        gameOverText.gameObject.SetActive(false);
        gameOverRestartExitText.gameObject.SetActive(false);
    }

    public void UpdateScore(int currScore, int playerId)
    {
        if (playerId == 1)
        {
            p1Score = currScore;
            p1ScoreText.text = "Score: " + p1Score;
        }
        else
        {
            p2Score = currScore;
            p2ScoreText.text = "Score: " + p2Score;
        }
    }

    public void UpdateLives(int currLives, int playerId)
    {
        if (playerId == 1)
        {
            p1LivesImg.sprite = p1LivesSprites[currLives];
            p1Lives = currLives;
        }
        else
        {
            p2LivesImg.sprite = p2LivesSprites[currLives];
            p2Lives = currLives;
        }
        if (p1Lives == 0 && p2Lives == 0)
            GameOverSequence();
    }
}
