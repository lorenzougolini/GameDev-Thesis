using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text.RegularExpressions;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] public PlayingMode playingMode;

    public GameObject gameCanva;

    public GameObject ballPrefab;
    public GameObject playerPrefab;
    public GameObject botPrefab;
    public GameObject defensePrefab;
    public GameObject RLPlayerPrefab;
    public GameObject ILPlayerPrefab;

    public GameObject progressBar;

    public static List<GameObject> gameObjects = new List<GameObject>();
    public static List<Vector3> originalPositions = new List<Vector3>();

    [SerializeField] public float matchDuration;

    private Rigidbody2D ballRb;

    public GameObject ball;
    public GameObject player1;
    public GameObject player2;
 
    public MatchTelemetry.MatchTelemetryStruct matchTelemetry;

    private void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start() {

        // if (playingMode == PlayingMode.TEST)
        // {
        //     StartCoroutine(TestInit());

        //     Gui.S.player1Goals = 0;
        //     Gui.S.player2Goals = 0;
        //     Gui.S.playing = false;

        //     Gui.S.matchDuration = matchDuration;

        //     StartCountdown();
            
        //     return;
        // }
            

        if (MainMenu.mode == PlayingMode.SINGLE)
            StartCoroutine(SetUpSingleGame());
        else if (MainMenu.mode == PlayingMode.MULTI)
            StartCoroutine(SetUpMultiGame());
        else if (MainMenu.mode == PlayingMode.NONE)
            StartCoroutine(SetUpBotGame());

        Gui.S.player1Goals = 0;
        Gui.S.player2Goals = 0;
        Gui.S.playing = false;

        Gui.S.matchDuration = matchDuration;

        matchTelemetry = new MatchTelemetry.MatchTelemetryStruct();
        matchTelemetry.ballTelemetry = new List<MatchTelemetry.BallTelemetry>();
        matchTelemetry.playerTelemetry = new List<MatchTelemetry.PlayerTelemetry>();
        matchTelemetry.opponentTelemetry = new List<MatchTelemetry.OpponentTelemetry>();
        matchTelemetry.scoreTelemetry = new List<MatchTelemetry.ScoreTelemetry>();
        matchTelemetry.matchID = GameIdController.gameId;
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
        p1Move.speed = 6f;
        p1Move.jumpForce = 16f;
        Transform footP1 = player1.transform.Find("Foot");
        footP1.GetComponent<Animator>().SetBool("isFlipped", true);
        AddGameObject(player1);

        // Instantiate player 2
        GameObject player2 = Instantiate(playerPrefab, new Vector3(7, 1, 0), Quaternion.identity);
        player2.tag = "Enemy";
        PlayerMovement p2Move = player2.GetComponent<PlayerMovement>();
        p2Move.playerNumber = 2;
        p2Move.speed = 6f;
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
        ball = Instantiate(ballPrefab, new Vector3(0, 3, 0), Quaternion.identity);
        ballRb = ball.GetComponent<Rigidbody2D>();
        ballRb.isKinematic = true;
        AddGameObject(ball);

        // Instantiate player 1
        player1 = Instantiate(playerPrefab, new Vector3(-7, 1, 0), Quaternion.identity);
        PlayerMovement p1Move = player1.GetComponent<PlayerMovement>();
        p1Move.playerNumber = 1;
        p1Move.speed = 6f;
        p1Move.jumpForce = 16f;
        Transform footP1 = player1.transform.Find("Foot");
        footP1.GetComponent<Animator>().SetBool("isFlipped", true);
        AddGameObject(player1);

        Debug.Log($"id: {GameIdController.gameId} \n substring: {GameIdController.gameId.Substring(3,2)}");

        // Instantiate player 2
        if (GameIdController.gameId.Substring(3,2) == "01")
        {
            player2 = Instantiate(botPrefab, new Vector3(7, 1, 0), Quaternion.identity);
            Debug.Log($"instatiated bot");
        }
        else if (GameIdController.gameId.Substring(3,2) == "02")
        {
            player2 = Instantiate(RLPlayerPrefab, new Vector3(7, 1, 0), Quaternion.identity);
            Debug.Log($"instatiated RL");
        }
        else if (GameIdController.gameId.Substring(3,2) == "03")
        {
            player2 = Instantiate(ILPlayerPrefab, new Vector3(7, 1, 0), Quaternion.identity);
            Debug.Log($"instatiated IL");
        }

        player2.tag = "Enemy";
        player2.TryGetComponent<Bot>(out Bot botMove);
        player2.TryGetComponent<AgentController>(out AgentController agentController);
        if (botMove)
        {
            botMove.speed = 6f;
            botMove.jumpForce = 16f;
            botMove.opponentGoalPosition = new Vector3(-9.8f, 1, 0);
            GameObject defense = Instantiate(defensePrefab, new Vector3(9.5f, 1, 0), Quaternion.identity);
            botMove.defense = defense.transform;
        } 
        else 
        {
            agentController.speed = 6f;
            agentController.jumpForce = 8f;
            agentController.opponentGoal = GameObject.FindGameObjectWithTag("GoalLeft").transform;
            agentController.ownGoal = GameObject.FindGameObjectWithTag("GoalRight").transform;
            agentController.ball = ball.transform;
            agentController.opponent = player1.transform;
        }
        Transform footBot = player2.transform.Find("Foot");
        footBot.GetComponent<Animator>().SetBool("isFlipped", true);

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
        if (agentController)
            agentController.progressBar = progBar2controller;

        
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

    IEnumerator TestInit()
    {
        GameObject ball = GameObject.FindGameObjectWithTag("Ball");
        ballRb = ball.GetComponent<Rigidbody2D>();
        ballRb.isKinematic = true;
        AddGameObject(ball);

        GameObject player1 = GameObject.FindGameObjectWithTag("Player");
        GameObject player2 = GameObject.FindGameObjectWithTag("Enemy");
        AddGameObject(player1);
        AddGameObject(player2);

        // Instantiate progress bar 1
        GameObject progBar1 = Instantiate(progressBar, new Vector3(-469, 426, 0), Quaternion.identity);
        progBar1.transform.SetParent(gameCanva.transform, false);
        ProgressBar progBar1controller = progBar1.GetComponent<ProgressBar>();
        Gui.S.progressBar1 = progBar1controller;
        progBar1controller.associatedPlayer = GameObject.FindGameObjectWithTag("Player");

        // Instantiate progress bar 2
        GameObject progBar2 = Instantiate(progressBar, new Vector3(469, 426, 0), Quaternion.identity);
        progBar2.transform.SetParent(gameCanva.transform, false);
        ProgressBar progBar2controller = progBar2.GetComponent<ProgressBar>();
        Gui.S.progressBar2 = progBar2controller;    
        progBar2controller.associatedPlayer = GameObject.FindGameObjectWithTag("Enemy");
        player2.TryGetComponent<AgentController>(out AgentController agentController);
        player2.TryGetComponent<Bot>(out Bot bot);
        if (agentController)
            agentController.progressBar = progBar2controller;
        if (bot) 
        {
            GameObject defense = Instantiate(defensePrefab, new Vector3(9.5f, 1, 0), Quaternion.identity);
            bot.defense = defense.transform;
        }
        // else if (bot)
        //     bot.progressBar = progBar2controller;
        Transform maskTransform = progBar2.transform.Find("Mask");
        Image maskImage = maskTransform.GetComponent<Image>();
        maskImage.fillOrigin = 0;

        yield return null;
    }

    private void StartCountdown() 
    {
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        Gui.S.goalText.SetActive(true);
        Gui.S.goalText.GetComponent<Text>().text = $"Round {GameIdController.RoundNumber}";
        yield return new WaitForSeconds(1.5f);

        int countdown = 3;

        while (countdown > 0) {
            // Gui.S.goalText.SetActive(true);
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
        if (kickedTag == "Player") 
        {
            GameObject player = gameObjects.Find(obj => obj.CompareTag("Player"));

            if (player)
            {
                player.TryGetComponent(out PlayerMovement playerMovement);
                player.TryGetComponent(out AIPlayerMovement aIPlayerMovement);
                player.TryGetComponent(out Bot bot);
                player.TryGetComponent(out AgentController agentController);
                
                if (playerMovement)
                    playerMovement.TakeDamage(-1);
                if (bot)
                    bot.TakeDamage(-1);
                if (agentController)
                    agentController.TakeDamage(-1);
            }
        }
        else if (kickedTag == "Enemy") {
            GameObject enemy = gameObjects.Find(obj => obj.CompareTag("Enemy"));
            if (enemy)
            {
                enemy.TryGetComponent(out PlayerMovement playerMovement);
                enemy.TryGetComponent(out AIPlayerMovement aIPlayerMovement);
                enemy.TryGetComponent(out Bot bot);
                enemy.TryGetComponent(out AgentController agentController);

                if (playerMovement)
                    playerMovement.TakeDamage(1);
                if (bot)
                    bot.TakeDamage(1);
                if (aIPlayerMovement)
                    aIPlayerMovement.TakeDamage(1);
                if (agentController)
                    agentController.TakeDamage(1);
            }
        }
    }

    public void SubmitAndClearTelemetry()
    {
        StartCoroutine(MatchTelemetry.SubmitMatchTelemetry(matchTelemetry));
        matchTelemetry.playerTelemetry.Clear();
        matchTelemetry.opponentTelemetry.Clear();
        matchTelemetry.ballTelemetry.Clear();
        matchTelemetry.scoreTelemetry.Clear();
        matchTelemetry.player1Goals = 0;
        matchTelemetry.player2Goals = 0;
    }

    private void Update() 
    {
        if (Gui.S.playing && !checkBallInside())
            ResetBall();
    }
    private bool checkBallInside()
    {
        Transform ball = GameObject.FindGameObjectWithTag("Ball").transform;
        return ball.localPosition.x >= -14 && ball.localPosition.x <= 11 && ball.localPosition.y >= -1 && ball.localPosition.y <= 10;
    }
    private void ResetBall()
    {
        Transform ball = GameObject.FindGameObjectWithTag("Ball").transform;
        ball.localPosition = new Vector3(-1.5f, 2f);
    }
}
