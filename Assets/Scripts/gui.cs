using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;
using System.Data;
using UnityEngine.SceneManagement;

public class gui : MonoBehaviour {
    public static gui S;
    public Text scoreText;
    public GameObject goalText;
    public ProgressBar progressBar1;
    public ProgressBar progressBar2;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTimeInSec;
    [SerializeField] GameObject endGameMenu;
    [SerializeField] GameObject pauseMenu;

    public int player1Goals;
    public int player2Goals;

    private bool isPaused = false;

	// Use this for initialization
	void Start () {
        S = this;
        player1Goals = 0;
        player2Goals = 0;
		scoreText = GameObject.Find("ScoreText").GetComponent<Text> ();
        goalText = GameObject.Find("GoalText");
        goalText.SetActive(false);
        Time.timeScale = 1;
    }
	
	// Update is called once per frame
    [ExecuteInEditMode()]
	void Update () {

        // pause and time logic
        if (Input.GetKeyDown    (KeyCode.Escape)) {
            TogglePause();
        }
        if (remainingTimeInSec > 0) {
            remainingTimeInSec -= Time.deltaTime;
        } else if (remainingTimeInSec <= 0) {
            remainingTimeInSec = 0;
            endGame();
            Time.timeScale = 0;
        }
        int min = Mathf.FloorToInt(remainingTimeInSec/60);
        int sec = Mathf.FloorToInt(remainingTimeInSec%60);
        timerText.text = string.Format("{0:00}:{1:00}", min, sec);

        // progress bar logic
        progressBar1.UpdateCurrent(0.05f);
        progressBar2.UpdateCurrent(0.05f);

		scoreText.text = player1Goals.ToString() + " - " + player2Goals.ToString();
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
    public void endGame() {
        endGameMenu.SetActive(true);
        Time.timeScale = 0;
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
