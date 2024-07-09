using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    // public static GameManager Instance { get; private set; }

    public GameObject ballPrefab;
    public GameObject playerPrefab;
    public GameObject botPrefab;
    public GameObject defensePrefab;

    public static List<GameObject> gameObjects = new List<GameObject>();
    public static List<Vector3> originalPositions = new List<Vector3>();

    private Rigidbody2D ballRb;
 
    public GameLogger gameLogger;

    // Start is called before the first frame update
    void Start() {
        if (MainMenu.mode == PlayingMode.SINGLE)
        // if (mode == PlayingMode.SINGLE)
            StartCoroutine(SetUpSingleGame());
        else if (MainMenu.mode == PlayingMode.MULTI)
            StartCoroutine(SetUpMultiGame());
        else if (MainMenu.mode == PlayingMode.NONE)
            StartCoroutine(SetUpTestGame());

        Gui.S.player1Goals = 0;
        Gui.S.player2Goals = 0;
        Gui.S.playing = false;

        string logFilePath = Path.Combine(Application.persistentDataPath, $"GameLog_{System.DateTime.Now.ToString("ddMMyyyy_HHmm")}.txt");
        GameLogger.Instance.SetLogFilePath(logFilePath);
        GameLogger.Instance.LogEvent("Game Started in " + MainMenu.mode + " mode");

    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        ClearGameObjects();
    }

    IEnumerator SetUpMultiGame() {
        // Instantiate ball
        GameObject ball = Instantiate(ballPrefab, new Vector3(0, 2, 0), Quaternion.identity);
        ballRb = ball.GetComponent<Rigidbody2D>();
        ballRb.isKinematic = true;
        AddGameObject(ball);

        // Instantiate player 1
        GameObject player1 = Instantiate(playerPrefab, new Vector3(-7, 0, 0), Quaternion.identity);
        PlayerMovement p1Move = player1.GetComponent<PlayerMovement>();
        p1Move.playerNumber = 1;
        p1Move.speed = 8f;
        p1Move.jumpForce = 16f;
        Transform footP1 = player1.transform.Find("Foot");
        footP1.GetComponent<Animator>().SetBool("isFlipped", true);
        AddGameObject(player1);

        // Instantiate player 2
        GameObject player2 = Instantiate(playerPrefab, new Vector3(7, 0, 0), Quaternion.identity);
        player2.tag = "Enemy";
        PlayerMovement p2Move = player2.GetComponent<PlayerMovement>();
        p2Move.playerNumber = 2;
        p2Move.speed = 8f;
        p2Move.jumpForce = 16f;
        Transform bodyP2 = player2.transform.Find("Body");
        if (bodyP2) 
            bodyP2.GetComponent<SpriteRenderer>().flipX = false;
        Transform footP2 = player2.transform.Find("Foot");
        footP2.GetComponent<Animator>().SetBool("isFlipped", false);
        AddGameObject(player2);

        StartCountdown();
        yield return null;
    }
    
    IEnumerator SetUpSingleGame() {

        // Instantiate ball
        GameObject ball = Instantiate(ballPrefab, new Vector3(0, 2, 0), Quaternion.identity);
        ballRb = ball.GetComponent<Rigidbody2D>();
        ballRb.isKinematic = true;
        AddGameObject(ball);

        // Instantiate player 1
        GameObject player1 = Instantiate(playerPrefab, new Vector3(-7, 0, 0), Quaternion.identity);
        PlayerMovement p1Move = player1.GetComponent<PlayerMovement>();
        p1Move.playerNumber = 1;
        p1Move.speed = 8f;
        p1Move.jumpForce = 16f;
        Transform footP1 = player1.transform.Find("Foot");
        footP1.GetComponent<Animator>().SetBool("isFlipped", true);
        AddGameObject(player1);

        // Instantiate player 2 bot
        GameObject bot = Instantiate(botPrefab, new Vector3(7, 0, 0), Quaternion.identity);
        bot.tag = "Enemy";
        Bot botMove = bot.GetComponent<Bot>();
        botMove.speed = 6f;
        botMove.jumpForce = 16f;
        Transform footBot = bot.transform.Find("Foot");
        footBot.GetComponent<Animator>().SetBool("isFlipped", true);

        GameObject defense = Instantiate(defensePrefab, new Vector3(9.5f, 0, 0), Quaternion.identity);
        bot.GetComponent<Bot>().defense = defense.transform;

        AddGameObject(bot);

        StartCountdown();
        yield return null;
    }

    IEnumerator SetUpTestGame() {

        GameObject ball = Instantiate(ballPrefab, new Vector3(0, 2, 0), Quaternion.identity);
        ballRb = ball.GetComponent<Rigidbody2D>();
        ballRb.isKinematic = true;
        AddGameObject(ball);

        StartCountdown();
        yield return null;
    }

    void StartCountdown() {
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown() {
        int countdown = 3;
        while (countdown > 0) {
            Gui.S.goalText.SetActive(true);
            Gui.S.goalText.GetComponent<Text>().text = countdown.ToString();
            yield return new WaitForSeconds(1f);
            countdown--;
        }
        Gui.S.goalText.GetComponent<Text>().text = "GO!";
        Gui.S.playing = true;
        ballRb.isKinematic = false;

       GameLogger.Instance.LogEvent("Game Started");
        
        yield return new WaitForSeconds(1f);
        Gui.S.goalText.SetActive(false);
    }

    public void AddGameObject(GameObject obj) {
        if (!gameObjects.Contains(obj)) {
            gameObjects.Add(obj);
            originalPositions.Add(obj.transform.position);
            
            GameLogger.Instance.LogEvent("Instantiated GameObject: " + obj.name + " at Position: " + obj.transform.position);
        }
    }

    public static void ClearGameObjects() {
        gameObjects.Clear();
        originalPositions.Clear();
    }

    public static void KickOpponent(string kickedTag) {
        if (kickedTag == "Player") {
            GameObject player = gameObjects.Find(obj => obj.CompareTag("Player"));
            if (player)
               player.GetComponent<PlayerMovement>().TakeDamage(-1);
        }
        else if (kickedTag == "Enemy") {
            GameObject enemy = gameObjects.Find(obj => obj.CompareTag("Enemy"));
            if (enemy)
                if (MainMenu.mode == PlayingMode.MULTI)
                    enemy.GetComponent<PlayerMovement>().TakeDamage(1);
                else
                    enemy.GetComponent<Bot>().TakeDamage(1);
        }
    }
}
