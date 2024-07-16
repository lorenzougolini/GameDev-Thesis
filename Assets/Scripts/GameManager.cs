using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{

    public GameObject gameCanva;

    public GameObject ballPrefab;
    public GameObject playerPrefab;
    public GameObject botPrefab;
    public GameObject defensePrefab;

    public GameObject progressBar;

    public static List<GameObject> gameObjects = new List<GameObject>();
    public static List<Vector3> originalPositions = new List<Vector3>();

    [SerializeField] public float matchDuration;

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
            StartCoroutine(SetUpBotGame());

        Gui.S.player1Goals = 0;
        Gui.S.player2Goals = 0;
        Gui.S.playing = false;

        Gui.S.matchDuration = matchDuration;

        string logFilePath = Path.Combine(Application.persistentDataPath, $"GameLog_{System.DateTime.Now.ToString("ddMMyyyy_HHmm")}.txt");
        GameLogger.Instance.SetLogFilePath(logFilePath);
        GameLogger.Instance.LogEvent("Game Started in " + MainMenu.mode + " mode");

        StartCoroutine(LogGameState());
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
        GameObject ball = Instantiate(ballPrefab, new Vector3(0, 3, 0), Quaternion.identity);
        ballRb = ball.GetComponent<Rigidbody2D>();
        ballRb.isKinematic = true;
        AddGameObject(ball);

        // Instantiate player 1
        GameObject player1 = Instantiate(playerPrefab, new Vector3(-7, 1, 0), Quaternion.identity);
        PlayerMovement p1Move = player1.GetComponent<PlayerMovement>();
        p1Move.playerNumber = 1;
        p1Move.speed = 8f;
        p1Move.jumpForce = 16f;
        Transform footP1 = player1.transform.Find("Foot");
        footP1.GetComponent<Animator>().SetBool("isFlipped", true);
        AddGameObject(player1);

        // Instantiate player 2
        GameObject player2 = Instantiate(playerPrefab, new Vector3(7, 1, 0), Quaternion.identity);
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

        // Instantiate progress bar 1
        GameObject progBar1 = Instantiate(progressBar, new Vector3(-469, 426, 0), Quaternion.identity);
        progBar1.transform.SetParent(gameCanva.transform, false);
        ProgressBar progBar1controller = progBar1.GetComponent<ProgressBar>();
        Gui.S.progressBar1 = progBar1controller;
        progBar1controller.associatedPlayer = player1;

        // Instantiate progress bar 2
        GameObject progBar2 = Instantiate(progressBar, new Vector3(469, 426, 0), Quaternion.identity);
        progBar2.transform.SetParent(gameCanva.transform, false);
        ProgressBar progBar2controller = progBar2.GetComponent<ProgressBar>();
        Gui.S.progressBar2 = progBar2controller;    
        progBar2controller.associatedPlayer = player2;
        
        Transform maskTransform = progBar2.transform.Find("Mask");
        Image maskImage = maskTransform.GetComponent<Image>();
        maskImage.fillOrigin = 0;

        StartCountdown();
        yield return null;
    }
    
    IEnumerator SetUpSingleGame() {

        // Instantiate ball
        GameObject ball = Instantiate(ballPrefab, new Vector3(0, 3, 0), Quaternion.identity);
        ballRb = ball.GetComponent<Rigidbody2D>();
        ballRb.isKinematic = true;
        AddGameObject(ball);

        // Instantiate player 1
        GameObject player1 = Instantiate(playerPrefab, new Vector3(-7, 1, 0), Quaternion.identity);
        PlayerMovement p1Move = player1.GetComponent<PlayerMovement>();
        p1Move.playerNumber = 1;
        p1Move.speed = 8f;
        p1Move.jumpForce = 16f;
        Transform footP1 = player1.transform.Find("Foot");
        footP1.GetComponent<Animator>().SetBool("isFlipped", true);
        AddGameObject(player1);

        // Instantiate player 2 bot
        GameObject bot = Instantiate(botPrefab, new Vector3(7, 1, 0), Quaternion.identity);
        bot.tag = "Enemy";
        Bot botMove = bot.GetComponent<Bot>();
        botMove.speed = 6f;
        botMove.jumpForce = 16f;
        botMove.opponentGoalPosition = new Vector3(-9.8f, 1, 0);
        Transform footBot = bot.transform.Find("Foot");
        footBot.GetComponent<Animator>().SetBool("isFlipped", false);

        GameObject defense = Instantiate(defensePrefab, new Vector3(9.5f, 1, 0), Quaternion.identity);
        bot.GetComponent<Bot>().defense = defense.transform;

        AddGameObject(bot);

        // Instantiate progress bar 1
        GameObject progBar1 = Instantiate(progressBar, new Vector3(-469, 426, 0), Quaternion.identity);
        progBar1.transform.SetParent(gameCanva.transform, false);
        ProgressBar progBar1controller = progBar1.GetComponent<ProgressBar>();
        Gui.S.progressBar1 = progBar1controller;
        progBar1controller.associatedPlayer = player1;

        // Instantiate progress bar 2
        GameObject progBar2 = Instantiate(progressBar, new Vector3(469, 426, 0), Quaternion.identity);
        progBar2.transform.SetParent(gameCanva.transform, false);
        ProgressBar progBar2controller = progBar2.GetComponent<ProgressBar>();
        Gui.S.progressBar2 = progBar2controller;    
        progBar2controller.associatedPlayer = bot;
        
        Transform maskTransform = progBar2.transform.Find("Mask");
        Image maskImage = maskTransform.GetComponent<Image>();
        maskImage.fillOrigin = 0;

        StartCountdown();
        yield return null;
    }

    IEnumerator SetUpBotGame() {

        // Instantiate ball
        GameObject ball = Instantiate(ballPrefab, new Vector3(0, 3, 0), Quaternion.identity);
        ballRb = ball.GetComponent<Rigidbody2D>();
        ballRb.isKinematic = true;
        AddGameObject(ball);

        // Instantiate bot 1
        GameObject bot1 = Instantiate(botPrefab, new Vector3(-7, 1, 0), Quaternion.identity);
        bot1.tag = "Player";
        Bot botMove1 = bot1.GetComponent<Bot>();
        botMove1.speed = 3f;
        botMove1.jumpForce = 10f;
        botMove1.opponentGoalPosition = new Vector3(-9.8f, 1, 0);
        Transform bodyP2 = bot1.transform.Find("Body");
        if (bodyP2) 
            bodyP2.GetComponent<SpriteRenderer>().flipX = true;
        Transform footBot1 = bot1.transform.Find("Foot");
        footBot1.GetComponent<Animator>().SetBool("isFlipped", true);

        GameObject defense1 = Instantiate(defensePrefab, new Vector3(-9.5f, 1, 0), Quaternion.identity);
        bot1.GetComponent<Bot>().defense = defense1.transform;

        AddGameObject(bot1);

        // Instantiate bot 2
        GameObject bot2 = Instantiate(botPrefab, new Vector3(7, 1, 0), Quaternion.identity);
        bot2.tag = "Enemy";
        Bot botMove2 = bot2.GetComponent<Bot>();
        botMove2.speed = 3f;
        botMove2.jumpForce = 10f;
        botMove2.opponentGoalPosition = new Vector3(9.8f, 1, 0);
        Transform footBot2 = bot2.transform.Find("Foot");
        footBot2.GetComponent<Animator>().SetBool("isFlipped", false);

        GameObject defense2 = Instantiate(defensePrefab, new Vector3(9.5f, 1, 0), Quaternion.identity);
        bot2.GetComponent<Bot>().defense = defense2.transform;

        AddGameObject(bot2);

        // Instantiate progress bar 1
        GameObject progBar1 = Instantiate(progressBar, new Vector3(-469, 426, 0), Quaternion.identity);
        progBar1.transform.SetParent(gameCanva.transform, false);
        ProgressBar progBar1controller = progBar1.GetComponent<ProgressBar>();
        Gui.S.progressBar1 = progBar1controller;
        progBar1controller.associatedPlayer = bot1;

        // Instantiate progress bar 2
        GameObject progBar2 = Instantiate(progressBar, new Vector3(469, 426, 0), Quaternion.identity);
        progBar2.transform.SetParent(gameCanva.transform, false);
        ProgressBar progBar2controller = progBar2.GetComponent<ProgressBar>();
        Gui.S.progressBar2 = progBar2controller;    
        progBar2controller.associatedPlayer = bot2;
        
        Transform maskTransform = progBar2.transform.Find("Mask");
        Image maskImage = maskTransform.GetComponent<Image>();
        maskImage.fillOrigin = 0;

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
                if (MainMenu.mode == PlayingMode.MULTI)
                    player.GetComponent<PlayerMovement>().TakeDamage(-1);
                else if (MainMenu.mode == PlayingMode.SINGLE)
                    player.GetComponent<PlayerMovement>().TakeDamage(-1);
                else if (MainMenu.mode == PlayingMode.NONE)
                    player.GetComponent<Bot>().TakeDamage(-1);
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

    IEnumerator LogGameState() {
        GameObject player1 = null;
        GameObject player2 = null;

        while (true) {
            yield return new WaitForSeconds(0.05f * 3); // Approximately every 3 frames at 60fps

            float elapsedTime = Time.time;
            float timeRemaining = Gui.S.matchDuration;

            Vector3 player1Position = Vector3.zero;
            Vector3 player2Position = Vector3.zero;
            Vector3 ballPosition = Vector3.zero;

            foreach (GameObject obj in gameObjects) {
                if (obj.GetComponent<PlayerMovement>())
                    if (obj.GetComponent<PlayerMovement>().playerNumber == 1)
                        player1 = obj;
                    else
                        player2 = obj;
                else
                    if (obj.CompareTag("Player"))
                        player1 = obj;
                    else
                        player2 = obj;
            }

            // GameObject player1 = gameObjects.Find(obj => obj.CompareTag("Player") && (obj.GetComponent<PlayerMovement>().playerNumber == 1 || obj.GetComponent<Bot>() != null));
            // GameObject player2 = gameObjects.Find(obj => obj.CompareTag("Enemy") && (obj.GetComponent<PlayerMovement>().playerNumber == 2 || obj.GetComponent<Bot>() != null));
            GameObject ball = gameObjects.Find(obj => obj.CompareTag("Ball"));

            if (player1 != null) player1Position = player1.transform.position;
            if (player2 != null) player2Position = player2.transform.position;
            if (ball != null) ballPosition = ball.transform.position;

            GameLogger.Instance.LogEvent($"Elapsed Time: {elapsedTime}, Time Remaining: {timeRemaining}, Player 1 Position: {player1Position}, Player 2 Position: {player2Position}, Ball Position: {ballPosition}");
        }
    }
}
