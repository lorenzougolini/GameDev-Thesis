﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class Gui : MonoBehaviour {
    public static Gui S;

    public bool playing = false;

    public Text scoreText;
    public TextMeshProUGUI finalScoreText;
    public GameObject goalText;

    public ProgressBar progressBar1;
    public ProgressBar progressBar2;
    
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] GameObject endGameMenu;
    [SerializeField] GameObject pauseMenu;
    
    public float matchDuration;

    public int player1Goals;
    public int player2Goals;

    private bool isPaused = false;
    private bool isEnded = false;

	// Use this for initialization
	void Awake () {
        S = this;

		scoreText = GameObject.Find("ScoreText").GetComponent<Text> ();
        goalText = GameObject.Find("GoalText");
        goalText.SetActive(false);

        // finalScoreText = GameObject.Find("FinalScoreText").GetComponent<Text>();

        Time.timeScale = 1;
    }
	
	// Update is called once per frame
    [ExecuteInEditMode()]
	void Update () {

        // pause logic
        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePause();
        }

        // timer logic
        if (playing) {
            UpdateTimer();
        }
        int min = Mathf.FloorToInt(matchDuration/60);
        int sec = Mathf.FloorToInt(matchDuration%60);
        timerText.text = string.Format("{0:00}:{1:00}", min, sec);

        // progress bar logic
        if (playing && !isPaused && !isEnded) {
            progressBar1.UpdateCurrent(0.1f);
            progressBar2.UpdateCurrent(0.1f);
        }

		scoreText.text = player1Goals.ToString() + " - " + player2Goals.ToString();
    }

    private void UpdateTimer() {
        if (matchDuration > 0) {
            matchDuration -= Time.deltaTime;
        } else if (matchDuration < 0) {
            matchDuration = 0;
            isEnded = true;
            EndGame();
            Time.timeScale = 0;
        }
    }

    public void ScoreGoalText(int i)
    {
        StartCoroutine(ScoreGoalTextEnum(i));
    }

    IEnumerator ScoreGoalTextEnum(int i)
    {
        goalText.SetActive(true);
        goalText.GetComponent<Text>().text = "Player " + i + " goal!";
        yield return new WaitForSeconds(1f);
        goalText.SetActive(false);
    }

    private void TogglePause() {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void Pause() {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
    }

    public void EndGame() {
        endGameMenu.SetActive(true);
        finalScoreText.text = player1Goals.ToString() + " - " + player2Goals.ToString();
        Time.timeScale = 0;

        // GameLogger.Instance.SaveLogsToFile();
        GameManager.Instance.ClearTelemetryData();
    }

    public void Home() {
        SceneManager.LoadScene("MenuScene");
        Time.timeScale = 1;
    }

    public void Resume() {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }

    public void Restart() {
        Time.timeScale = 1;

        string logFilePath = Path.Combine(Application.persistentDataPath, $"GameLog_{System.DateTime.Now.ToString("ddMMyyyy_HHmm")}.txt");
        GameLogger.Instance.SetLogFilePath(logFilePath);
        GameLogger.Instance.ClearLogs();
        GameManager.Instance.ClearTelemetryData();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
