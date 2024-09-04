using UnityEngine;
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
    public TextMeshProUGUI finalScoreText1;
    public TextMeshProUGUI finalScoreText2;
    public GameObject goalText;

    public ProgressBar progressBar1;
    public ProgressBar progressBar2;
    
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] GameObject endGameMenu;
    [SerializeField] GameObject endGameMenuToForm;
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
        playing = false;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
    }

    public void EndGame() {
        playing = false;
        if (GameManager.Instance.playingMode == PlayingMode.TEST)
        {
            GameObject.FindGameObjectWithTag("Player").TryGetComponent<AIPlayerMovement>(out AIPlayerMovement aiPlayerMovement);
            if (aiPlayerMovement)
            {
                aiPlayerMovement.EndEpisode();
            }
        }

        // if (GameIdController.RoundNumber == 3)
        // {
        //     endGameMenu.SetActive(true);
        //     finalScoreText1.text = player1Goals.ToString() + " - " + player2Goals.ToString();
        // }
        // else
        // {
        //     endGameMenuToForm.SetActive(true);
        //     finalScoreText2.text = player1Goals.ToString() + " - " + player2Goals.ToString();
        // }
        endGameMenuToForm.SetActive(true);
        finalScoreText2.text = player1Goals.ToString() + " - " + player2Goals.ToString();

        Debug.Log($"game id is: {GameIdController.gameId}");
        Time.timeScale = 0;

        // GameLogger.Instance.SaveLogsToFile();
        GameManager.Instance.ClearTelemetryData();
    }

    public void Home() {
        SceneManager.LoadScene("MenuScene");
        Time.timeScale = 1;
    }

    public void Resume() {
        playing = true;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }
    
    public void GoToForm() {
        SceneManager.LoadScene("FormScene");
    }

    public void Restart() {
        if (GameManager.Instance.playingMode == PlayingMode.TEST)
        {
            GameObject.FindGameObjectWithTag("Player").TryGetComponent<AIPlayerMovement>(out AIPlayerMovement aiPlayerMovement);
            if (aiPlayerMovement)
            {
                aiPlayerMovement.EndEpisode();
            }
        }

        Time.timeScale = 1;

        string logFilePath = Path.Combine(Application.persistentDataPath, $"GameLog_{System.DateTime.Now.ToString("ddMMyyyy_HHmm")}.txt");
        GameLogger.Instance.SetLogFilePath(logFilePath);
        GameLogger.Instance.ClearLogs();
        GameManager.Instance.ClearTelemetryData();

        GameIdController.SetRoundNumber(0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
